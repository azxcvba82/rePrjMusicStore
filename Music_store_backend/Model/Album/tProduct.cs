using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tProduct
    {
        public int fProductID { get; set; }
        public Nullable<int> fAlbumID { get; set; }
        //[Display(Name = "單曲名稱")]
        public string fProductName { get; set; }
        //[Display(Name = "歌手")]
        public string fSinger { get; set; }
        //[Display(Name = "售價")]
        public Nullable<decimal> fSIPrice { get; set; }
        //[Display(Name = "作曲")]
        public string fComposer { get; set; }
        //[Display(Name = "音樂檔(限mp3)")]
        public string fFilePath { get; set; }
        public Nullable<double> fPlayStart { get; set; }
        public Nullable<double> fPlayEnd { get; set; }
    }
}
