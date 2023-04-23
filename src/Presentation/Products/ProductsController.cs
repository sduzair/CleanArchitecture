using Application.Products;

using Domain.Products.Entities;
using Domain.Products.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Contracts.Products;

namespace Presentation.Products;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;
    public ProductsController(IProductsService productsService)
    {
        _productsService = productsService;
    }
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productsService.GetProducts();
        return Ok(products);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto productDto)
    {
        var (result, id) = await _productsService.CreateProduct(productDto.Name, productDto.Description, productDto.UnitPrice);
        if (result == 0)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(GetProduct), new { id = id.ToString() }, id); 
    }
    [HttpGet]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productsService.GetProduct(ProductId.Create(id));
        if (product is null)
        {
            return NotFound();
        }
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
    {
        var result = await _productsService.UpdateProduct(updateProductDto.Id, updateProductDto.Name, updateProductDto.Description, updateProductDto.UnitPrice);
        if (result == 0)
        {
            return BadRequest();
        }
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteProduct(ProductId id)
    {
        var result = await _productsService.DeleteProduct(id);
        if (result == 0)
        {
            return BadRequest();
        }
        return Ok();
    }
}
