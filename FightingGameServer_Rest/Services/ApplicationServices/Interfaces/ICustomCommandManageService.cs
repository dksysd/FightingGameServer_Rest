using FightingGameServer_Rest.Dtos.CustomCommand;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface ICustomCommandManageService
{
    Task<IEnumerable<CustomCommandDto>> GetCustomCommands(int userId);

    Task<bool> SetCustomCommands(IEnumerable<UpdateCustomCommandRequestDto> requests, int userId);
}