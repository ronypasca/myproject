using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DItemComparisonSession : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region public property
        [Key, Column(Order = 1)]

        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemSecondaryName { get; set; }
        public string SessionID { get; set; }
        public string GroupGUID { get; set; }
        public string ComparisonDetilID { get; set; }
        #endregion

    }
}