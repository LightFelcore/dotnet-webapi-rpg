using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_webapi_rpg.DTOs.Fight;
using dotnet_webapi_rpg.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_webapi_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [HttpPost("WeaponAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto request) {
            return Ok(await _fightService.WeaponAttack(request));
        }

        [HttpPost("SkillAttack")]
        public async Task<ActionResult<ServiceResponse<AttackResultDto>>> SkillAttack(SkillAttackDto request) {
            return Ok(await _fightService.SkillAttack(request));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDto>>> Fight(FightRequestDto request) {
            return Ok(await _fightService.Fight(request));
        }

        [HttpGet("HighScore")]
        public async Task<ActionResult<ServiceResponse<List<HighScoreDto>>>> GetHighScore() {
            return Ok(await _fightService.GetHighScore());
        }
    }
}