using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class SuccessInfo
    {
        private UserPrincipal mUser = null;
        private String mMessage = "";
        public SuccessInfo(String message, UserPrincipal user, AppSettings Util)
        {
            mUser = user;
            mMessage = message;
        }
        public String Message
        {
            get { return mMessage; }
        }
    }

            public class SuccessInfoAnonymous
    {
        private String mUser = null;
        private String mMessage = "";
        public SuccessInfoAnonymous(String message, AppSettings Util)
        {
            mUser = "Anonymous";
            mMessage = message;
        }
        public String Message
        {
            get { return mMessage; }
        }
    }
}
