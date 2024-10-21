using Ecommerce.DataAccess.Data;
using Ecommerce.Entities.Interfaces;
using Ecommerce.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repositories
{
    internal class OrderDetailRepository : GenericRepository<OrderDetail> , IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        public OrderDetailRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Update(OrderDetail orderDetail)
        {
             _dbcontext.OrderDetails.Update(orderDetail); 
        }
    }
}
