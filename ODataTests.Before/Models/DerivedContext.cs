using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public class DerivedContext : DbContext {

        public DerivedContext()
            : base("name=DerivedContext") {
        }

        public DbSet<Entity> Entities { get; set; }
    }
}
