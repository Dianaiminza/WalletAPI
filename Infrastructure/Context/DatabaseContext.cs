using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class DatabaseContext : DbContext
    {
        public DbContextOptions<DatabaseContext> Options { get; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
              : base(options)
        {
           
            Options = options;
        }
    }
}
