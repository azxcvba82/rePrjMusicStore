using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CMusic
    {
        public static tProduct getMusic(AppSettings Util, int musicId)
        {
            tProduct tp = CgetProducts.getProductByProductId(Util, musicId);
            return tp;
        }
    }
}
