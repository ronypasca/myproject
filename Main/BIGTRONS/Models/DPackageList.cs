using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DPackageList : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string PackageID { get; set; }
        [Key, Column(Order = 2)]
        public string BudgetPlanID { get; set; }
        [Key, Column(Order = 3)]
        public int BudgetPlanVersion { get; set; }

        #endregion
    }
}