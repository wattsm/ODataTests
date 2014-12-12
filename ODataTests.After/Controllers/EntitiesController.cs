using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ODataTests.Models;

namespace ODataTests.Controllers {
    public class EntitiesController : EntityControllerBase<Entity, Int32> {

        public EntitiesController()
            : base(new DerivedContext()) {
        }

        //This is the constructor you would use in production, injecting context via IOC.
        public EntitiesController(IBasicContext context)
            : base(context) {
        }
        
        protected override Expression<Func<Entity, bool>> GetKeyExpression(int keyValue) {
            return entity => entity.Id == keyValue;
        }
    }
}
