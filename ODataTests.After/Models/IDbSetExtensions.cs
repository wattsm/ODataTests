using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataTests.Models {
    public static class IDbSetExtensions {

        public static IBasicSet<T> Wrap<T>(this IDbSet<T> set) where T : class {
            return new BasicSet<T>(set);
        }

    }
}
