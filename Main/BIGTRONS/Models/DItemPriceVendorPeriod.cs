using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DItemPriceVendorPeriod : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemPriceID { get; set; }
        [Key, Column(Order = 2)]
        public string VendorID { get; set; }
        [Key, Column(Order = 3)]
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public string TaskID { get; set; }

        #endregion
    }
}