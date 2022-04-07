using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CgetProducts
    {
        public static List<tProduct> getProductByAlbumId(AppSettings Util, int amid)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tProduct> tP = myConnection.Query<tProduct>("SELECT * FROM tProducts where fAlbumID=@fAlbumID ", new { fAlbumID = amid }).ToList();
            return tP;
        }

        public static tProduct getProductByProductId(AppSettings Util, int prodid)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tProduct tP = myConnection.QueryFirstOrDefault<tProduct>("SELECT * FROM tProducts where fProductID=@fProductID ", new { fProductID = prodid });
            return tP;
        }

    }
}
