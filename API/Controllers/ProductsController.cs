
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController(IGenericRepository<Product> repo, IProductRepository productRepository) : BaseApiController
    {
        //  Specification Pattern
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
        // {
        //     var spec = new ProductSpecification(specParams);

        //     return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
        // }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repo.GetByIdAsync(id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repo.Add(product);

            if (await repo.SaveAllAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id)) return BadRequest("Cannot update this product");

            repo.Update(product);

            if (await repo.SaveAllAsync())
            {
                return NoContent();
            };

            return BadRequest("Problem updating product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await repo.GetByIdAsync(id);

            if (product == null) return NotFound();

            repo.Remove(product);

            if (await repo.SaveAllAsync())
            {
                return NoContent();
            };

            return BadRequest("Problem deleting product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();

            return Ok(await repo.ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            
            return Ok(await repo.ListAsync(spec));
        }

        // Check if a product exists
        public bool ProductExists(int id)
        {
            return repo.Exists(id);
        }









        // Repository Pattern
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {
            var query = productRepository.GetProductsAsync(specParams);

            return Ok(await PagedList<Product>.CreateAsync(query, specParams.PageIndex, specParams.PageSize));
        }
        // [HttpGet("{id:int}")]
        // public async Task<ActionResult<Product>> GetProduct(int id)
        // {
        //     var product = await productRepository.GetProductByIdAsync(id);

        //     if (product == null) return NotFound();

        //     return product;
        // }
        // [HttpPost]
        // public async Task<ActionResult<Product>> CreateProduct(Product product)
        // {
        //     productRepository.AddProduct(product);

        //     if (await productRepository.SaveChangesAsync())
        //     {
        //         return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        //     }

        //     return BadRequest("Problem creating product");
        // }
        // [HttpPut("{id:int}")]
        // public async Task<ActionResult> UpdateProduct(int id, Product product)
        // {
        //     if (product.Id != id || !ProductExists(id)) return BadRequest("Cannot update this product");

        //     productRepository.UpdateProduct(product);

        //     if (await productRepository.SaveChangesAsync())
        //     {
        //         return NoContent();
        //     };

        //     return BadRequest("Problem updating product");
        // }
        // [HttpDelete("{id:int}")]
        // public async Task<ActionResult> DeleteProduct(int id)
        // {
        //     var product = await productRepository.GetProductByIdAsync(id);

        //     if (product == null) return NotFound();

        //     productRepository.DeleteProduct(product);

        //     if (await productRepository.SaveChangesAsync())
        //     {
        //         return NoContent();
        //     };

        //     return BadRequest("Problem deleting product");
        // }
        // [HttpGet("brands")]
        // public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        // {
        //     return Ok(await productRepository.GetBrandsAsync());
        // }
        // [HttpGet("types")]
        // public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        // {
        //     return Ok(await productRepository.GetTypesAsync());
        // }

        // // Check if a product exists
        // public bool ProductExists(int id)
        // {
        //     return productRepository.ProductExists(id);
        // }
    }
}