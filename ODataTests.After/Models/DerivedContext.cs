using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public class DerivedContext : DbContext, IBasicContext {

        public DerivedContext()
            : base("name=DerivedContext") {
        }

        public DbSet<Entity> Entities { get; set; }

        #region IBasicContext Members

        public IBasicSet<T> GetSet<T>() where T : class {
            return this.Set<T>().Wrap();
        }

        #endregion
    }
}
