using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace XOgame.Authentication;

//JWT tokken json web tokken
public class AuthSettings
{
    public string Issuer { get; private set; } //Кто издатель токена
    public string Audience { get; private set; } //Получатель токена (в нашем случае наше приложение XOgame(не пользователь))
    public string Key { get; private set; } //Ключ на основе которого будет шифрование
    public int LifeTime { get; private set; } //Время жизни токена

    public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    // Зашифрованный ключ
}