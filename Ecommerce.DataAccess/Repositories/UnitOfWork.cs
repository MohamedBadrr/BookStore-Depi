using Ecommerce.DataAccess.Data;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public ICategoryRepository Category { get; set; }
        public IProductRepository Product { get; set; }
        public IShoppingCartRepository ShoppingCart { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Category = new CategoryRepository(dbContext);
            Product = new ProductRepository(dbContext);
            ShoppingCart = new ShoppingCartRepository(dbContext);
            OrderDetail = new OrderDetailRepository(dbContext);
            OrderHeader = new OrderHeaderRepository(dbContext);
            ApplicationUser = new ApplicationUserRepository(dbContext);
        }

        public int Complete()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
