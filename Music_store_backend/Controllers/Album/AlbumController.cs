using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace Music_store_backend.Controllers.Album
{

    [Route("api/Album")]
    [ApiController]
    public class AlbumController : MBController
    {
        private IWebHostEnvironment _he;
        public AlbumController(IOptions<AppSettings> appSettings, IWebHostEnvironment he) : base(appSettings) 
        {
            _he = he;
        }

        [Authorize(Policy = "NormalUser")]
        [Route("nextPlayLists/{amid}/{playmode}")]
        [HttpGet()]
        public ActionResult<JObject> nextPlayLists([FromRoute]int amid, [FromRoute]int playmode)
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);

            try
            {
                tProduct tprodSec = null;
                if (DateTime.Now < user.SubscriptEndDate)
                {
                    //Subscrption User
                    if(Util.getEnumPlaymode(playmode)!= "Random")
                        {
                        if (Util.getEnumPlaymode(playmode) != "LastSong")
                        {
                            List<tProduct> tPlCheck = CPlaylist.getUserPlaylistSubscribeNext(Util, user.UserID, amid);
                            if (tPlCheck == null)
                            {
                                throw new Exception(String.Format("BackEndMusic.BackEndMusicCallGet Error:{0}", "error id in poducts"));
                            }
                            else
                            {
                                tprodSec = tPlCheck.Skip(1).Take(1).FirstOrDefault();
                            }
                        }
                        else
                        {
                            List<tProduct> tPlCheck = CPlaylist.getUserPlaylistSubscribeNext(Util, user.UserID, amid);
                            if (tPlCheck == null)
                            {
                                throw new Exception(String.Format("BackEndMusic.BackEndMusicCallGet Error:{0}", "error id in poducts"));
                            }
                            else
                            {
                                tprodSec = tPlCheck.LastOrDefault();
                            }
                        }

                    }
                    else
                    {
                        while (tprodSec == null)
                        {
                            List<tProduct> tPlCheck = CPlaylist.getUserPlaylistSubscribeNext(Util, user.UserID, amid);
                            if (tPlCheck == null)
                            {
                                throw new Exception(String.Format("BackEndMusic.BackEndMusicCallGet Error:{0}", "error id in poducts"));
                            }else
                            {
                                Random rand = new Random();
                                int rInt = rand.Next(2, 10);
                                tprodSec = tPlCheck.Skip(rInt).Take(1).FirstOrDefault();
                            }
                        }
                    }
                }
                else
                {
                    //Normal User
                    if (Util.getEnumPlaymode(playmode) != "Random")
                    {
                        if (Util.getEnumPlaymode(playmode) != "LastSong")
                        {
                            tprodSec = CPlaylist.getUserPlaylistNormalNext(Util, user.UserID, amid).Skip(1).Take(1).FirstOrDefault();
                        }
                        else
                        {
                            tprodSec = CPlaylist.getUserPlaylistNormalNext(Util, user.UserID, amid).LastOrDefault();
                        }
                    }
                    else
                    {
                        while (tprodSec == null)
                        {
                            Random rand = new Random();
                            int rInt = rand.Next(2, 10);
                            tprodSec = CPlaylist.getUserPlaylistNormalNext(Util, user.UserID, amid).Skip(rInt).Take(1).FirstOrDefault();
                        }
                    }
                }
                if (tprodSec == null)
                {
                    throw new Exception(String.Format("BackEndMusic.BackEndMusicCallGet Error:{0}", "查無此2"));
                }

                int m_temp_nextalbum = (int)tprodSec.fAlbumID;
                try
                {
                    string talbSec = CgetAlbums.getAlbumByAlbumId(Util, m_temp_nextalbum).fCoverPath;
                    SQLExecResult sqlExecResult = null;
                    //SQL nonquery
                    if (DateTime.Now < user.SubscriptEndDate)
                    {
                        sqlExecResult = CPlaylist.updateUserLastPlaySongBytPlayList(Util, user.UserID, tprodSec.fProductID);
                    }
                    else
                    {
                        sqlExecResult = CPlaylist.updateUserLastPlaySongBytPurchaseItems(Util, user.UserID, tprodSec.fProductID);
                    }
                    if (sqlExecResult.bOK == false)
                    {
                        throw new Exception(sqlExecResult.message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("BackEndMusic.BackEndMusicCallGet Error:{0}", ex));
                }
                
                CPlayListOutputMusic tprodSecResult = new CPlayListOutputMusic
                {
                        fProductID = tprodSec.fProductID,
                        fProductName = tprodSec.fProductName,
                        fFilePath = tprodSec.fFilePath,
                        fComposer = tprodSec.fComposer,
                        fCoversource = CgetAlbums.getAlbumByAlbumId(Util, m_temp_nextalbum).fCoverPath,
                        fAlbumName = CgetAlbums.getAlbumByAlbumId(Util, m_temp_nextalbum).fAlbumName,
                        fSbuscribedate = DateTime.Now < user.SubscriptEndDate ? "包月會員:"+ user.SubscriptEndDate.ToString() : "一般會員",
                };
                return JObject.FromObject(tprodSecResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("AlbumDetail/{amid}")]
        [HttpGet()]
        public ActionResult<Array> AlbumDetail([FromRoute]int amid)
        {
            Array list = CgetProducts.getProductByAlbumId(Util, amid).ToArray();
            return list;
        }

        [Route("AlbumDetailSubI/{amid}")]
        [HttpGet()]
        public ActionResult<JObject> AlbumDetailSubI([FromRoute]int amid)
        {
            tAlbum list = CgetAlbums.getAlbumByAlbumId(Util, amid);
            if(list == null)
            {
                Exception ex =new Exception("error id");
                return JObject.FromObject(ex);
            }
            else
            {
                return JObject.FromObject(list);
            }
        }

        [Route("AlbumDetailSubII")]
        [HttpGet()]
        public ActionResult<int> AlbumDetailSubII()
        {
            if (string.IsNullOrEmpty(User.Identity.Name)){
                return 0;
            }
            else
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                return user.PrivLevel;
            }
        }

        [Authorize(Policy = "NormalUser")]
        [Route("PlayLists")]
        [HttpGet()]
        public ActionResult<Array> PlayLists()
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);

            List<tProduct> tp = CPlaylist.getUserPlaylistSubscribe(Util, user.LastPlaySong, user.UserID);
            return tp.ToArray();
        }

        [Authorize(Policy = "NormalUser")]
        [Route("_PlayLists")]
        [HttpGet()]
        public ActionResult _PlayLists()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Content("");
            }
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            List<tProduct> tp = null;
            if (DateTime.Now < user.SubscriptEndDate)
            {
                tp = CPlaylist.getUserPlaylistSubscribe(Util,user.LastPlaySong,user.UserID);
            }
            else
            {
                tp = CPlaylist.getUserPlaylistNormal(Util, user.LastPlaySong, user.UserID);
            }
            return Ok(tp);
        }


        [Route("PlayListAlbumDetail")]
        [HttpPost()]
        public ActionResult<JArray> PlayListAlbumDetail([FromBody] int[] amidList)
        {
            List<tAlbum> list = new List<tAlbum>() { };
            foreach (int amid in amidList)
            {
                tAlbum tA = CgetAlbums.getAlbumByAlbumId(Util, amid);
                list.Add(tA);
            }
            if (list == null)
            {
                Exception ex = new Exception("error id");
                return JArray.FromObject(ex);
            }
            else
            {
                return JArray.FromObject(list);
            }
        }

        [Authorize(Policy = "NormalUser")]
        [Route("getMemberUsingType")]
        [HttpGet()]
        public ActionResult<JObject> getMemberUsingType()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Content("");
            }
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            UserSubscriptType memberUseType = new UserSubscriptType() { };
            if (DateTime.Now < user.SubscriptEndDate)
            {
                memberUseType.typeName = "包月會員";
                memberUseType.typeId = "Subscription";
                memberUseType.endDate = user.SubscriptEndDate.ToString();
            }
            else
            {
                memberUseType.typeName = "一般會員";
                memberUseType.typeName = "Normal";
                memberUseType.endDate = "";
            }
            return Ok(JObject.FromObject(memberUseType));
        }

        [Route("addPlayLists/{amid}")]
        [HttpGet()]
        public ActionResult<JObject> addPlayLists([FromRoute]int amid)
        {
            string userID = "";
            if (string.IsNullOrEmpty(User.Identity.Name)){
                userID = "";
            }
            else
            {
                userID = User.Identity.Name;
            }
            try
            {
                string msg = CPlaylist.userAddPlayLists(Util, userID, amid);
                if (string.IsNullOrEmpty(userID))
                {
                    SuccessInfoAnonymous sucess = new SuccessInfoAnonymous(msg, Util);
                    return JObject.FromObject(sucess);
                }
                else
                {
                    UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                    SuccessInfo sucess = new SuccessInfo(msg, user, Util);
                    return JObject.FromObject(sucess);
                }
            }
            catch (Exception ex)
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                ExceptionInfo exInfo = new ExceptionInfo(ex, user, Util);
                return BadRequest(exInfo);
            }
        }

        [Route("deleteSongInPlayList/{amid}")]
        [HttpGet()]
        public ActionResult<JObject> deleteSongInPlayList([FromRoute]int amid)
        {
            string userID = "";
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                userID = "";
            }
            else
            {
                userID = User.Identity.Name;
            }
            try
            {
                string msg = CPlaylist.userDeletePlayLists(Util, userID, amid);
                if (string.IsNullOrEmpty(userID))
                {
                    SuccessInfoAnonymous sucess = new SuccessInfoAnonymous(msg, Util);
                    return JObject.FromObject(sucess);
                }
                else
                {
                    UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                    SuccessInfo sucess = new SuccessInfo(msg, user, Util);
                    return JObject.FromObject(sucess);
                }
            }
            catch (Exception ex)
            {
                UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
                ExceptionInfo exInfo = new ExceptionInfo(ex, user, Util);
                return BadRequest(exInfo);
            }
        }

        [Authorize(Policy = "NormalUser")]
        [Route("MyAlbumList")]
        [HttpGet()]
        public ActionResult<Array> MyAlbumList()
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            Array tA = CgetAlbums.getAlbumsByAccountId(Util, user.UserID).ToArray();
            return tA;
        }

        [Authorize(Policy = "NormalUser")]
        [Route("AlbumInfo/{amid}")]
        [HttpGet()]
        public ActionResult<Array> AlbumInfo([FromRoute]int amid)
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            Array tA = null;
            if (CgetAlbums.isAlbumOwnByAccount(Util, amid, user.UserID) == true)
            {
                tA = CgetProducts.getProductByAlbumId(Util, amid).ToArray();
            }
            else
            {
                SuccessInfo sucess = new SuccessInfo("error id or not author", user, Util);
                SuccessInfo[] suarr = { sucess };
                tA = suarr;
            }
            return tA;
        }

        [Authorize(Policy = "NormalUser")]
        [Route("MyMusic")]
        [HttpGet()]
        public ActionResult<Array> MyMusic()
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            List<tProduct> tp = null;
            tp = CPlaylist.getUserPlaylistNormalForMusicPage(Util, user.UserID).ToList();
            return tp.ToArray();
        }

        [Authorize(Policy = "NormalUser")]
        [Route("MusicDownload/{songId}")]
        [HttpGet()]
        public ActionResult MusicDownload([FromRoute] int songId)
        {
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), User.Identity.Name);
            string message = CAlbum.checkDownloadPrivilege(Util, user.UserID, songId);
            if (message == "")
            {
                string filepath = CgetProducts.getProductByProductId(Util, songId).fFilePath;
                string path = _he.WebRootPath + filepath;
                //取得檔案名稱
                string filename = System.IO.Path.GetFileName(path);
                //讀成串流
                Stream iStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/unknown", filename);
            }
            else
            {
                return Content(message);
            }
        }
    }
}