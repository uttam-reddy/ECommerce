using AutoMapper;
using ECommerce.API.Products.Db;
using ECommerce.API.Products.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Products.Providers
{
    public class ProductsProvider : IProductsProvider
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductsProvider> _logger;
        private readonly IMapper _mapper;

        public ProductsProvider(ProductDbContext productDbContext,ILogger<ProductsProvider> logger,IMapper mapper)
        {
                this._context = productDbContext;
                this._logger = logger;
                this._mapper = mapper;

                SeedData();
        }

        private void SeedData()
        {
            if(!_context.Products.Any())
            {
                _context.Products.Add(new Product() { Id = 1, Name = "Keyboard", Price = 20, Inventory = 100 });
                _context.Products.Add(new Product() { Id = 2, Name = "Mouse", Price = 5, Inventory = 200 });
                _context.Products.Add(new Product() { Id = 3, Name = "Monitor", Price = 150, Inventory = 100 });
                _context.Products.Add(new Product() { Id = 4, Name = "CPU", Price = 200, Inventory = 2000 });
                _context.SaveChanges();
            }
            
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Product> Products, string ErrorMessage)> GetProductsAsync()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                if(products != null && products.Any())
                {
                    var result = _mapper.Map<IEnumerable<Db.Product>, IEnumerable<Models.Product>>(products);
                    return (true, result, null);
                }
                else return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);

                
            }
        }

        public async Task<(bool IsSuccess, Models.Product Product, string ErrorMessage)> GetProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product != null )
                {
                    var result = _mapper.Map<Db.Product, Models.Product>(product);
                    return (true, result, null);
                }
                else return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);


            }
        }
    }
}
