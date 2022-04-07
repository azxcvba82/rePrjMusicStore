using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tActivity
    {
        public int fId { get; set; }
        public string fLauncher { get; set; }
        public Nullable<System.DateTime> fStartTime { get; set; }
        public Nullable<System.DateTime> fEndTime { get; set; }
        public string fTitle { get; set; }
        public string fPhotoPath { get; set; }
    }
}
