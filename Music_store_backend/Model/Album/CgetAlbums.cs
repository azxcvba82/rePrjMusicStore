using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CgetAlbums
    {
        public static tAlbum getAlbumByAlbumId(AppSettings Util, int amid)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tAlbum tA = myConnection.QueryFirstOrDefault<tAlbum>("SELECT * FROM tAlbum where fAlbumID=@fAlbumID ", new { fAlbumID = amid });
            return tA;
        }

        public static List<tAlbum> getAlbumsByAccountId(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List <tAlbum> tA = myConnection.Query<tAlbum>("SELECT * FROM tAlbum where fAccount=@fAccount ", new { fAccount = account }).ToList();
            return tA;
        }

        public static List<tAlbum> getAlbumsByStatus(AppSettings Util, int status)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tAlbum> tA = myConnection.Query<tAlbum>("SELECT * FROM tAlbum where fStatus=@fStatus ", new { fStatus = status }).ToList();
            return tA;
        }

        public static bool isAlbumOwnByAccount(AppSettings Util,int amid, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tAlbum tA = myConnection.QueryFirstOrDefault<tAlbum>("SELECT * FROM tAlbum where fAlbumID=@fAlbumID AND fAccount=@fAccount", new { fAlbumID = amid, fAccount = account });
            if (tA == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
