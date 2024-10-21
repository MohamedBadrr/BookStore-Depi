using Ecommerce.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        ICategoryRepository Category { get; set; }
        IProductRepository Product { get; set; }
        IShoppingCartRepository ShoppingCart { get; set; }
        IOrderHeaderRepository OrderHeader { get; set; }
        IOrderDetailRepository OrderDetail { get; set; }
        IApplicationUserRepository ApplicationUser { get; set; }
        int Complete(); 
    }
}
