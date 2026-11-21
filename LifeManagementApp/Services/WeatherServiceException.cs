namespace LifeManagementApp.Services;

// The only exception that leaves the weather service. The message is written for the user.
public class WeatherServiceException : Exception
{
    public WeatherServiceException(string message, Exception? inner = null)
        : base(message, inner)
    {
    }
}
