using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Results;
using System.Web.Http.Results;
using Moq;
using ODataTests.Controllers;
using ODataTests.Models;
using Xunit;

namespace ODataTests.Tests.Controllers {
    public class EntitiesControllerFacts {

        #region Helpers

        private static class Helpers {

            public static Mock<IBasicSet<Entity>> CreateSet() {
                return Helpers.CreateSet(new Entity[0]);
            }

            public static Mock<IBasicSet<Entity>> CreateSet(IEnumerable<Entity> entities) {

                var queryable = entities.AsQueryable();

                var set = new Mock<IBasicSet<Entity>>();
                set.Setup(x => x.ElementType).Returns(queryable.ElementType);
                set.Setup(x => x.Expression).Returns(queryable.Expression);
                set.Setup(x => x.Provider).Returns(queryable.Provider);

                set.Setup(x => x.Find(It.IsAny<Object[]>())).Returns<Object[]>(
                    keyValues => {

                        var id = (Int32)keyValues[0];

                        return entities.FirstOrDefault(entity => entity.Id == id);
                    }
                );

                return set;
            }

            public static Mock<IBasicContext> CreateContext(IEnumerable<Entity> entities) {

                var set = Helpers.CreateSet(entities);

                return Helpers.CreateContext(set.Object);
            }

            public static Mock<IBasicContext> CreateContext() {
                return Helpers.CreateContext(new Entity[0]);
            }

            public static Mock<IBasicContext> CreateContext(IBasicSet<Entity> set) {

                var context = new Mock<IBasicContext>();
                context.Setup(x => x.GetSet<Entity>()).Returns(set);

                return context;
            }

            public static EntitiesController CreateController() {
                return Helpers.CreateController(new Entity[0]);
            }

            public static EntitiesController CreateController(IEnumerable<Entity> entities) {

                var set = Helpers.CreateSet(entities);

                return Helpers.CreateController(set.Object);
            }

            public static EntitiesController CreateController(IBasicSet<Entity> set) {

                var context = Helpers.CreateContext(set);

                return Helpers.CreateController(context.Object);
            }

            public static EntitiesController CreateController(IBasicContext context) {
                return new EntitiesController(context);
            }
        }

        #endregion

        #region _Type

        //Tests for the type in general
        public class _Type { 

            [Fact]
            public void context_is_disposed_when_controller_is_disposed() {

                var context = new Mock<IBasicContext>();
                var controller = Helpers.CreateController(context.Object);

                controller.Dispose();

                context.Verify(x => x.Dispose(), Times.Once());
            }

        }

        #endregion

        #region GetMethod_1

        //Tests for Get()
        public class GetMethod_1 {

            [Fact]
            public void returns_set() {

                var set = Mock.Of<IBasicSet<Entity>>();
                var context = Helpers.CreateContext(set);

                var controller = Helpers.CreateController(context.Object);

                var result = controller.Get();

                Assert.True(Object.ReferenceEquals(set, result)); //NOTE Assert.Equal(set, result) throws NRE, something in xUnit code does not like the mocks
            }

        }

        #endregion

        #region GetMethod_2

        //Tests for Get(Int32)
        public class GetMethod_2 {

            [Fact]
            public void returns_single_result() {

                var controller = Helpers.CreateController();
                var result = controller.Get(10);

                Assert.NotNull(result);
                Assert.IsType<SingleResult<Entity>>(result);
            }

            [Fact]
            public void returns_correct_entity() {

                var entities = new Entity[] {
                    new Entity() { Id = 1 },
                    new Entity() { Id = 2 }
                };

                var controller = Helpers.CreateController(entities);
                var result = controller.Get(2);

                Assert.Equal(1, result.Queryable.Count());
                Assert.Equal(2, result.Queryable.First().Id);
            }

        }

        #endregion

        #region PostMethod

        //Tests for Post(Entity)
        public class PostMethod {

            [Fact]
            public void invalid_model_returns_bad_request() {

                var controller = Helpers.CreateController();
                controller.ModelState.AddModelError(String.Empty, "Invalid");

                var result = controller.Post(new Entity());

                Assert.NotNull(result);
                Assert.IsType<InvalidModelStateResult>(result);
            }

            [Fact]
            public void invalid_model_returns_model_state() {

                var controller = Helpers.CreateController();
                controller.ModelState.AddModelError(String.Empty, "Invalid");

                var result = controller.Post(new Entity()) as InvalidModelStateResult;

                Assert.Equal(controller.ModelState, result.ModelState);
            }

            [Fact]
            public void entity_is_added_to_set() {

                var set = Helpers.CreateSet();
                var controller = Helpers.CreateController(set.Object);
                var entity = new Entity();

                var result = controller.Post(entity);

                set.Verify(x => x.Add(entity), Times.Once());
            }

            [Fact]
            public void context_is_saved() {

                var context = Helpers.CreateContext();
                var controller = Helpers.CreateController(context.Object);

                var result = controller.Post(new Entity());

                context.Verify(x => x.SaveChanges(), Times.Once());
            }

