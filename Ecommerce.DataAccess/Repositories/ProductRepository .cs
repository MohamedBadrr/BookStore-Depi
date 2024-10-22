using Ecommerce.DataAccess.Data;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public ProductRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Update(Product product)
        {
            var updatedProcuct = _dbcontext.Products.Find(product.Id);
            if (updatedProcuct != null)
            {
                updatedProcuct.Name = product.Name;
                updatedProcuct.Description = product.Description;
                updatedProcuct.Price = product.Price;
                updatedProcuct.Img = product.Img;
                updatedProcuct.CategoryId = product.CategoryId;
            }
        }
        public IQueryable<Product> SearchByName(string name)
        {
            return _dbcontext.Products.Where(p => p.Name.ToLower().Contains(name));
        }
    }
}
