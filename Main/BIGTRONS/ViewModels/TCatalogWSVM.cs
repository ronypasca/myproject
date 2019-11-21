using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace com.SML.BIGTRONS.ViewModels
{
    public class TCatalogWSVM
    {
        public string CatalogCartID { get; set; }
        public string CatalogCartDesc { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public decimal GrandTotal { get; set; }

        [XmlArray("CartItem")]
        [XmlArrayItem("CartListItem")]
        public List<CatalogCartItemsWSMV> CatalogItemList { get; set; }
    }
}