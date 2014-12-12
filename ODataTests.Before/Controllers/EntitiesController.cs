using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ODataTests.Models;

namespace ODataTests.Controllers {
    public class EntitiesController : EntityControllerBase<Entity, Int32> {

        //In production you would probably want to inject the context using IOC
        public EntitiesController()
            : base(new DerivedContext()) {
        }
        
        protected override Expression<Func<Entity, bool>> GetKeyExpression(int keyValue) {
            return entity => entity.Id == keyValue;
        }
    }
}
