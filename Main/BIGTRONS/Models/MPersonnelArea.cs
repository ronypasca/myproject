using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MPersonnelArea : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string PersonnelAreaID { get; set; }
        public string CompanyID { get; set; }
        public string PersonnelAreaDesc { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}