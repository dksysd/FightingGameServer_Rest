using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using FightingGameServer_Rest.Dtos.CustomCommand;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using MySqlConnector;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation.Possible")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("Usage", "CA2208:올바른 인수 예외를 인스턴스화하세요.")]
public class CustomCommandManageService(
    ICustomCommandService customCommandService,
    ICharacterService characterService,
    ISkillService skillService,
    IPlayerService playerService)
    : ICustomCommandManageService
{
    public async Task<IEnumerable<CustomCommandDto>> GetCustomCommands(int userId)
    {
        List<Player> players = await playerService.GetPlayerByUserId(userId);
        Player player = players.First();
        IEnumerable<CustomCommand> customCommands = await customCommandService.GetCustomCommandsByPlayerId(player.Id);
        return customCommands.Select(customCommand => customCommand.ToDto());
    }

    public async Task<bool> SetCustomCommands(
        IEnumerable<UpdateCustomCommandRequestDto> requests,
        int userId)
    {
        List<Player> players = await playerService.GetPlayerByUserId(userId);
        Player player = players.First();

        IEnumerable<UpdateCustomCommandRequest> validatedRequests =
            await ValidateCustomCommandsDtos(requests, player.Id);

        List<CustomCommand> customCommands =
            (List<CustomCommand>)await customCommandService.GetCustomCommandsByPlayerId(player.Id);

        foreach (UpdateCustomCommandRequest validatedRequest in validatedRequests)
        {
            CustomCommand? existCustomCommand = customCommands.FirstOrDefault(customCommand =>
                customCommand.Character == validatedRequest.CustomCommand.Character &&
                customCommand.Skill == validatedRequest.CustomCommand.Skill);
            switch (validatedRequest.Action)
            {
                case UpdateCustomCommandRequestDto.ActionType.Create:
                    if (existCustomCommand is not null)
                        throw new InvalidOperationException("Already exist custom command");
                    await customCommandService.CreateCustomCommand(validatedRequest.CustomCommand);
                    break;
                case UpdateCustomCommandRequestDto.ActionType.Update:
                    if (existCustomCommand is null) throw new InvalidOperationException("Custom command not found");
                    validatedRequest.CustomCommand.Id = existCustomCommand.Id;
                    await customCommandService.UpdateCustomCommand(validatedRequest.CustomCommand);
                    break;
                case UpdateCustomCommandRequestDto.ActionType.Delete:
                    if (existCustomCommand is null) throw new InvalidOperationException("Custom command not found");
                    validatedRequest.CustomCommand.Id = existCustomCommand.Id;
                    await customCommandService.DeleteCustomCommand(validatedRequest.CustomCommand);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Parameter {nameof(validatedRequest.Action)} is not supported");
            }
        }
        
        return true;
    }

    private async Task<IEnumerable<UpdateCustomCommandRequest>> ValidateCustomCommandsDtos(
        IEnumerable<UpdateCustomCommandRequestDto> updateCustomCommandRequestDtos, int playerId)
    {
        List<Character> characters = (List<Character>)await characterService.GetAllCharacters();
        List<Skill> skills = (List<Skill>)await skillService.GetAllSkills();

        return updateCustomCommandRequestDtos.Select(updateCustomCommandRequestDto =>
        {
            CustomCommandDto customCommandDto = updateCustomCommandRequestDto.CustomCommand;
            Character? character =
                characters.FirstOrDefault(character => character.Name == customCommandDto.CharacterName);
            if (character is null)
            {
                throw new InvalidOperationException($"Character {customCommandDto.CharacterName} does not exist");
            }

            Skill? skill = skills.FirstOrDefault(skill => skill.Name == customCommandDto.SkillName);
            if (skill is null)
            {
                throw new InvalidOperationException($"Skill {customCommandDto.SkillName} does not exist");
            }

            return new UpdateCustomCommandRequest
            {
                Action = updateCustomCommandRequestDto.Action,
                CustomCommand = new CustomCommand
                {
                    Command = customCommandDto.Command,
                    Character = character,
                    Skill = skill,
                    PlayerId = playerId,
                    CharacterId = character.Id,
                    SkillId = skill.Id
                }
            };
        });
    }

    private class UpdateCustomCommandRequest
    {
        public required UpdateCustomCommandRequestDto.ActionType Action { get; set; }
        public required CustomCommand CustomCommand { get; set; }
    }
}