using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CMessage
    {
        public static List<tMessage> getReceiveMessage(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tMessage> tM = myConnection.Query<tMessage>("SELECT * FROM tMessage where fAccountTo=@account AND fStatus = 1", new { account = account }).ToList();
            tM.Reverse();
            return tM;
        }

        public static IEnumerable<tMessage> getReceiveMessagePage(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            IEnumerable<tMessage> tM = myConnection.Query<tMessage>("SELECT * FROM tMessage where fAccountTo=@account AND fStatus = 1", new { account = account });
            tM.Reverse();
            return tM;
        }
        
        public static List<tMessage> getSendMessageCopy(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tMessage> tM = myConnection.Query<tMessage>("SELECT * FROM tMessage where fAccountFrom=@account AND fStatus = 0", new { account = account }).ToList();
            tM.Reverse();
            return tM;
        }
    }
}
