using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MPosition : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string PositionID { get; set; }
        public string PositionAltID { get; set; }
        public string PositionDesc { get; set; }
        [Key, Column(Order = 2)]
        public DateTime StartDate { get; set; }
        [Key, Column(Order = 3)]
        public DateTime EndDate { get; set; }
        public string OrganizationID { get; set; }
        public string OrganizationAltID { get; set; }
        public string OrganizationDesc { get; set; }
        public string OrgSectionID { get; set; }
        public string OrgSectionAltID { get; set; }
        public string OrgSectionDesc { get; set; }
        public string OrgDepartmentID { get; set; }
        public string OrgDepartmentAltID { get; set; }
        public string OrgDepartmentDesc { get; set; }
        public string OrgDivisionID { get; set; }
        public string OrgDivisionAltID { get; set; }
        public string OrgDivisionDesc { get; set; }
        public string OrgBusinessUnitID { get; set; }
        public string OrgBusinessUnitAltID { get; set; }
        public string OrgBusinessUnitDesc { get; set; }
        public string OrgGCEOID { get; set; }
        public string OrgGCEOAltID { get; set; }
        public string OrgGCEODesc { get; set; }
        public string OrgChairmanID { get; set; }
        public string OrgChairmanAltID { get; set; }
        public string OrgChairmanDesc { get; set; }
        public string HierSectionID { get; set; }
        public string HierSectionAltID { get; set; }
        public string HierSectionDesc { get; set; }
        public string HierDepartmentID { get; set; }
        public string HierDepartmentAltID { get; set; }
        public string HierDepartmentDesc { get; set; }
        public string HierDivisionID { get; set; }
        public string HierDivisionAltID { get; set; }
        public string HierDivisionDesc { get; set; }
        public string HierBusinessUnitID { get; set; }
        public string HierBusinessUnitAltID { get; set; }
        public string HierBusinessUnitDesc { get; set; }
        public string HierGCEOID { get; set; }
        public string HierGCEOAltID { get; set; }
        public string HierGCEODesc { get; set; }
        public string HierChairmanID { get; set; }
        public string HierChairmanAltID { get; set; }
        public string HierChairmanDesc { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}