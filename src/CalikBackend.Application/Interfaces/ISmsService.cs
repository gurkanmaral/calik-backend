namespace CalikBackend.Application.Interfaces;

public interface ISmsService
{
    Task SendOtpAsync(string phoneNumber, string otp);
}
