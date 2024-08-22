#pragma warning disable CA1050 // Declare types in namespaces
public class UserRegistration
{
    public int ID { get; set; } // Primary key
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Role { get; set; } // Nullable for now
    public string Status { get; set; } = "Pending"; // Default value
    public DateTime? RejectedAt { get; set; } = DateTime.Now;
    public int? RejectedByHeadAdminID { get; set; }
    public string? RejectedByName { get; set; }

}
