using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MUser : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string UserID { get; set; }
        //public string RoleID { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string VendorID { get; set; }
        public string EmployeeID { get; set; }
        public string BusinessUnitID { get; set; }
        public string DivisionID { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public DateTime LastLogin { get; set; }
        public string HostIP { get; set; }
        public bool IsActive { get; set; }

        #endregion
    }
}