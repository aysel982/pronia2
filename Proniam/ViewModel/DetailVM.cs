using Proniam.Models;

namespace Proniam.ViewModel
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
