namespace Application.Products.Exceptions;
public sealed class ProductNotFoundException : ApplicationException
{
    public ProductNotFoundException() : base("Product not found") { }
}
