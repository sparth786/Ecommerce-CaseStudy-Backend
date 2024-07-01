namespace EcommerceApp.Models
{
    public class CartDetails
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string Image { get; set; }
        //public virtual ProductMaster ProductMaster { get; set; }
        //public virtual UserMaster UserMaster{ get; set; }
    }
}
