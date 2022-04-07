using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class UserProfile
    {
        private UserPrincipal mUser = null;
        private String mTimeZoneName = null;
        public UserProfile(SqlConnection myConnection, UserPrincipal user)
        {
            mUser = user;

        }

        public String UserName
        {
            get { return mUser.UserName; }
        }
        public int PrivLevel
        {
            get { return mUser.PrivLevel; }
        }

        public String LangId
        {
            get { return mUser.LangId; }
        }

        public String LangName
        {
            get { return mUser.LangName; }
        }

        public String IsActive
        {
            get { return mUser.IsActive; }
        }
    }
}
