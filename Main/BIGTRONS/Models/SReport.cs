using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class SReport : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ReportID { get; set; }
        public string ReportDesc { get; set; }
        public string ReportViewName { get; set; }
        public bool ReportVisible { get; set; }

        #endregion
    }
}