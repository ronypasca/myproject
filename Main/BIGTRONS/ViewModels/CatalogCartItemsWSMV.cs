using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.SML.BIGTRONS.ViewModels
{
    public class CatalogCartItemsWSMV
    {
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public string ItemPriceID { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
    }
}