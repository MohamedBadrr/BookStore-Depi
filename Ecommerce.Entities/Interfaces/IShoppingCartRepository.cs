using Ecommerce.Entities.Repositories;
using Ecommerce.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Interfaces
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncreaseCount (ShoppingCart shoppingCart,int count);
        int DecreaseCount (ShoppingCart shoppingCart,int count);
        int CountItems ();
    }
}
