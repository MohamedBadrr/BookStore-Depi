using Ecommerce.Entities.Models;
using Ecommerce.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Interfaces
{
    public interface IApplicationUserRepository:IGenericRepository<ApplicationUser>
    {
    }
}
