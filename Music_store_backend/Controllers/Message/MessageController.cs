using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;

namespace Music_store_backend.Controllers.Message
{
    [Authorize(Policy = "NormalUser")]
    [Route("api/Message")]
    [ApiController]
    public class MessageController : MBController
    {
        public MessageController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("MessageBox")]
        [HttpGet()]
        public ActionResult<int> MessageBox()
        {
            try
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                int tME = CMessage.getReceiveMessage(Util, user.UserID).Count();
                int totalPage = tME % 5 == 0 ? (tME / 5) + 1 : (tME / 5) + 2;
                return totalPage;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("_MessageListView/{page}")]
        [HttpGet()]
        public ActionResult<List<tMessage>> _MessageListView([FromRoute]int page=1)
        {
            try
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                if (page != -1)
                {
                    List<tMessage> tm = CMessage.getReceiveMessagePage(Util, user.UserID).Skip(5 * (page - 1)).Take(5).ToList();
                    //foreach (var a in tm.Skip(5 * (page - 1)).Take(5))
                    //{
                    //    //未讀信第一次讀取
                    //    if (a.fReaded == 0 || a.fReaded == null)
                    //    {
                    //        a.fReaded = 1;
                    //    }
                    //    //已讀信第一次讀取
                    //    else if (a.fReaded == 1)
                    //    {
                    //        a.fReaded = 2;
                    //    }
                    //}
                    //db.SaveChanges();
                    
                    //List<tMessage> tm = CMessage.getReceiveMessage(Util, user.UserID);
                    return tm;
                }
                else
                {
                   List<tMessage> tm = CMessage.getSendMessageCopy(Util, user.UserID);
                    return tm;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}