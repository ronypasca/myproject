using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace com.SML.BIGTRONS.ViewModels
{
    public class PackageWSVM
    {
        public string PackageID { get; set; }
        public string PackageDesc { get; set; }
        public decimal GrandTotal { get; set; }
        [XmlArray("PackageList")]
        [XmlArrayItem("PackageListItem")]
        public List<BudgetPlanWSVM> PackageList { get; set; }
    }
}