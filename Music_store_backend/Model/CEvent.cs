using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CEvent
    {
        public static IEnumerable<tActivity> ConstructEventQuery(AppSettings Util, string account)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            IEnumerable<tActivity> tA = null;
            if (string.IsNullOrEmpty(account))
            {
                tA = myConnection.Query<tActivity>("SELECT * FROM tActivity where fStartTime < GETDATE()", new { });
            }
            else
            {
                tA = myConnection.Query<tActivity>("SELECT * FROM tActivity where fLauncher=@account AND fStartTime < GETDATE()", new { account = account });
            }
            return tA;
        }
    }
}
