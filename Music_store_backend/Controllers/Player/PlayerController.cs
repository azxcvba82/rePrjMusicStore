using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Newtonsoft.Json.Linq;

namespace Music_store_backend.Controllers.Player
{
    [Route("api/Main")]
    [ApiController]
    public class PlayerController : MBController
    {
        public PlayerController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("DemoPlayer/{musicId}")]
        [HttpGet()]
        public ActionResult<JObject> DemoPlayer([FromRoute]int musicId)
        {
            try
            {
                return JObject.FromObject(CMusic.getMusic(Util, musicId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}