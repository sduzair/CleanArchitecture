﻿using Application.Products.Commands;
using Application.Products.Queries;

using Domain.Products.ValueObjects;

using FluentResults.Extensions;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Utility;

namespace Presentation.Products;

[Authorize]
public sealed class ProductsController : ApiControllerBase
{
    private readonly ApplicationAspNetCoreResultEndpointProfile _resultProfile;

    public ProductsController(ApplicationAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpPost]
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
    public async Task<IActionResult> GetProducts()
    {
        return await Mediator.Send(new GetProductsQuery())
            .Map((products) => products.Select(ProductDto.MapFrom))
            .ToActionResult(_resultProfile);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        return await Mediator.Send(new GetProductByIdQuery(ProductId.Create(id)))
            .Map(ProductDto.MapFrom)
            .ToActionResult(_resultProfile);
    }

    [HttpPut]
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
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        return await Mediator.Send(new DeleteProductCommand(ProductId.Create(id)))
            .ToActionResult(_resultProfile);
    }
}
