namespace UserService.Models;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
}
