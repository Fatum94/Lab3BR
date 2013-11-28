using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Registration.Models
{
    public class Database : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Kompressor> Compressor { get; set; }
    }

    public class DbInit : DropCreateDatabaseIfModelChanges<Database>
    {
        protected override void Seed(Database context)
        {
            base.Seed(context);
        }
    }
}