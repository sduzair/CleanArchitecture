using Application.Products.Commands;
using Application.Products.Errors;
using Application.Products.Queries;

using Domain.Products.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Products;

[Authorize]
public sealed class ProductsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductCommand productDto)
    {
        var result = await Mediator.Send(productDto);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        return CreatedAtAction(nameof(GetProduct), new { id = result.Value.ToString() }, result.Value); 
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var result = await Mediator.Send(new GetProductsQuery());
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await Mediator.Send(new GetProductByIdQuery(ProductId.Create(id)));
        if (result.HasError<ProductNotFoundError>())
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductCommand updateProductCommand)
    {
        if (id != updateProductCommand.Id.Value)
        {
            return BadRequest();
        }
        var result = await Mediator.Send(updateProductCommand);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProduct(ProductId id)
    {
        var result = await Mediator.Send(new DeleteProductCommand(ProductId.Create(id.Value)));
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        return NoContent();
    }
}