            [Fact]
            public void returns_created() {

                var controller = Helpers.CreateController();

                var result = controller.Post(new Entity());

                Assert.NotNull(result);
                Assert.IsType<CreatedODataResult<Entity>>(result);
            }

            [Fact]
            public void returns_entity() {

                var controller = Helpers.CreateController();
                var entity = new Entity();

                var result = controller.Post(entity) as CreatedODataResult<Entity>;

                Assert.Equal(entity, result.Entity);
            }

        }

        #endregion

        #region PatchMethod

        //Tests for Patch(Int32, Delta<Entity>)
        public class PatchMethod {

            private readonly Entity[] _entities = new Entity[] {
                new Entity() { Id = 1 , Name = "Entity" }
            };

            [Fact]
            public void invalid_model_returns_bad_request() {

                var controller = Helpers.CreateController(_entities);
                controller.ModelState.AddModelError(String.Empty, "Invalid");

                var result = controller.Patch(1, new Delta<Entity>());

                Assert.NotNull(result);
                Assert.IsType<InvalidModelStateResult>(result);
            }

            [Fact]
            public void invald_model_returns_model_state() {

                var controller = Helpers.CreateController(_entities);
                controller.ModelState.AddModelError(String.Empty, "Invalid");

                var result = controller.Patch(1, new Delta<Entity>()) as InvalidModelStateResult;

                Assert.Equal(controller.ModelState, result.ModelState);
            }

            [Fact]
            public void invalid_key_returns_not_found() {

                var controller = Helpers.CreateController(_entities);

                var result = controller.Patch(2, new Delta<Entity>());

                Assert.NotNull(result);
                Assert.IsType<NotFoundResult>(result);
            }

            [Fact]
            public void entity_is_patched() {

                var controller = Helpers.CreateController(_entities);

                dynamic delta = new Delta<Entity>();
                delta.Name = "Updated";

                var result = controller.Patch(1, delta);

                Assert.Equal("Updated", _entities.First().Name);
            }

            [Fact]
            public void context_is_saved() {

                var context = Helpers.CreateContext(_entities);
                var controller = Helpers.CreateController(context.Object);

                var result = controller.Patch(1, new Delta<Entity>());

                context.Verify(x => x.SaveChanges(), Times.Once());
            }

            [Fact]
            public void concurrency_exception_returns_not_found_if_entity_no_longer_exists() {

                var entities = new List<Entity>(_entities);

                var context = Helpers.CreateContext(entities);

                context.Setup(x => x.SaveChanges()).Callback(() => {

                    //Simulate another process deleting the entity by removing it
                    //from the list used as the basis for the DbSet.
                    entities.Clear();

                    throw new DbUpdateConcurrencyException();
                });

                var controller = Helpers.CreateController(context.Object);

                var result = controller.Patch(1, new Delta<Entity>());

                Assert.NotNull(result);
                Assert.IsType<NotFoundResult>(result);
            }

            [Fact]
            public void concurrency_exception_is_rethrown_if_entity_exists() {
                Assert.Throws<DbUpdateConcurrencyException>(() => {

                    var context = Helpers.CreateContext(_entities);
                    context.Setup(x => x.SaveChanges()).Throws<DbUpdateConcurrencyException>();

                    var controller = Helpers.CreateController(context.Object);
                    var result = controller.Patch(1, new Delta<Entity>());
                });
            }

            [Fact]
            public void returns_updated() {

                var controller = Helpers.CreateController(_entities);

                var result = controller.Patch(1, new Delta<Entity>());

                Assert.NotNull(result);
                Assert.IsType<UpdatedODataResult<Entity>>(result);
            }

            [Fact]
            public void returns_entity() {

                var controller = Helpers.CreateController(_entities);

                var result = controller.Patch(1, new Delta<Entity>()) as UpdatedODataResult<Entity>;

                Assert.Equal(_entities.First(), result.Entity);
            }

        }

        #endregion

        #region DeleteMethod

        //Tests Delete(Int32)
        public class DeleteMethod {

            private readonly Entity[] _entities = new Entity[] {
                new Entity() { Id = 1 , Name = "Entity" }
            };

            [Fact]
            public void invalid_key_returns_not_found() {

                var controller = Helpers.CreateController(_entities);

                var result = controller.Delete(2);

                Assert.NotNull(result);
                Assert.IsType<NotFoundResult>(result);
            }

            [Fact]
            public void entity_is_removed() {

                var set = Helpers.CreateSet(_entities);
                var controller = Helpers.CreateController(set.Object);
                var entity = _entities.First();

                var result = controller.Delete(1);

                set.Verify(x => x.Remove(entity), Times.Once());
            }

            [Fact]
            public void context_is_saved() {

                var context = Helpers.CreateContext(_entities);
                var controller = Helpers.CreateController(context.Object);

                var result = controller.Delete(1);

                context.Verify(x => x.SaveChanges(), Times.Once());
            }

            [Fact]
            public void returns_ok() {

                var controller = Helpers.CreateController(_entities);

                var result = controller.Delete(1);

                Assert.NotNull(result);
                Assert.IsType<OkResult>(result);
            }

        }

        #endregion
    }
}
