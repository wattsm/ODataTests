using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using ODataTests.Models;

namespace ODataTests.Controllers {
    public abstract class EntityControllerBase<TEntity, TKey> : ODataController where TEntity : class {

        private readonly IBasicContext _context;
        private readonly IBasicSet<TEntity> _set;

        public EntityControllerBase(IBasicContext context) {
            _context = context;
            _set = context.GetSet<TEntity>();
        }

        #region OData Actions

        [EnableQuery]
        public IQueryable<TEntity> Get() {
            return _set;
        }

        [EnableQuery]
        public SingleResult<TEntity> Get([FromODataUri] TKey key) {
            return SingleResult.Create<TEntity>(
                _set.Where(this.GetKeyExpression(key))
            );
        }

        public IHttpActionResult Post(TEntity entity) {

            IHttpActionResult result;

            if(!this.ModelState.IsValid) {
                result = this.BadRequest(this.ModelState);
            } else {

                _set.Add(entity);
                _context.SaveChanges();

                result = this.Created(entity);
            }

            return result;
        }

        [AcceptVerbs("MERGE", "PATCH")]
        public IHttpActionResult Patch([FromODataUri] TKey key, Delta<TEntity> changes) {

            IHttpActionResult result;

            if(!this.ModelState.IsValid) {
                result = this.BadRequest(this.ModelState);
            } else {

                var entity = _set.Find(key);

                if(entity == null) {
                    result = this.NotFound();
                } else {
                    try {

                        changes.Patch(entity);

                        _context.SaveChanges();

                        result = this.Updated(entity);

                    } catch(DbUpdateConcurrencyException) {
                        if(!this.Exists(key)) {
                            result = this.NotFound(); //Entity was deleted before removed
                        } else {
                            throw;
                        }
                    }
                }
            }

            return result;
        }

        public IHttpActionResult Delete([FromODataUri] TKey key) {

            IHttpActionResult result;
            var entity = _set.Find(key);

            if(entity == null) {
                result = this.NotFound();
            } else {

                _set.Remove(entity);
                _context.SaveChanges();

                result = this.Ok();
            }

            return result;
        }

        #endregion

        protected virtual bool Exists(TKey key) {
            return _set.Any(this.GetKeyExpression(key));
        }

        protected abstract Expression<Func<TEntity, bool>> GetKeyExpression(TKey keyValue);

        protected override void Dispose(bool disposing) {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
