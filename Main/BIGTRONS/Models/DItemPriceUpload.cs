using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DItemPriceUpload : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemPriceUploadID { get; set; }
        public string ItemPriceID { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string RegionID { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public string UnitTypeID { get; set; }
        public string PriceTypeID { get; set; }
        public string VendorID { get; set; }
        public bool IsDefault { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public string TaskID { get; set; }

        #endregion
    }
}