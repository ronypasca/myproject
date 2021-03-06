using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DTaskDetails : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string TaskDetailID { get; set; }
        public string TaskID { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }

        #endregion
    }
}