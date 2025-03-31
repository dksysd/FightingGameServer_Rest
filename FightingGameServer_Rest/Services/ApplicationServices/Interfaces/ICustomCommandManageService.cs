using FightingGameServer_Rest.Domains.CustomCommand.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface ICustomCommandManageService
{
    Task<IEnumerable<CustomCommandDto>> GetCustomCommands(int userId);

    Task<bool> SetCustomCommands(IEnumerable<CustomCommandDto> requests, int userId);
}