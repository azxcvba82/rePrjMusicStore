using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class tPurchaseItem
    {
        public int fPurchaseItemID { get; set; }
        public string fCustomer { get; set; }
        public int fProductID { get; set; }
        public Nullable<System.DateTime> fDate { get; set; }
        public Nullable<decimal> fPrice { get; set; }
        public Nullable<int> fQuanity { get; set; }
        public Nullable<int> fisAlbum { get; set; }
        public Nullable<float> fDiscount { get; set; }
    }
}
