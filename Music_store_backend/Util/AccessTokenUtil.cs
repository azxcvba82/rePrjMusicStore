using Microsoft.IdentityModel.Tokens;
using Music_store_backend.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Music_store_backend.Util
{
    public class AccessTokenUtil
    {
        public static String GenerateRefreshToken(AppSettings Util, String userId)
        {

            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), userId);

            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            SqlDataAdapter myCommand = new SqlDataAdapter("SELECT * FROM tMemberRole where fAccount=@userId", myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar, 50));
            myCommand.SelectCommand.Parameters["@userId"].Value = userId;
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");
            //return String.Format("here1 userId={0}, rows={1}", userId, ds.Tables[0].Rows);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("webuser1gogo#123456"));
            List<Claim> claimList = new List<Claim>();
            claimList.Add(new Claim(ClaimTypes.Name, userId));
            claimList.Add(new Claim("Mail", user.Mail));

            claimList.Add(new Claim("Credential", user.PasswordMD5));
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                claimList.Add(new Claim(ClaimTypes.Role, (String)r["fRoleId"]));

            }

            var claims = claimList.ToArray<Claim>();

            var token = new JwtSecurityToken(
                issuer: "BackendM",
                audience: "BackendM-RefreshToken",
                claims: claims,
                notBefore: DateTime.Now,
                // expires: DateTime.Now.AddDays(28),
                expires: DateTime.Now.AddYears(1),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            SQLExecResult sqlExecResult = AssociateRefreshTokenWithUser(Util, userId, jwtToken);
            if (sqlExecResult.bOK == false)
            {
                throw new Exception(sqlExecResult.message);
            }

            return jwtToken;

        }

        private static SQLExecResult AssociateRefreshTokenWithUser(AppSettings Util, String userId, String refreshToken)
        {
            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            string strSQL = " UPDATE tMember " +
                            " SET fRefreshToken =@refreshToken" +
                            " WHERE fAccount=@userId ";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.NVarChar, 200));
            myCommand.Parameters["@userId"].Value = userId;
            myCommand.Parameters.Add(new SqlParameter("@refreshToken", SqlDbType.NVarChar));
            myCommand.Parameters["@refreshToken"].Value = refreshToken;
            String message = "";
            bool bOK = Util.SQLExecute(myCommand, out message);
            SQLExecResult sqlExecResult = new SQLExecResult
            {
                bOK = bOK,
                message = message
            };
            return sqlExecResult;
        }

        public static String GenerateAccessToken(AppSettings Util, String userId)
        {


            UserPrincipal user = new UserPrincipal(Util.GetSqlConnectionStringMBMan(), userId);

            SqlConnection myConnection = Util.GetSqlConnectionMBMan();
            SqlDataAdapter myCommand = new SqlDataAdapter("SELECT * FROM tMemberRole WHERE fAccount=@userId", myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar, 50));
            myCommand.SelectCommand.Parameters["@userId"].Value = userId;
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");
            //return String.Format("here1 userId={0}, rows={1}", userId, ds.Tables[0].Rows);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("webuser1gogo#123456"));
            List<Claim> claimList = new List<Claim>();
            claimList.Add(new Claim(ClaimTypes.Name, userId));
            claimList.Add(new Claim("Mail", user.Mail));

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                claimList.Add(new Claim(ClaimTypes.Role, (String)r["fRoleId"]));
            }

            var claims = claimList.ToArray<Claim>();

            var token = new JwtSecurityToken(
                issuer: "BackendM",
                audience: "BackendM",
                claims: claims,
                notBefore: DateTime.Now,
                // expires: DateTime.Now.AddDays(28),
                expires: DateTime.Now.AddHours(1),
                //expires: DateTime.Now.AddMinutes(2),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;

        }
    }
}
