   public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuantityAvalaible { get; set; }
        public string? ImageUrl { get; set; }  // New property for the image URL
        public string QRcode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CreatedByID { get; set; }
        public int? UpdatedByID { get; set; }
        public int? DeactivatedByID { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string? DeactivatedByName { get; set; }

}