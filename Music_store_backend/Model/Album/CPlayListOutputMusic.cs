using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CPlayListOutputMusic
    {
        public int fProductID { get; set; }
        public string fProductName { get; set; }
        public string fFilePath { get; set; }
        public string fComposer { get; set; }
        public string fCoversource { get; set; }
        public string fAlbumName { get; set; }
        public string fSbuscribedate { get; set; }
    }
}
