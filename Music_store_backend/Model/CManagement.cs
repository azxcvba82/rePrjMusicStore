using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CManagement
    {
        public static SQLExecResult typeNew(AppSettings Util, string typeName)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            string strSQL = " INSERT INTO tAlbumType " +
                " (fTypeName)" +
                " VALUES (@fTypeName) ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("fTypeName", SqlDbType.NVarChar, 50));
            myCommand.Parameters["fTypeName"].Value = typeName;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }

        public static SQLExecResult typeDelete(AppSettings Util, int deleteId)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            string strSQL = " DELETE FROM tAlbumType " +
                " WHERE fTypeID=@fTypeID ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("fTypeID", SqlDbType.Int));
            myCommand.Parameters["fTypeID"].Value = deleteId;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }

        public static SQLExecResult typeEdit(AppSettings Util, int typeOrigin, string typeChange)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();

            string strSQL = " UPDATE tAlbumType " +
                " SET fTypeName=@fTypeName" +
                " WHERE fTypeID=@fTypeID ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("fTypeName", SqlDbType.NVarChar, 50));
            myCommand.Parameters["fTypeName"].Value = typeChange;
            myCommand.Parameters.Add(new SqlParameter("fTypeID", SqlDbType.Int));
            myCommand.Parameters["fTypeID"].Value = typeOrigin;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }
    }
}
