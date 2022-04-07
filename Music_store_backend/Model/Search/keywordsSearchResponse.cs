using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class KeywordsSearchResponse
    {
        public int fAlbumID { get; set; }
        public int fProductID { get; set; }
        public string fProductName { get; set; }
        public string fSinger { get; set; }
        public string fComposer { get; set; }
        public string fAlbumName { get; set; }
        public string fCoverPath { get; set; }
        public string fKinds { get; set; }
        public Nullable<float> fDiscount { get; set; }
        public Nullable<int> fActivityID { get; set; }
    }
}
