namespace XOgame.Common.Exceptions;

public class UserFriendlyException : Exception
{
    public int Code { get; set; }
    
    public UserFriendlyException(string message) : base(message)
    {
    }

    public UserFriendlyException(string message, int code) : base(message)
    {
        Code = code;
    }
}