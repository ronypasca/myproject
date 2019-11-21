using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanListWSVM
    {
        [XmlArrayItem("BudgetPlanItem")]
        public List<BudgetPlanActionWSVM> BudgetPlan { get; set; }
    }
}