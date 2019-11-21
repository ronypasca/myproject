using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;


namespace com.SML.BIGTRONS.Models
{
    public class TNegotiationBidStructures : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationBidID { get; set; }
        public string NegotiationConfigID { get; set; }
        public int Sequence { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string ItemParentID { get; set; }
        public decimal BudgetPlanDefaultValue { get; set; }
        public int? Version { get; set; }
        public int? ParentVersion { get; set; }
        public int? ParentSequence { get; set; }

        #endregion
    }
}