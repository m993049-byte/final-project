using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using final_project.Models;

namespace final_project.Data
{
    public class final_projectContext : DbContext
    {
        public final_projectContext (DbContextOptions<final_projectContext> options)
            : base(options)
        {
        }

        public DbSet<final_project.Models.orders> orders { get; set; } = default!;
    }
}
