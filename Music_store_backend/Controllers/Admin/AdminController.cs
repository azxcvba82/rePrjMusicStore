using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Newtonsoft.Json.Linq;

namespace Music_store_backend.Controllers.Admin
{
    //[Authorize(Policy = "WebAdmin")]
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : MBController
    {
        public AdminController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("IndexSubI")]
        [HttpGet()]
        public ActionResult<Array> IndexSubI()
        {
            try
            {
                Array tA = CgetAlbums.getAlbumsByStatus(Util, 1).OrderBy(m => m.fYear).Take(5).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("IndexSubII")]
        [HttpGet()]
        public ActionResult<Array> IndexSubII()
        {
            try
            {
                DateTime now = DateTime.Now;
                Array tA = CgetActivity.getActivitiesWithInPeriod(Util, now).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("IndexSubIII")]
        [HttpGet()]
        public ActionResult<Array> IndexSubIII()
        {
            try
            {
                DateTime later = DateTime.Now.AddDays(7);
                Array tA = CgetActivity.getActivitiesWithInPeriod(Util, later).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("TypeAlter")]
        [HttpGet()]
        public ActionResult<Array> TypeAlter()
        {
            try
            {
                Array tA = CSearch.takeAllType(Util).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("TypeNew/{typeName}")]
        [HttpPut()]
        public ActionResult<JObject> TypeNew([FromRoute]string typeName)
        {
            try
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                SQLExecResult result = CManagement.typeNew(Util, typeName);
                SuccessInfo success = new SuccessInfo(result.message, user, Util);
                return JObject.FromObject(success);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("TypeDelete/{deleteId}")]
        [HttpDelete()]
        public ActionResult<JObject> TypeDelete([FromRoute]int deleteId)
        {
            try
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                SQLExecResult result = CManagement.typeDelete(Util, deleteId);
                SuccessInfo success = new SuccessInfo(result.message, user, Util);
                return JObject.FromObject(success);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("TypeEdit/{typeOrigin}/{typeChange}")]
        [HttpGet()]
        public ActionResult<JObject> TypeEdit([FromRoute]int typeOrigin, [FromRoute] string typeChange)
        {
            try
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                SQLExecResult result = CManagement.typeEdit(Util, typeOrigin, typeChange);
                SuccessInfo success = new SuccessInfo(result.message, user, Util);
                return JObject.FromObject(success);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}