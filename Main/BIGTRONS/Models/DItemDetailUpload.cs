using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DItemDetailUpload : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
        public string ItemDetailUploadID { get; set; }
        public string ItemDetailID { get; set; }
		public string ItemDetailDesc { get; set; }
		public string ItemDetailTypeID { get; set; }
        public string ItemDetailTypeDesc { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string VendorID { get; set; }
        public string TaskID { get; set; }
        #endregion
    }
}