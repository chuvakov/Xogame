using XOgame.Services.Account.Dto;

namespace XOgame.Services;

public interface IAccountService
{
    Task<bool> IsExist(AccountInput input);
    Task Create(AccountInput input);
}