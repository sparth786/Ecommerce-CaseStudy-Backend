namespace EcommerceApp.Models
{
    public class AddToCartVM
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
    }

    public class CartData
    {
        public int CartId { get; set; }
        public string ProductName { get; set; }
        public string UserName { get; set; }
        public string ImageURL { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
    }
}
