using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Domains.CustomCommand.Dtos;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation.Possible")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("Usage", "CA2208:올바른 인수 예외를 인스턴스화하세요.")]
public class CustomCommandManageService(
    ICustomCommandService customCommandService,
    ICharacterService characterService,
    ISkillService skillService)
    : ICustomCommandManageService
{
    public async Task<IEnumerable<CustomCommandDto>> GetCustomCommands(int playerId)
    {
        IEnumerable<CustomCommand> customCommands = await customCommandService.GetCustomCommandsByPlayerId(playerId);
        return customCommands.Select(customCommand => customCommand.ToDto());
    }

    public async Task<bool> SetCustomCommands(IEnumerable<CustomCommandDto> requests, int playerId)
    {
        IEnumerable<CustomCommand> customCommands = await ValidateCustomCommands(requests, playerId);
        await customCommandService.DeleteAllCustomCommands(playerId);
        await Task.WhenAll(customCommands.Select(customCommandService.CreateCustomCommand));
        return true;
    }

    private async Task<IEnumerable<CustomCommand>> ValidateCustomCommands(
        IEnumerable<CustomCommandDto> customCommandDtos, int playerId)
    {
        IEnumerable<Character> characters = await characterService.GetAllCharacters();
        IEnumerable<Skill> skills = await skillService.GetAllSkills();

        return customCommandDtos.Select(customCommandDto =>
        {
            Character? character =
                characters.FirstOrDefault(character => character.Name == customCommandDto.CharacterName);
            if (character == null)
            {
                throw new InvalidOperationException($"Character {customCommandDto.CharacterName} does not exist");
            }

            Skill? skill = skills.FirstOrDefault(skill => skill.Name == customCommandDto.SkillName);
            if (skill == null)
            {
                throw new InvalidOperationException($"Skill {customCommandDto.SkillName} does not exist");
            }

            return new CustomCommand
            {
                Command = customCommandDto.Command,
                PlayerId = playerId,
                CharacterId = character.Id,
                SkillId = skill.Id,
                Character = character,
                Skill = skill
            };
        });
    }
}