#pragma warning disable 618

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Batch;
using System.Web.Http.OData.Builder;
using ODataTests.Models;

namespace ODataTests {
    public static class OData {

        public static void Configure(HttpConfiguration config) {

            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Entity>("Entities");

            config.MapHttpAttributeRoutes();

            //The method below is marked as obsolete, but if you're using the WebAPI OData v1-3 libraries
            //the suggested alternative is not available to you. The pragma at the top of this file
            //suppresses the warning.
            config.Routes.MapODataRoute(
                routeName: "odata",
                routePrefix: null,
                model: builder.GetEdmModel(),
                batchHandler: new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer)
            );
        }

    }
}