using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DEmpOrgAss : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string EmployeeID { get; set; }
        public string PersonnelAreaID { get; set; }
        public string PersonnelSubareaID { get; set; }
        public string EmployeeGroupID { get; set; }
        public string EmployeeSubgroupID { get; set; }
        public string WorkContractID { get; set; }
        public string PositionID { get; set; }
        public string Grade { get; set; }
        public int Level { get; set; }
        public string SupervisorID { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}