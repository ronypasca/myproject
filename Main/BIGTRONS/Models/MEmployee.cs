using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MEmployee : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string Gender { get; set; }
        public string LanguageID { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string BirthCountry { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string BloodType { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime PermanentDate { get; set; }
        public DateTime ResignDate { get; set; }
        public string RemunerationType { get; set; }
        public string CreatedHostName { get; set; }
        public string ModifiedHostName { get; set; }

        #endregion
    }
}