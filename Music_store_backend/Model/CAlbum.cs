using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CAlbum
    {
        public static string checkDownloadPrivilege(AppSettings Util, string account, int songid)
        {
            string s2 = "";
            //不同會員有相同網址但該會員沒有購買這首時的防護
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tProduct tp = CgetProducts.getProductByProductId(Util, songid);
            if (tp == null)
            {
                return "查無此ID";
            }
            else
            {
                int? amid = tp.fAlbumID;
                tPurchaseItem tPI = myConnection.QueryFirstOrDefault<tPurchaseItem>("SELECT * FROM tPurchaseItem a left join tShoppingCart b on a.fPurchaseItemID = b.fCartID left join tProducts c on a.fProductID = c.fProductID where a.fCustomer=@account AND b.fType=1 AND ( a.fProductID=@fProductID OR c.fAlbumID=@fAlbumID)", new { account = account, fProductID = songid, fAlbumID=amid });
                if(tPI == null)
                {
                    return "你沒有購買這首單曲";
                }
                //if (db.tPurchaseItems.FirstOrDefault(p => p.fCustomer == s1 && p.tShoppingCart.fType == 1 && (p.fProductID == songid || (p.fisAlbum == 1 && p.tProduct.tAlbum.fAlbumID == amid))) == null)
                //{
                //    return "你沒有購買這首單曲";
                //}
                return s2;
            }
        }
    }
}
