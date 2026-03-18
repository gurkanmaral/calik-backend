using CalikBackend.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CalikBackend.Infrastructure.Services;

public class ConsoleSmsService : ISmsService
{
    private readonly ILogger<ConsoleSmsService> _logger;

    public ConsoleSmsService(ILogger<ConsoleSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string phoneNumber, string otp)
    {
        _logger.LogInformation("[SMS MOCK] Sending OTP {Otp} to {PhoneNumber}", otp, phoneNumber);
        return Task.CompletedTask;
    }
}
