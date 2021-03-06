using System;
using System.Web;

namespace com.SML.BIGTRONS.Models
{
	public abstract class BaseModel
	{
		#region Public Property

		public string CreatedBy { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string CreatedHost { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string ModifiedHost { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor for BaseModel class
		/// </summary>
		public BaseModel()
		{
			DateTime m_dateNow = DateTime.Now;
			CreatedBy = HttpContext.Current.User.Identity.Name;
			CreatedDate = m_dateNow;
			CreatedHost = Global.LocalHostName;
			ModifiedBy = HttpContext.Current.User.Identity.Name;
			ModifiedDate = m_dateNow;
			ModifiedHost = Global.LocalHostName;
		}

		#endregion

		public void PrepareModifiedInfo()
		{
			ModifiedBy = HttpContext.Current.User.Identity.Name;
			ModifiedDate = DateTime.Now;
			ModifiedHost = Global.LocalHostName;
		}
	}
}