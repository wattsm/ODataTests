using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public class BasicSet<T> : IBasicSet<T> where T : class {

        private readonly IDbSet<T> _wrapped;

        public BasicSet(IDbSet<T> wrapped) {
            _wrapped = wrapped;
        }

        #region IBasicSet<T> Members

        public T Add(T entity) { return _wrapped.Add(entity); }
        public T Find(params object[] keyValues) { return _wrapped.Find(keyValues); }
        public T Remove(T entity) { return _wrapped.Remove(entity); }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator() { return _wrapped.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_wrapped).GetEnumerator(); }

        #endregion

        #region IQueryable Members

        public Type ElementType { get { return _wrapped.ElementType; } }
        public Expression Expression { get { return _wrapped.Expression; } }
        public IQueryProvider Provider { get { return _wrapped.Provider; } }

        #endregion
    }
}
