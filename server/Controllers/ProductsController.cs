using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ProductsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _appDbContext.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            product.Id = Guid.NewGuid();
            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();
            return Ok(product);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(p=> p.Id == id);
            if(product == null)
                return NotFound();  
            return Ok(product);
        }
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateProdcut([FromRoute] Guid id,Product updateProdcutRequest)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if(product == null)
                return NotFound();
            product.Name = updateProdcutRequest.Name;
            product.Type = updateProdcutRequest.Type;
            product.Price = updateProdcutRequest.Price;
            await _appDbContext.SaveChangesAsync();
            return this.Ok(product);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if(product==null)
                return NotFound();
            _appDbContext.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return this.Ok(product);
        }
    }
}
