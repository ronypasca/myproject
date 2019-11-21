using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanWSVM
    {
        public string BudgetPlanID { get; set; }
        public string BudgetPlanDesc { get; set; }
        public string BudgetPlanType { get; set; }
        public int Version { get; set; }
        public string Project { get; set; }
        public string Cluster { get; set; }
        public string Region { get; set; }
        public string Division { get; set; }
        public string Company { get; set; }
        public string UnitType { get; set; }
        public string Area { get; set; }
        public string Unit { get; set; }
        public string Location { get; set; }
        [XmlArray("StructureList")]
        [XmlArrayItem("StructureListItem")]
        public List<BudgetPlanVersionStructureWSVM> Structure { get; set; }
        public decimal SummaryStructure { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal PriceArea { get; set; }
    }
}