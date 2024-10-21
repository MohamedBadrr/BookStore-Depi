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
    public class OrderHeaderRepository : GenericRepository<OrderHeader> , IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        public OrderHeaderRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Update(OrderHeader orderHeader)
        {
            _dbcontext.OrderHeaders.Update(orderHeader);
        }

        public void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
        {
            var orderFromDB = _dbcontext.OrderHeaders.Find(id);
            if (orderFromDB != null)
            {
                orderFromDB.PaymentDate = DateTime.Now;
                orderFromDB.OrderStatus = OrderStatus;
                orderFromDB.PaymentStatus = PaymentStatus;
            }
        }
    }
}
