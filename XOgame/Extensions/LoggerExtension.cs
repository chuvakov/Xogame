namespace XOgame.Extensions;

public static class LoggerExtension
{
    public static void Info(this ILogger logger, string message)
    {
        logger.LogInformation(message);
    }
    
    public static void Info(this ILogger logger, Exception exception, string message)
    {
        logger.LogInformation(exception, message);
    }
    
    public static void Warning(this ILogger logger, string message)
    {
        logger.LogWarning(message);
    }
    
    public static void Warning(this ILogger logger, Exception exception, string message)
    {
        logger.LogWarning(exception, message);
    }
    
    public static void Error(this ILogger logger, string message)
    {
        logger.LogError(message);
    }
    
    public static void Error(this ILogger logger, Exception exception, string message)
    {
        logger.LogError(exception, message);
    }
    
    public static void Error(this ILogger logger, Exception exception)
    {
        logger.LogError(exception, exception.Message);
    }
}