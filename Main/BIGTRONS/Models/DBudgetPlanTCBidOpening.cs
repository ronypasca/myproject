using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DBudgetPlanTCBidOpening : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
        public string BPTCBidOpeningID { get; set; }
        public string BPBidOpeningID { get; set; }
        public string TCMemberID { get; set; }

		#endregion
	}
}