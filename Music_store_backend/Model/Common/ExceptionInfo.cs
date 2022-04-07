using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class ExceptionInfo
    {
        private Exception mException = null;
        private UserPrincipal mUser = null;
        private String mMessage = "";
        private String mStackTrace = "";
        public ExceptionInfo(Exception ex, UserPrincipal user, AppSettings Util)
        {

            mException = ex;
            mUser = user;
            // mMessage = Util.Translate("Message", user) + ":" + Util.Translate(mException.Message,user);
            mMessage = mException.Message;
            // if (user.IsInRole("SysAdmin")) {


        }
        public String Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }

        public String StackTrace
        {
            get { return mStackTrace; }
        }
    }
}
