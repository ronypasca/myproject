using System.ComponentModel;

namespace com.SML.BIGTRONS.Enum
{
    public enum TaskType
    {
       
        [Description("TT0001")]
        TCMember,

        [Description("TT0002")]
        NegotiationConfigurations,

        [Description("TT0003")]
        VendorWinner,

        [Description("TT0004")]
        Invitation_Schedules,

        [Description("TT0005")]
        MailNotification,

        [Description("TT0006")]
        MinutesOfMeeting,

        [Description("TT0007")]
        Invitation_Pretender,

        [Description("TT0008")]
        UploadItem


    }
}