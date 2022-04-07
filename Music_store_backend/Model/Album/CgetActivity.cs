using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CgetActivity
    {
        public static List<tActivity> getActivitiesWithInPeriod(AppSettings Util, DateTime time)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            List<tActivity> tA = myConnection.Query<tActivity>("SELECT * FROM tActivity where fStartTime < @time AND fEndTime > @time", new { time = time }).ToList();
            return tA;
        }

        public static tActivity getActivityById(AppSettings Util, int eventId)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            tActivity tA = myConnection.QueryFirstOrDefault<tActivity>("SELECT * FROM tActivity where fId = @fId", new { fId = eventId });
            return tA;
        }
    }
}
