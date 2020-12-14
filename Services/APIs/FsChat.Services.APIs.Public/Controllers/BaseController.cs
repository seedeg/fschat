using FsChat.Interfaces;
using FsChat.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace FsChat.Services.APIs.Public.Controllers
{
    public class BaseController : ApiController
    {
        protected readonly ILogProvider logProvider;

        public BaseController(ILogProvider logProvider)
        {
            this.logProvider = logProvider ?? throw new ArgumentNullException(nameof(logProvider));
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            try
            {
                //TODO any overrides here
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
            }

            base.Initialize(controllerContext);
        }
    }
}
