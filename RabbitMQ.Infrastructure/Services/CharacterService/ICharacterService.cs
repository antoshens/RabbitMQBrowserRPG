using BrowserTextRPG.DTOModel.Character;
using BrowserTextRPG.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<GatewayResponse<List<GetCharacterDto>>> GetAll();
        Task<GatewayResponse<GetCharacterDto>> GetByID(int id);
        Task<GatewayResponse<GetCharacterDto>> Add(AddCharacterDto newCharacter);
        Task<GatewayResponse<GetCharacterDto>> Modify(ModifyCharacterDto updatedCharacter);
        Task<GatewayResponse<List<GetCharacterDto>>> Delete(DeleteCharacterDto deletedCharacter);
        Task<GatewayResponse<GetCharacterDto>> AddSkill(AddCharacterSkillDto characterSkill);
    }
}
