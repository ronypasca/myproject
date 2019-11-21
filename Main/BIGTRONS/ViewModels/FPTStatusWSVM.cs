using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace com.SML.BIGTRONS.ViewModels
{
    public class FPTStatusWSVM
    {
        [Key, Column(Order = 1)]
        public string FPTStatusID { get; set; }
        [Key, Column(Order = 2)]
        public string FPTID { get; set; }
        public DateTime StatusDateTimeStamp { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public string StatusDesc { get; set; }
        
    }
    public class FPTStatusUpdateWSVM
    {
       [Key, Column(Order = 1)]
        public string FPTID { get; set; }
        public DateTime StatusDateTimeStamp { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }

    }
}