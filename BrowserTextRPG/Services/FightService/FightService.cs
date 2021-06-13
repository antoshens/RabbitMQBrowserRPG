using AutoMapper;
using BrowserTextRPG.Commands;
using BrowserTextRPG.Data;
using BrowserTextRPG.DTOModel.Fight;
using BrowserTextRPG.Events;
using BrowserTextRPG.Model;
using BrowserTextRPG.Services.CharacterService;
using BrowserTextRPG.Services.NotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using RabbitMQ.Core.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrowserTextRPG.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _dbContext;
        private readonly ICharacterService _characterService;
        private readonly IMapper _mapper;
        private readonly ILogger<FightService> _logger;
        private readonly JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();
        private readonly IEventBus _bus;
        private readonly IHttpContextAccessor _httpContextAcessor;
        private readonly INotificationService _notificationService;

        public Character AttackerCharacter { get; set; }

        public FightService(
            DataContext dbContext,
            ICharacterService characterService,
            IMapper mapper,
            ILogger<FightService> logger,
            IEventBus bus,
            IHttpContextAccessor httpContextAcessor,
            INotificationService notificationService)
        {
            this._dbContext = dbContext;
            this._characterService = characterService;
            this._mapper = mapper;
            this._logger = logger;
            this._bus = bus;
            this._httpContextAcessor = httpContextAcessor;
            this._notificationService = notificationService;
        }

        private bool ValidateOpponentCharacter(Character opponent, out int errorCode, out string errorMessage)
        {
            bool validationResult = true;
            errorCode = 0;
            errorMessage = default;

            if (opponent == null)
            {
                errorCode = 1007;
                errorMessage = "Character has not been found";

                validationResult = false;
            }

            if (opponent.Health <= 0)
            {
                errorCode = 1010;
                errorMessage = "Opponent character is already dead";

                validationResult = false;
            }

            return validationResult;
        }

        private int DoWeaponAttack(Character attacker, Character opponent)
        {
            var weaponDamage = (attacker.Weapon != null ? attacker.Weapon.Damage : 0) + new Random().Next(0, attacker.Strength);
            var resultDamage = weaponDamage - new Random().Next(0, opponent.Agility);

            return resultDamage > 0 ? resultDamage : 0;
        }

        private int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            var skillDamage = skill.Damage + new Random().Next(0, attacker.Intelligence);
            var resultDamage = skillDamage - new Random().Next(0, opponent.Agility);

            return resultDamage > 0 ? resultDamage : 0;
        }

        private int GetUserID() => int.Parse(this._httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        public async Task<GatewayResponse<SkillAttackResultDto>> SkillAttack(SkillAttackDto skillAttack)
        {
            var response = new GatewayResponse<SkillAttackResultDto>();

            this._logger.LogInformation($"Get 'SkillAttack' request: {{ {this._jsonSerializer.Serialize(skillAttack)} }}.");

            // If attacker character exists and it belongs to the authorized user
            var attackerCharacterResponse = await this._characterService.GetByID(skillAttack.AttackerId);

            if (attackerCharacterResponse.Fault != null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = attackerCharacterResponse.Fault.ErrorCode,
                    ErrorMessage = attackerCharacterResponse.Fault.ErrorMessage
                };

                return response;
            }

            // If opponent character exists
            var opponentChar = await this._dbContext.Characters.FirstOrDefaultAsync(ch => ch.Id == skillAttack.OpponentId);

            if (!ValidateOpponentCharacter(opponentChar, out int errorCode, out string errorMessage))
            {
                response.Fault = new Fault
                {
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };

                this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                return response;
            }

            var attackerChar = await _dbContext.Characters.FirstAsync(ch => ch.Id == attackerCharacterResponse.Data.Id);

            // If skill exists and it belongs to the attacker character
            var isAttackerHaveSkill = this._dbContext.CharacterSkills
                .Any(cs => cs.SkillId == skillAttack.SkillId && cs.CharacterId == skillAttack.AttackerId);
            var skill = await this._dbContext.Skills.FirstOrDefaultAsync(s => s.Id == skillAttack.SkillId);

            if (!isAttackerHaveSkill || skill == null)
            {
                response.Fault = skill != null
                ? new Fault
                {
                    ErrorCode = 1009,
                    ErrorMessage = $"{attackerChar.Name} does not know {skill?.Name}"
                }
                : new Fault
                {
                    ErrorCode = 1008,
                    ErrorMessage = "Skill has not been found"
                };

                this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                return response;
            }

            var damage = DoSkillAttack(attackerChar, opponentChar, skill);
            opponentChar.Health -= damage;

            if (opponentChar.Health <= 0)
            {
                attackerChar.Victories++;
                opponentChar.Defeats++;
            }

            attackerChar.Fights++;
            opponentChar.Fights++;

            // Publish event to RabbitMQ
            var createSkillAttack = new SkillAttackCommand(
                    skillAttack.AttackerId,
                    skillAttack.OpponentId,
                    damage
                );

            var rpcResponse = this._bus.RPCCall<SkillAttackCommand, AttackCreatedEvent, AttackFinishedEvent>(createSkillAttack);

            if (rpcResponse.Success)
            {
                // Update DB context
                this._dbContext.Characters.UpdateRange(attackerChar, opponentChar);

                await this._dbContext.SaveChangesAsync();

                response.Data = new SkillAttackResultDto
                {
                    AttackerName = attackerChar.Name,
                    AttackerHP = attackerChar.Health,
                    OpponentName = opponentChar.Name,
                    OpponentHP = opponentChar.Health,
                    Damage = damage,
                    SkillName = skill.Name
                };

                this._logger.LogInformation($"Send reply from 'SkillAttack': {{ {this._jsonSerializer.Serialize(response.Data)} }}");

                // Send WebSocket push notification
                await this._notificationService.PushMessage($"Skill attack from '{attackerChar.Name}' to '{opponentChar.Name}' has been made with {damage} damage.", GetUserID());

                return response;
            }
            else
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1,
                    ErrorMessage = rpcResponse.Message
                };

                this._logger.LogInformation($"Send reply from 'SkillAttack': {{ {this._jsonSerializer.Serialize(response.Fault)} }}");

                return response;
            }
        }

        public async Task<GatewayResponse<WeaponAttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            // Perform logging and validation
            this._logger.LogInformation($"Get 'WeaponAttack' request: {{ {this._jsonSerializer.Serialize(weaponAttack)} }}.");

            var response = new GatewayResponse<WeaponAttackResultDto>();

            var attackerCharacterResponse = await this._characterService.GetByID(weaponAttack.AttackerId);

            if (attackerCharacterResponse.Fault != null)
            {
                response.Fault = new Fault
                {
                    ErrorCode = attackerCharacterResponse.Fault.ErrorCode,
                    ErrorMessage = attackerCharacterResponse.Fault.ErrorMessage
                };

                this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                return response;
            }

            var opponentChar = await this._dbContext.Characters.FirstOrDefaultAsync(ch => ch.Id == weaponAttack.OpponentId);

            if (!ValidateOpponentCharacter(opponentChar, out int errorCode, out string errorMessage))
            {
                response.Fault = new Fault
                {
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };

                this._logger.LogWarning($"Request failed due to {response.Fault.ErrorMessage}.");

                return response;
            }

            // Calculate damage
            var attackerChar = await _dbContext.Characters.FirstAsync(ch => attackerCharacterResponse.Data != null
                && ch.Id == attackerCharacterResponse.Data.Id);
            var damage = DoWeaponAttack(attackerChar, opponentChar);
            opponentChar.Health -= damage;

            if (opponentChar.Health <= 0)
            {
                attackerChar.Victories++;
                opponentChar.Defeats++;
            }

            attackerChar.Fights++;
            opponentChar.Fights++;

            // Publish event to RabbitMQ
            var createWeaponAttack = new WeaponAttackCommand(
                    weaponAttack.AttackerId,
                    weaponAttack.OpponentId,
                    damage
                );

            //await this._bus.SendCommand(createWeaponAttack);
            var rpcResponse = this._bus.RPCCall<WeaponAttackCommand, AttackCreatedEvent, AttackFinishedEvent>(createWeaponAttack);

            if (rpcResponse.Success)
            {
                // Update Character DB table
                this._dbContext.Characters.UpdateRange(attackerChar, opponentChar);

                await this._dbContext.SaveChangesAsync();

                // Send a response
                response.Data = new WeaponAttackResultDto
                {
                    AttackerName = attackerChar.Name,
                    AttackerHP = attackerChar.Health,
                    OpponentName = opponentChar.Name,
                    OpponentHP = opponentChar.Health,
                    Damage = damage
                };

                this._logger.LogInformation($"Send reply from 'WeaponAttack': {{ {this._jsonSerializer.Serialize(response.Data)} }}");

                // Send WebSocket push notification
                await this._notificationService.PushMessage($"Weapon attack from '{attackerChar.Name}' to '{opponentChar.Name}' has been made with {damage} damage.", GetUserID());

                return response;
            }
            else
            {
                response.Fault = new Fault
                {
                    ErrorCode = 1,
                    ErrorMessage = rpcResponse.Message
                };

                this._logger.LogInformation($"Send reply from 'WeaponAttack': {{ {this._jsonSerializer.Serialize(response.Fault)} }}");

                return response;
            }
        }

        public async Task<GatewayResponse<FightResponseDto>> AutoFight(FightRequestDto fightRequest)
        {
            this._logger.LogInformation($"Get 'AutoFight' request: {{ {this._jsonSerializer.Serialize(fightRequest)} }}.");

            var response = new GatewayResponse<FightResponseDto>
            {
                Data = new FightResponseDto()
            };

            var charactersInFight = await this._dbContext.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                .Where(ch => fightRequest.CharacterIds.Any(id => id == ch.Id)).ToListAsync();

            bool isWinnerExist = false;
            int defeats = 0;

            while (!isWinnerExist)
            {
                foreach (var character in charactersInFight)
                {
                    if (character.Health <= 0) continue;

                    var possibleOpponents = charactersInFight.Where(c => c.Id != character.Id && c.Health > 0).ToList();
                    var opponent = possibleOpponents[new Random().Next(0, possibleOpponents.Count)];

                    var useWeapon = new Random().Next(0, 2) == 0;
                    int damage = 0;
                    string attackUsed = String.Empty;

                    if (useWeapon)
                    {
                        attackUsed = character.Weapon.Name;
                        damage = DoWeaponAttack(character, opponent);
                    }
                    else
                    {
                        var skill = character.CharacterSkills[new Random().Next(0, character.CharacterSkills.Count)].Skill;
                        attackUsed = skill.Name;
                        damage = DoSkillAttack(character, opponent, skill);
                    }

                    response.Data.LogMessages
                        .Add($"'{character.Name}' hits '{opponent.Name}' on {(damage > 0 ? damage : 0)} HP using '{attackUsed}'.");

                    opponent.Health -= damage;
                    opponent.Fights++;
                    character.Fights++;

                    if (opponent.Health <= 0)
                    {
                        response.Data.LogMessages.Add($"'{opponent.Name}' has been defeated!");

                        opponent.Defeats++;
                        character.Victories++;
                        defeats++;

                        if (charactersInFight.Count == defeats + 1)
                        {
                            isWinnerExist = true;
                            response.Data.LogMessages.Add($"'{character.Name}' wins with {character.Health} HP left!!!");
                        }
                    }

                    this._dbContext.UpdateRange(character, opponent);

                    await this._dbContext.SaveChangesAsync();
                }
            }

            charactersInFight.ForEach(ch =>
            {
                ch.Health = 100;
            });

            this._dbContext.UpdateRange(charactersInFight);
            await this._dbContext.SaveChangesAsync();

            this._logger.LogInformation($"Send reply from 'AutoFight': {{ {this._jsonSerializer.Serialize(response.Data)} }}");

            return response;
        }

        public async Task<GatewayResponse<List<GetHighscoreDto>>> GetHighscore()
        {
            this._logger.LogInformation($"Get 'AutoFight' request.");

            var response = new GatewayResponse<List<GetHighscoreDto>>();

            var highscore = await this._dbContext.Characters
                .Where(ch => ch.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            response.Data = highscore.Select(c => this._mapper.Map<Character, GetHighscoreDto>(c)).ToList();

            this._logger.LogInformation($"Send reply from 'GetHighscore': {{ {this._jsonSerializer.Serialize(response.Data)} }}");

            return response;
        }

        public async Task<GatewayResponse<bool>> GetWebSocketConnection(HttpContext context)
        {
            var userID = this.GetUserID();
            var user = this._httpContextAcessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            return await this._notificationService.InitiateWebSocketConnection(context);
        }
    }
}
