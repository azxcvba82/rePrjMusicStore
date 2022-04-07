using Dapper;
using Music_store_backend.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend
{
    public class CPlaylist
    {
        public static List<tProduct> getUserPlaylistSubscribe(AppSettings Util, int lastPlaySong ,string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tPlayList tPLCheck = myConnection.QueryFirstOrDefault<tPlayList>("SELECT * FROM tPlayLists where fPlayId=@fPlayId ", new { fPlayId = lastPlaySong });
            if (tPLCheck == null)
            {
                SQLExecResult sqlExecResult = CPlaylist.emptyUserLastPlaySong(Util, account);
                if (sqlExecResult.bOK == false)
                {
                    throw new Exception(sqlExecResult.message);
                }
            }
            List<tPlayList> tPL = myConnection.Query<tPlayList>("SELECT * FROM tPlayLists where fAccount=@fAccount ", new { fAccount = account }).ToList();
            List<tPlayList> tPLOut = null;
            if (tPLCheck != null)
            {
                int lastPlayID = tPL.FirstOrDefault(p => p.fPlayId == lastPlaySong).fPlayId;
                tPLOut = tPL.Where(p => p.fPlayId >= lastPlayID).ToList();
                tPLOut.AddRange(tPL.Where(p => p.fPlayId < lastPlayID));
            }
            else
            {
                tPLOut = tPL;
            }
            List<tProduct> tp = new List<tProduct>();
            foreach (var a in tPLOut)
            {
                tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                tp.Add(tP);
            }
            return tp;
        }

        public static List<tProduct> getUserPlaylistNormal(AppSettings Util, int lastPlaySong, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tPurchaseItem> tPI = myConnection.Query<tPurchaseItem>("SELECT * FROM tPurchaseItem a left join tShoppingCart b on a.fPurchaseItemID = b.fCartID where a.fCustomer=@account AND b.fType=1", new { account = account}).ToList();
            tPurchaseItem tPICheck = tPI.FirstOrDefault(p => p.fProductID == lastPlaySong);
            if (tPICheck == null)
            {
                SQLExecResult sqlExecResult = CPlaylist.emptyUserLastPlaySong(Util, account);
                if (sqlExecResult.bOK == false)
                {
                    throw new Exception(sqlExecResult.message);
                }
            }
            List<tPurchaseItem> tPLOut = null;
            if (tPICheck != null)
            {
                int lastPlayID = tPICheck.fProductID;
                tPLOut = tPI.Where(p => p.fProductID >= lastPlayID).ToList();
                tPLOut.AddRange(tPI.Where(p => p.fProductID < lastPlayID));
            }
            else
            {
                tPLOut = tPI;
            }
            List<tProduct> tp = new List<tProduct>();
            foreach (var a in tPLOut)
            {
                //如果是買整張專輯則列出所有單曲
                if (a.fisAlbum == 1)
                {
                    tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                    if (tP != null)
                    {
                        List<tProduct> LtP = myConnection.Query<tProduct>("SELECT * FROM tProducts where fAlbumID=@fAlbumID ", new { fAlbumID = tP.fAlbumID }).ToList();
                        tp.AddRange(LtP);
                    }
                }
                //如果是買單曲則只列該單曲
                else
                {
                    tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                    if(tP != null)
                    {
                        tp.Add(tP);
                    }
                }
            }
            return tp;
        }

        public static List<tProduct> getUserPlaylistNormalForMusicPage(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tPurchaseItem> tPI = myConnection.Query<tPurchaseItem>("SELECT * FROM tPurchaseItem a left join tShoppingCart b on a.fPurchaseItemID = b.fCartID where a.fCustomer=@account AND b.fType=1", new { account = account }).ToList();

            List<tProduct> tp = new List<tProduct>();
            foreach (var a in tPI)
            {
                ////如果是買整張專輯則列出所有單曲
                //if (a.fisAlbum == 1)
                //{
                //    tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                //    if (tP != null)
                //    {
                //        List<tProduct> LtP = myConnection.Query<tProduct>("SELECT * FROM tProducts where fAlbumID=@fAlbumID ", new { fAlbumID = tP.fAlbumID }).ToList();
                //        tp.AddRange(LtP);
                //    }
                //}
                ////如果是買單曲則只列該單曲
                //else
                //{
                    tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                    if (tP != null)
                    {
                        tp.Add(tP);
                    }
                //}
            }
            return tp;
        }

        public static SQLExecResult emptyUserLastPlaySong(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            string strSQL = " UPDATE tMember " +
                " SET fLastPlaySong =@lastPlaySong" +
                " WHERE fAccount=@userId ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 50));
            myCommand.Parameters["@userId"].Value = account;
            myCommand.Parameters.Add(new SqlParameter("@lastPlaySong", SqlDbType.Int));
            myCommand.Parameters["@lastPlaySong"].Value = SqlInt32.Null;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }

        public static List<tProduct> getUserPlaylistSubscribeNext(AppSettings Util, string account, int amid)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tPlayList> tPL = myConnection.Query<tPlayList>("SELECT * FROM tPlayLists where fAccount=@fAccount ", new { fAccount = account }).ToList();
            tPlayList lastPlayIDObj = tPL.FirstOrDefault(p => p.fProductID == amid);
            if(lastPlayIDObj == null)
            {
                //return null;
                if (tPL.Count() != 0) {
                    lastPlayIDObj = tPL.First();
                }
                else
                {
                    return null;
                }
            }
            int lastPlayID = lastPlayIDObj.fPlayId;
            List<tPlayList> tPLOut = null;
            if (lastPlayID != 0)
            {
                tPLOut = tPL.Where(p => p.fPlayId >= lastPlayID).ToList();
                tPLOut.AddRange(tPL.Where(p => p.fPlayId < lastPlayID));
            }
            else
            {
                tPLOut = tPL;
            }
            List<tProduct> tp = new List<tProduct>();
            foreach (var a in tPLOut)
            {
                tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID});
                tp.Add(tP);
            }
            return tp;
        }

        public static List<tProduct> getUserPlaylistNormalNext(AppSettings Util, string account, int amid)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            //int? fLastPlaySong = db.tMembers.FirstOrDefault(p => p.fAccount == s1).fLastPlaySong;
            List<tPurchaseItem> tPL = myConnection.Query<tPurchaseItem>("SELECT * FROM tPurchaseItem where fCustomer=@fAccount ", new { fAccount = account }).ToList();
            tPurchaseItem tPI = tPL.FirstOrDefault(p => p.fProductID == amid);
            List<tPurchaseItem> tPLOut = null;
            if (tPI != null)
            {
                int lastPlayID = tPI.fPurchaseItemID;
                tPLOut = tPL.Where(p => p.fPurchaseItemID >= lastPlayID).ToList();
                tPLOut.AddRange(tPL.Where(p => p.fPurchaseItemID < lastPlayID));
            }
            else
            {
                tPLOut = tPL;
            }
            // List<tProduct> tp = null;
            List<tProduct> tp = new List<tProduct>();
            foreach (var a in tPLOut)
            {
                tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = a.fProductID });
                tp.Add(tP);
            }
            return tp;
        }

        public static SQLExecResult updateUserLastPlaySongBytPlayList(AppSettings Util, string account, int productId)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            int? fPlayId = myConnection.QueryFirstOrDefault<tPlayList>("SELECT * FROM tPlayLists where fAccount=@fAccount AND fProductID=@fProductID", new { fAccount = account, fProductID = productId }).fPlayId;

            string strSQL = " UPDATE tMember " +
                " SET fLastPlaySong =@lastPlaySong" +
                " WHERE fAccount=@userId ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 50));
            myCommand.Parameters["@userId"].Value = account;
            myCommand.Parameters.Add(new SqlParameter("@lastPlaySong", SqlDbType.Int));
            myCommand.Parameters["@lastPlaySong"].Value = fPlayId;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }
        public static SQLExecResult updateUserLastPlaySongBytPurchaseItems(AppSettings Util, string account, int productId)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            int? fProductID = myConnection.QueryFirstOrDefault<tPurchaseItem>("SELECT * FROM tPurchaseItem where fCustomer=@fAccount AND fProductID=@fProductID", new { fAccount = account, fProductID = productId }).fProductID;

            string strSQL = " UPDATE tMember " +
                " SET fLastPlaySong =@lastPlaySong" +
                " WHERE fAccount=@userId ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 50));
            myCommand.Parameters["@userId"].Value = account;
            myCommand.Parameters.Add(new SqlParameter("@lastPlaySong", SqlDbType.Int));
            myCommand.Parameters["@lastPlaySong"].Value = fProductID;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }
        public static string userAddPlayLists(AppSettings Util, string account, int amid)
        {
            string s2 = "";
            //有無此人
            if (account == "")
            {
                s2 = "撥放清單需登入才能使用";
                return s2;
            }
            //是否過期
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), account);
            DateTime? dt = user.SubscriptEndDate;
            if (dt == null || dt < DateTime.Now)
            {
                s2 = "此帳號包月已過期無法加入單曲";
                return s2;
            }
            //有無此單曲
            tProduct tp = CgetProducts.getProductByProductId(Util, amid);
            if (tp == null)
            {
                s2 = "查無此單曲";
                return s2;
            }
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tPlayList> tnowPL = myConnection.Query<tPlayList>("SELECT * FROM tPlayLists where fAccount=@fAccount ", new { fAccount = user.UserID }).ToList();
            if (tnowPL.FirstOrDefault(p => p.fProductID == amid) != null)
            {
                s2 = "撥放清單已經有此首音樂了";
                return s2;
            }

            string strSQL = " INSERT INTO tPlayLists " +
            " (fAccount, fProductID) " +
            " VALUES (@userId,@fProductID) ";
            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar, 50));
            myCommand.Parameters["@userId"].Value = user.UserID;
            myCommand.Parameters.Add(new SqlParameter("@fProductID", SqlDbType.Int));
            myCommand.Parameters["@fProductID"].Value = amid;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            if (sqlExecResult.bOK == false)
            {
                throw new Exception(sqlExecResult.message);
            }
            s2 = "成功";
            return s2;
        }

        public static string userDeletePlayLists(AppSettings Util, string account, int amid)
        {
            string s2 = "";
            //有無此人
            if (account == "")
            {
                s2 = "撥放清單需登入才能使用";
                return s2;
            }
            //是否過期
            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), account);
            DateTime? dt = user.SubscriptEndDate;
            if (dt == null || dt < DateTime.Now)
            {
                s2 = "此帳號包月已過期無法刪除單曲";
                return s2;
            }
            //有無此單曲
            tProduct tp = CgetProducts.getProductByProductId(Util, amid);
            if (tp == null)
            {
                s2 = "查無此單曲";
                return s2;
            }
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tPlayList> tnowPL = myConnection.Query<tPlayList>("SELECT * FROM tPlayLists where fAccount=@fAccount ", new { fAccount = user.UserID }).ToList();
            if (tnowPL.FirstOrDefault(p => p.fProductID == amid) == null)
            {
                s2 = "你的撥放清單找不到此首音樂";
                return s2;
            }
            //最後一首單曲時移除member欄位的最後單曲紀錄
            if (tnowPL.Count() == 1)
            {
                CPlaylist.emptyUserLastPlaySong(Util, user.UserID);
            }

            //移除此單曲
            string strSQL = " DELETE FROM tPlayLists " +
            " WHERE fAccount = @userId AND fProductID = @fProductID ";
            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar, 50));
            myCommand.Parameters["@userId"].Value = user.UserID;
            myCommand.Parameters.Add(new SqlParameter("@fProductID", SqlDbType.Int));
            myCommand.Parameters["@fProductID"].Value = amid;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            if (sqlExecResult.bOK == false)
            {
                throw new Exception(sqlExecResult.message);
            }
            s2 = "成功";
            return s2;
        }
    }
}
