using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tMessage
    {
        public int fMessageId { get; set; }
        //[Display(Name = "寄件人")]
        public string fAccountFrom { get; set; }
        //[Display(Name = "收件人")]
        public string fAccountTo { get; set; }
        //[Display(Name = "內文")]
        public string fContent { get; set; }
        public Nullable<System.DateTime> fTime { get; set; }
        public Nullable<int> fStatus { get; set; }
        //[Display(Name = "主旨")]
        public string fTitle { get; set; }
        public Nullable<int> fReaded { get; set; }
    }
}
