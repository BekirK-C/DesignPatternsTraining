using MongoDB.Driver;
using System.Collections;
using WebApp.Strategy.Models;

namespace WebApp.Strategy.Repositories
{
    public class ProductRepositoryFromMongoDb : IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductRepositoryFromMongoDb(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("ProductDb");
            _productCollection = database.GetCollection<Product>("Products");
        }

        public async Task Delete(Product product)
        {
            await _productCollection.DeleteOneAsync(p => p.Id == product.Id);
        }

        public async Task<List<Product>> GetAllByUserId(string userId)
        {
            return await _productCollection.Find(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Product> GetById(string id)
        {
            return await _productCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> Save(Product product)
        {
            await _productCollection.InsertOneAsync(product);
            return product;
        }

        public async Task Update(Product product)
        {
            await _productCollection.FindOneAndReplaceAsync(p => p.Id == product.Id, product);
        }
    }
}
