﻿
using Ecommerce.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        void Update(Product product);
        public IQueryable<Product> SearchByName(string name);

    }
}
