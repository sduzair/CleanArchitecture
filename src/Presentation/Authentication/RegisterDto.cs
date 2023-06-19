namespace Presentation.Authentication;
public record RegisterDto(
    string Email,
    string Password,
    string ConfirmPassword,
    string RoleName);
//string? FirstName,
//string? LastName,
//string? PhoneNumber,
//string? AddressLine1,
//string? AddressLine2,
//string? City,
//string? State,
//string? ZipCode,
//string? Country);


