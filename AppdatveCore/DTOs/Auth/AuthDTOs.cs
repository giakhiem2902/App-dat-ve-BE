namespace AppdatveCore.DTOs.Auth;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FullName, string PhoneNumber);
public record AuthResponse(string Token, string FullName, string Email);