using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MEmployeeGroup : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string EmployeeGroupID { get; set; }
        public string EmployeeGroupDesc { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}