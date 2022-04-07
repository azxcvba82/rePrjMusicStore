using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CSearchResult
    {
        public KeywordsSearchResponse album { get; set; }
        public tProduct product { get; set; }
    }
}
