using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Music_store_backend.Util;
using Newtonsoft.Json;

namespace Music_store_backend.Controllers
{
    [Route("api/auth")]
    [Produces("application/json")]
    [ApiController]
    public class AuthController : MBController
    {
        public AuthController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("autoLogin/{encUserToken}")]
        [HttpGet]

        public ActionResult<CurrentUser> AutoLogin(String encUserToken)
        {
            try
            {
                EncUserToken encUser = null;

                String encUserJson = Util.Decrypt(encUserToken);
                encUser = JsonConvert.DeserializeObject<EncUserToken>(encUserJson);
                String userId = encUser.userId;

                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), userId);
                UserProfile userProfile = new UserProfile(Util.GetSqlConnectionMBMan(), user);
                CurrentUser currentUser = new CurrentUser
                {
                    accessToken = AccessTokenUtil.GenerateAccessToken(Util, userId),
                    userProfile = userProfile,
                    //userMenu = UserMenu.GetUserMenu(Util, user, HttpContext),
                };
                //AuditLog.auditUserAutoLogin(Util, Request, user);
                return currentUser;


            }
            catch (Exception ex)
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), "aaa");
                ExceptionInfo exInfo = new ExceptionInfo(ex, user, Util);
                return BadRequest(exInfo);
            }
        }

        [Route("login")]
        [HttpPost]

        public ActionResult<CurrentUser> Login([FromBody] UserLoginPost userLogin)
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

                AddRefreshTokenCookie(AccessTokenUtil.GenerateRefreshToken(Util, userLogin.userId), Response);
                SqlConnection myConnection = Util.GetSqlConnectionMBMan();
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), userLogin.userId);
                UserProfile userProfile = new UserProfile(myConnection, user);
                CurrentUser currentUser = new CurrentUser
                {
                    accessToken = AccessTokenUtil.GenerateAccessToken(Util, userLogin.userId),
                    userProfile = userProfile,
                    userMenu = UserMenu.GetUserMenu(Util, user, HttpContext),
                };
                //AuditLog.auditUserLogin(Util, Request, user);
                return currentUser;

            }
            catch (Exception ex)
            {

                //return BadRequest(new { message = ex.Message });
                return StatusCode((int)System.Net.HttpStatusCode.Unauthorized, new { message = ex.Message });

            }
        }

        private bool AddRefreshTokenCookie(String refreshToken, HttpResponse httpResponse)
        {
            bool bOK = true;
            String cookieKey = "RefreshToken";
            String cookieValue = refreshToken;
            Microsoft.AspNetCore.Http.CookieOptions cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions();
            cookieOptions.HttpOnly = true;
            cookieOptions.Secure = true;
            cookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
            cookieOptions.Domain = Request.Host.Host;
            cookieOptions.Path = "/";
            Response.Cookies.Append(cookieKey, cookieValue, cookieOptions);
            return bOK;
        }
    }
}