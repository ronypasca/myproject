using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MPersonnelSubarea : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string PersonnelAreaID { get; set; }
        [Key, Column(Order = 2)]
        public string PersonnelSubareaID { get; set; }
        public string PersonnelSubareaDesc { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}