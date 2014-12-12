using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public interface IBasicSet<T> : IQueryable<T> where T : class {
        T Add(T entity);
        T Find(params object[] keyValues);
        T Remove(T entity);
    }
}
