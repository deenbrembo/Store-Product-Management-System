    public class Borrowing
    {
        public int ID { get; set; }
        public int? EmployeeID { get; set; } 
        public string? EmployeeName { get; set; } = string.Empty;
        public int? AdminID { get; set; } 
        public string? AdminName { get; set; } = string.Empty;
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int QuantityBorrow { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }  // Nullable if returned at can be null
        public string Status { get; set; } = string.Empty;
    }