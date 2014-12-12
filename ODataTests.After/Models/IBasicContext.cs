using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public interface IBasicContext : IDisposable {
        int SaveChanges();
        IBasicSet<T> GetSet<T>() where T : class;
    }
}
