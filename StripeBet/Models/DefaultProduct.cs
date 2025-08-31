namespace StripeBet.Models
{
    public class DefaultProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DefaultProduct(string id , string name, decimal price)
        {
            this.Id = id;
            this.Name = name;
            this.Price = price;
        }
    }
}
