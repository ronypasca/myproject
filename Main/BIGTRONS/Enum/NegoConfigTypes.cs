using System.ComponentModel;

namespace com.SML.BIGTRONS.Enum
{
    public enum NegoConfigTypes
    {
       
        [Description("CT001")]
        Round,

        [Description("CT002")]
        Schedule,

        [Description("CT003")]
        BudgetPlan,

        [Description("CT004")]
        RoundTime,

        [Description("CT005")]
        SubItemLevel,

        [Description("CT006")]
        TCMember,

        [Description("CT007")]
        TRMLead,

        [Description("CT008")]
        Vendor,

        [Description("CT009")]
        Source,

    }

    public enum NegoConfigSource {
        [Description("Upload")]
        Upload,
        [Description("Budget Plan")]
        BudgetPlan,
        [Description("Cart")]
        Cart
    }
}