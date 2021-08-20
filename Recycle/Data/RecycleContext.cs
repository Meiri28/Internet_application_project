using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recycle.Models;

namespace Recycle.Data
{
    public class RecycleContext : DbContext
    {
        public RecycleContext (DbContextOptions<RecycleContext> options)
            : base(options)
        {
        }

        public DbSet<Recycle.Models.User> User { get; set; }

        public DbSet<Recycle.Models.UserGender> UserGender { get; set; }

        public DbSet<Recycle.Models.Store> Store { get; set; }

        public DbSet<Recycle.Models.StoreComment> StoreComment { get; set; }

        public DbSet<Recycle.Models.ProductsComment> ProductsComment { get; set; }
    }
}
