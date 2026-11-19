using High_Traffic_E_Commerce_Inventory_Order_System.Dto_s.ProductDto_s;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace High_Traffic_E_Commerce_Inventory_Order_System.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController:ControllerBase
    {
        private readonly IProductServices productServices;
        public ProductController(IProductServices productServices)
        {
            this.productServices = productServices;
        }



        [HttpGet]
        public async Task<IActionResult>GetAllProducts(CancellationToken cancellationToken)
        {
            var products= await productServices.GetAllProducts(cancellationToken);
            return Ok(products);
        }



        [HttpGet("id/{productId:int}")]
        public async Task<IActionResult> SearchProduct(int productId,CancellationToken cancellationToken)
        {
            var product = await productServices.SearchProduct(productId, cancellationToken);
            return Ok(product);
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct product, CancellationToken cancellationToken)
        {
            await productServices.CreateProduct(product, cancellationToken);
           
           
            return CreatedAtAction(nameof(SearchProductBySKU), new { SKU = product.SKU }, product);
        }



        [HttpPut("id/{productId:int}")]
        public async Task<IActionResult>UpdateProduct(int productId,[FromBody]UpdateProduct product, CancellationToken cancellationToken)
        {
            await productServices.UpdateProduct(productId, product, cancellationToken);
            return Ok(product);
        }



        [HttpGet("sku/{SKU}")]
        public async Task <IActionResult>SearchProductBySKU(string SKU,CancellationToken cancellationToken)
        {
            var product= await productServices.SearchProductBySKU(SKU, cancellationToken);

            return Ok(product);

        }

    }
}
