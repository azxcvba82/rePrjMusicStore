using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Music_store_backend.Controllers
{
    [Produces("application/json")]
    public class MBController : ControllerBase
    {
        protected readonly AppSettings Util;
        public MBController(IOptions<AppSettings> appSettings)
        {
            Util = appSettings.Value;
        }
    }
}