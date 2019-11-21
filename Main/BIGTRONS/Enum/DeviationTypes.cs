using System.ComponentModel;

namespace com.SML.BIGTRONS.Enum
{
    public enum DeviationTypes
    {
        [Description("DV001")]
        FPTVerification,
        [Description("DV002")]
        PreTender,
        [Description("DV003")]
        Aanwijzing,
        [Description("DV004")]
        VendorClarification,
        [Description("DV005")]
        VendorNegotiationSelection,
        [Description("DV006")]
        ReNegotiation,
        [Description("DV007")]
        StopNegotiation,
        [Description("DV008")]
        ReTender,
    }
}