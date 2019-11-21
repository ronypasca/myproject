using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DVendorCommunications : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorCommID { get; set; }
        public string CommunicationTypeID { get; set; }
        public string VendorPICID { get; set; }
        public bool IsDefault { get; set; }
        public string CommDesc { get; set; }

        #endregion
    }
}