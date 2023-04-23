namespace Infrastructure.Common;
public interface ISmsSender
{
    Task SendSmsAsync(string number, string message);
}
