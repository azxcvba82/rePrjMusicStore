using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Domain.Authorization.Secret
{
    public class DataAccessService
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        private static string _connectionStr = "server=localhost;database=dbProjectMusicStore;user=webuser1;password=user123My;port=1433;Connect Timeout=30;";

        /// <summary>
        /// 全域的資料庫連線字串
        /// </summary>
        public static string connectionStr { get { return _connectionStr; } }
    }
}
