public class UpdateProductDto
{
    public int ID { get; set; }  // Include ID to identify the product
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int QuantityAvalaible { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
