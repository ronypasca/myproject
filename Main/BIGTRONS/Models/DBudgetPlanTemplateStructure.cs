using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DBudgetPlanTemplateStructure : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanTemplateID { get; set; }
        [Key, Column(Order = 2)]
        public string ItemID { get; set; }
        [Key, Column(Order = 3)]
        public int Version { get; set; }
        [Key, Column(Order = 4)]
        public int Sequence { get; set; }
        public string ParentItemID { get; set; }
        public int ParentVersion { get; set; }
        public int ParentSequence { get; set; }
        public bool? IsDefault { get; set; }

        #endregion
    }
}