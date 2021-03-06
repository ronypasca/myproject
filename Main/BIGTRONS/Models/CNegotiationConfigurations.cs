using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class CNegotiationConfigurations : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationConfigID { get; set; }
        public string NegotiationConfigTypeID { get; set; }
        public string FPTID { get; set; }
        public string TaskID { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterValue2 { get; set; }

        #endregion
    }
}