using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;

namespace ODataTests {
    public class Global : HttpApplication {

        protected void Application_Start(Object source, EventArgs args) {
            GlobalConfiguration.Configure(OData.Configure);
        }

    }
}