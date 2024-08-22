
    public class Employee
    {
        public int ID { get; set; }
        public int UserID { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Role { get; set; } = "Employee";
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public int? ApprovedByHeadAdminID { get; set; }
        public string? ApprovedByName { get; set; }
        public int? ActivatedByHeadAdminID { get; set; }
        public string? ActivatedByName { get; set; }
        public int? DeactivatedByHeadAdminID { get; set; }
        public string? DeactivatedByName { get; set; }
    }

