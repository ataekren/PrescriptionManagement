namespace SharedKernel.Models.Auth;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Doctor" or "Pharmacy"
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
} 