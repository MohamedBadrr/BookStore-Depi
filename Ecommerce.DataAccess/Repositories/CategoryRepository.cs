using Ecommerce.DataAccess.Data;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category> , ICategoryRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public CategoryRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void Update(Category category)
        {
            var updatedCategory = _dbcontext.Categories.Find(category.Id);
            if (updatedCategory != null)
            {
                updatedCategory.Name = category.Name;
                updatedCategory.Description = category.Description;
                updatedCategory.CreatedTime = category.CreatedTime;
            }
        }
    }
}
