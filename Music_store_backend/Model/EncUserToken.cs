using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class EncUserToken
    {
        public String userId { get; set; }
        public String cspId { get; set; }
        public String langId { get; set; }
        public String timeZone { get; set; }
    }
}
