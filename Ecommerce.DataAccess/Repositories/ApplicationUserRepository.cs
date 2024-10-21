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
    public class ApplicationUserRepository : GenericRepository<ApplicationUser> , IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
        }
    }
}
