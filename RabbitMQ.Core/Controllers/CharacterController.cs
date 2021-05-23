using BrowserTextRPG.DTOModel.Character;
using BrowserTextRPG.Model;
using BrowserTextRPG.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrowserTextRPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            this._characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<GatewayResponse<List<GetCharacterDto>>>> GetAll()
        {
            var response = await this._characterService.GetAll();

            return Ok(response);
        }

        [HttpGet("GetByID/{id}")]
        public async Task<ActionResult<GatewayResponse<GetCharacterDto>>> GetByID(int id)
        {
            var response = await this._characterService.GetByID(id);

            return Ok(response);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<GatewayResponse<GetCharacterDto>>> Add([FromBody]AddCharacterDto character)
        {
            var response = await this._characterService.Add(character);

            return Ok(response);
        }

        [HttpPut("Modify")]
        public async Task<ActionResult<GatewayResponse<GetCharacterDto>>> Modify([FromBody]ModifyCharacterDto character)
        {
            var response = await this._characterService.Modify(character);

            return Ok(response);
        }

        [HttpPost("AddSkill")]
        public async Task<ActionResult<GatewayResponse<GetCharacterDto>>> AddSkill([FromBody]AddCharacterSkillDto characterSkill)
        {
            var response = await this._characterService.AddSkill(characterSkill);

            return Ok(response);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<GatewayResponse<List<GetCharacterDto>>>> Delete([FromBody]DeleteCharacterDto character)
        {
            var response = await this._characterService.Delete(character);

            return Ok(response);
        }
    }
}
