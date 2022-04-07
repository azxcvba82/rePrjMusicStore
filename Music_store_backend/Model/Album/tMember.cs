using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tMember
    {
        public string fAccount { get; set; }
        public string fPassword { get; set; }
        public string fEmail { get; set; }
        public Nullable<int> fPrivilege { get; set; }
        public string fNickName { get; set; }
        public Nullable<decimal> fMoney { get; set; }
        public string fPicPath { get; set; }
        public Nullable<System.DateTime> fSubscriptStartDate { get; set; }
        public Nullable<System.DateTime> fSubscriptEndDate { get; set; }
        public Nullable<int> fLastPlaySong { get; set; }
        public string fLineId { get; set; }
        public string fLineName { get; set; }
        public Nullable<System.DateTime> fLineTimeMark { get; set; }
        public Nullable<int> fLineStatus { get; set; }
        public string fPayPalAccount { get; set; }
    }
}
