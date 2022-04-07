using Music_store_backend.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class UserPrincipal : IPrincipal
    {
        protected readonly AppSettings Util;
        SqlConnection mSqlConnectionMBMan = null;
        private GenericIdentity mIdentity;
        private String mUserId = "";
        private String mUserName = "";
        private String mLangId = "";
        private String mLangName = "";
        private String mTimeZone = "";
        private String mIsActive = "";
        private String mPasswordMD5 = "";
        private int mLastPlaySong = 0;
        private int mPrivLevel = 1;
        private String mMail = "";
        private DateTime mSubscriptEndDate = DateTime.Today.AddDays(-1);
        private String mDBConnectionString = "";

        public UserPrincipal(string connectionStringMBMan="", string userId="")
        {
            if(string.IsNullOrEmpty(connectionStringMBMan) || string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("onnectionStringMBMan or iserId null");
            }
            this.mSqlConnectionMBMan = new SqlConnection(connectionStringMBMan);
            EncUserToken encUser = null;
            if (userId.StartsWith("55B5"))
            {
                String encUserJson = CryptoUtil.Decrypt(userId);
                encUser = JsonConvert.DeserializeObject<EncUserToken>(encUserJson);
                userId = encUser.userId;
            }
            mUserId = userId;

            //mDBConnectionString = this.GetDBConnectionStringFromUserId(mUserId);
            mIdentity = new GenericIdentity(userId);
            SqlConnection myConnection = this.GetSqlConnectionMBMan();
            String strSQL = " SELECT *,  " +
                       " langName=(SELECT fLangName FROM tLang WHERE  fLangId=A.fLangId)" +
                     " FROM tMember A " +
                     " WHERE A.fAccount = @userId";

            SqlDataAdapter myCommand = new SqlDataAdapter(strSQL, myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", mIdentity.Name));
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "tMember");
            if (ds.Tables[0].Rows.Count == 0)
                return;
            DataRow r = ds.Tables[0].Rows[0];

            //mCSPId = r["cspId"].ToString();
            //mAPITypeId = r["apiTypeId"].ToString();
            //mCSPName = r["cspName"].ToString(); ;

            //mUserType = r["userType"].ToString();

            //mCustomerId = r["customerId"].ToString();
            //mCompanyName = r["companyName"].ToString();
            //mEAEnrollmentNumber = r["eaEnrollmentNumber"].ToString();

            mUserName = r["fNickName"].ToString();
            mPrivLevel = (int)r["fPrivilege"];
            mLastPlaySong = Int32.TryParse(r["fLastPlaySong"].ToString(), out mLastPlaySong) ? mLastPlaySong:0;
            mLangId = r["fLangId"].ToString();
            mTimeZone = r["fTimeZone"].ToString();
            mLangName = r["langName"].ToString();
            mIsActive = r["fisActive"].ToString();
            mPasswordMD5 = r["fPassword"].ToString();
            mMail = r["fEmail"].ToString();
            mSubscriptEndDate = (DateTime)r["fSubscriptEndDate"];
            //this.mAppId = r["appId"].ToString();
            //this.mAppName = r["appName"].ToString();

            //mCSP = CSPProfile.GetCSPProfile(connectionStringCWMan, mCSPId);
            //if (encUser != null)
            //{
            //    mCSPId = encUser.cspId;
            //    mLangId = encUser.langId;
            //    mTimeZone = encUser.timeZone;
            //}
        }

        private String GetDBConnectionStringFromUserId(String userId)
        {
            SqlConnection myConnection = this.GetSqlConnectionMBMan();

            String strSQL = "SELECT  cspId  " +
                           " FROM AppUser " +
                           " WHERE userId=@userId";
            SqlDataAdapter myCommand = new SqlDataAdapter(strSQL, myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", userId));
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");
            String cspId = (String)ds.Tables[0].Rows[0]["cspId"];
            return GetDBConnectionStringFromCSPId(cspId);

        }

        private String GetDBConnectionStringFromCSPId(String cspId)
        {
            SqlConnection myConnection = this.GetSqlConnectionMBMan();

            String strSQL = "SELECT TOP 1 dbConnectionString  " +
                           " FROM DB " +
                           " WHERE dbId=(SELECT dbId FROM CSP WHERE cspId=@cspId)";
            SqlDataAdapter myCommand = new SqlDataAdapter(strSQL, myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@cspId", cspId));
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");
            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return (String)ds.Tables[0].Rows[0]["dbConnectionString"];

        }

        public String PasswordMD5
        {
            get { return mPasswordMD5; }
        }

        public String UserName
        {
            get { return mUserName; }
        }

        public String UserID
        {
            get { return mUserId; }
        }

        public int PrivLevel
        {
            get { return mPrivLevel; }
        }

        public int LastPlaySong
        {
            get { return mLastPlaySong; }
        }


        public String LangId
        {
            get { return mLangId; }
        }

        public String TimeZone
        {
            get { return mTimeZone; }
        }

        public String LangName
        {
            get { return mLangName; }
        }


        public String IsActive
        {
            get { return mIsActive; }
        }

        public DateTime SubscriptEndDate
        {
            get { return mSubscriptEndDate; }
        }

        public IIdentity Identity
        {
            get { return mIdentity; }
        }

        private SqlConnection GetSqlConnectionMBMan()
        {
            return mSqlConnectionMBMan;
        }

        public bool IsInRole(string role)
        {
            //SqlConnection myConnection = new SqlConnection(mDBConnectionString);
            SqlConnection myConnection = this.GetSqlConnectionMBMan();
            String strSQL = "SELECT fRoleId FROM tMemberRole" +
                           " WHERE fAccount=@userId AND fRoleId=@appRoleId";
            SqlDataAdapter myCommand = new SqlDataAdapter(strSQL, myConnection);
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@userId", mUserId));
            myCommand.SelectCommand.Parameters.Add(new SqlParameter("@appRoleId", role));
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "t1");

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public String Mail
        {
            get { return mMail; }
            set { mMail = value; }

        }
    }
}
