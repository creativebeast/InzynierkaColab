namespace Inzynierka.Models.ViewModels
{
    public class ProductsContainer
    {
        public List<Product> products { get; set; }

        public ProductsContainer()
        {
            products = new List<Product>();
            products.Add(new Product());
        }
    }
}
