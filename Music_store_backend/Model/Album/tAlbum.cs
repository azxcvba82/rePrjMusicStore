using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tAlbum
    {
        public int fAlbumID { get; set; }
        public string fAlbumName { get; set; }
        public string fMaker { get; set; }
        public string fAccount { get; set; }
        public Nullable<System.DateTime> fYear { get; set; }
        public Nullable<int> fType { get; set; }
        public Nullable<int> fStatus { get; set; }
        public Nullable<decimal> fALPrice { get; set; }
        public string fCoverPath { get; set; }
        public string fKinds { get; set; }
        public Nullable<float> fDiscount { get; set; }
        public Nullable<int> fActivityID { get; set; }
    }
}
