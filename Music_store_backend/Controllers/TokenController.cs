using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Music_store_backend.Util;

namespace Music_store_backend.Controllers
{

//    [ApiController]
    public class TokenController : MBController
    {
        public TokenController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("/auth")]
        [HttpPost]
        public ActionResult<String> Login([FromBody] UserLoginPost userLogin)
        {
            try
            {

                if (String.IsNullOrEmpty(userLogin.userId))
                {
                    throw new Exception("user ID cannot be empty!");
                }
                if (String.IsNullOrEmpty(userLogin.password))
                {
                    throw new Exception("password cannot be empty!");
                }

                String message = "";

                if (Util.IsValidUserAndPassword(userLogin.userId, userLogin.password, out message) == false)
                {


                    throw new Exception(String.Format("{0}", message));
                }

                String cookieKey = "RefreshToken";
                String cookieValue = AccessTokenUtil.GenerateRefreshToken(Util, userLogin.userId);
                Microsoft.AspNetCore.Http.CookieOptions cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions();
                cookieOptions.HttpOnly = true;
                cookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                cookieOptions.Domain = Request.Host.Host;
                cookieOptions.Path = "/";
                Response.Cookies.Append(cookieKey, cookieValue, cookieOptions);
                return AccessTokenUtil.GenerateAccessToken(Util, userLogin.userId);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}