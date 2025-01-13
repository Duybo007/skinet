using System;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces;

public interface IProductRepository
{
    IQueryable<Product> GetProductsQuery(ProductSpecParams specParams);
    Task<Product?> GetProductByIdAsync(int id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    bool ProductExists(int id);
    Task<bool> SaveChangesAsync();
}