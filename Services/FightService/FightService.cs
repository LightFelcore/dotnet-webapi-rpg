using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_webapi_rpg.Data;
using dotnet_webapi_rpg.DTOs.Fight;
using Microsoft.EntityFrameworkCore;

namespace dotnet_webapi_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto> {
                Data = new FightResultDto()
            };

            try
            {
                // Grab all the characters from the database that matches the passed character ids from the request
                List<Character> characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                // loops stop when the first character is defeated
                while(!defeated) {
                    // Let every character fight in order
                    foreach(Character attacker in characters) {
                        // Grab all the other characters in the array (exculude the current attacker of the loop)
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        // Grab randomly one opponent of the opponents list above
                        Character opponent = opponents[new Random().Next(opponents.Count)];
                        
                        // Declaration of two variables for the resulting log message after fight
                        int damage = 0;
                        String attackUsed = String.Empty;

                        // Boolean to determine if a weapon or skill has been used for the attack
                        // 0 ==> Weapon
                        // 1 ==> Skill
                        bool usedWeapon = new Random().Next(2) == 0;
                        
                        // Attacker chose a Weapon to attack
                        if(usedWeapon) {
                            // Name of the used weapon
                            attackUsed = attacker.Weapon.Name;
                            // Weapon dealt damage
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        // Attacker chose a Skill to attack
                        else {
                            // Grab a random skill that the attacker owns
                            Skill skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            // Name of the used skill
                            attackUsed = skill.Name;
                            // Skill dealt damage
                            damage = DoSkillAttack(attacker, skill, opponent);
                        }

                        // Add a log message for the response
                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with following damage: {(damage >= 0 ? damage : 0)}.");

                        // Whenever an opponent is dead
                        if(opponent.HitPoints <= 0) {
                            // Stop the while loop in case of a dead opponent
                            defeated = true;
                            // Increase the attacker's victories count
                            attacker.Victories++;
                            // Increase the opponent's defeats count
                            opponent.Defeats++;
                            // Add a log message for the response
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            // Break the while loop
                            break;
                        }
                    }
                }
                // RESET: Increase the fights count and restore HP of each participant
                foreach(Character character in characters) {
                    character.Fights++;
                    character.HitPoints = 100;
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                // Get the attacker with corresponding weapon
                Character attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                Skill skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill";
                    return response;
                }

                // Get the opponent
                Character opponent = await _context.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker == null || opponent == null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                int damage = DoSkillAttack(attacker, skill, opponent);
                // If the HP of the openent is below or equals to zero, then the attack won.
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }

                await _context.SaveChangesAsync();

                // Format the response
                response.Data = new AttackResultDto
                {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHitPoints = attacker.HitPoints,
                    OpponentHitPoints = opponent.HitPoints,
                    Damage = damage,
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Character attacker, Skill skill, Character opponent)
        {
            // Create a random damage based on the damage of the weapon and the strength of the user multiplied by a random number
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            // The oponent has also defence. The damage of the attacker will be reduced based on the defence of the oponent
            damage -= new Random().Next(opponent.Defense);

            // If the damage is lower or equals to zero it means that the opponent still has some defence
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
                if (opponent.HitPoints <= 0) opponent.HitPoints = 0;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                // Get the attacker with corresponding weapon
                Character attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                // Get the opponent
                Character opponent = await _context.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker == null || opponent == null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                int damage = DoWeaponAttack(attacker, opponent);
                // If the HP of the openent is below or equals to zero, then the attack won.
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"{opponent.Name} has been defeated!";
                }

                await _context.SaveChangesAsync();

                // Format the response
                response.Data = new AttackResultDto
                {
                    AttackerName = attacker.Name,
                    OpponentName = opponent.Name,
                    AttackerHitPoints = attacker.HitPoints,
                    OpponentHitPoints = opponent.HitPoints,
                    Damage = damage,
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            // Create a random damage based on the damage of the weapon and the strength of the user multiplied by a random number
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            // The oponent has also defence. The damage of the attacker will be reduced based on the defence of the oponent
            damage -= new Random().Next(opponent.Defense);

            // If the damage is lower or equals to zero it means that the opponent still has some defence
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            ServiceResponse<List<HighScoreDto>> response = new ServiceResponse<List<HighScoreDto>>();
            // Get the high scores of characters that participated at fights
            // Order result by victories descending then by defeats ascending
            List<Character> characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();
            
            response.Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList();
            return response;
        }
    }
}