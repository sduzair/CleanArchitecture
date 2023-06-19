using Application.Common.Security.Policies;
using Application.Products.Commands;
using Application.Products.Queries;

using Domain.Products.ValueObjects;

using FluentResults.Extensions;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;

namespace Presentation.Products;

public sealed class ProductController : ApiControllerBase
{
    private readonly CustomAspNetCoreResultEndpointProfile _resultProfile;

    public ProductController(CustomAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpPost]
    [Authorize(Policy = nameof(ProductManagementPolicy))]
    public async Task<IActionResult> CreateProduct(CreateProductCommand productDto)
    {
        var result = await Mediator.Send(productDto);
        if (result.IsFailed)
        {
            return result.ToActionResult(_resultProfile);
        }
        return CreatedAtAction(nameof(GetProductById), new { id = result.Value.ToString() }, result.Value); 
    }

    [HttpGet]
    [Authorize(Policy = nameof(ProductViewPolicy))]
    public async Task<IActionResult> GetProducts()
    {
        return await Mediator.Send(new GetProductsQuery())
            .Map((products) => products.Select(product => product.MapTo()))
            .ToActionResult(_resultProfile);
    }

    [HttpGet]
    [Authorize(Policy = nameof(ProductViewPolicy))]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        return await Mediator.Send(new GetProductByIdQuery(ProductId.From(id)))
            .Map(product => product.MapTo())
            .ToActionResult(_resultProfile);
    }

    [HttpPut]
    [Authorize(Policy = nameof(ProductManagementPolicy))]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductDto productDto)
    {
        if (id != productDto.Id)
        {
            ModelState.AddModelError(nameof(productDto.Id), "Id in payload must match with the Id in Url parameter");
            return ValidationProblem();
        }

        return await Mediator.Send(productDto.MapToUpdateProductCommand())
            .ToActionResult(_resultProfile);
    }

    [HttpDelete]
    [Authorize(Policy = nameof(ProductAdminPolicy))]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        return await Mediator.Send(new DeleteProductCommand(ProductId.From(id)))
            .ToActionResult(_resultProfile);
    }
}
