namespace EcommerceApp.Models
{
    public class ProductMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int CategoryId { get; set; }
        public virtual CategoryMaster Category { get; set; }
    }
}
