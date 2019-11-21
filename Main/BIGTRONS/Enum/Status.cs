using System.ComponentModel;

namespace com.SML.BIGTRONS.Enum
{
    public enum BudgetPlanVersionStatus
    {
        [Description("0")]
        Draft = 0,

        [Description("1")]
        Submitted = 1,

        [Description("2")]
        Approved = 2,

        [Description("99")]
        Deleted = 99
    }

    public enum BudgetPlanVersionPeriodStatus
    {
        [Description("0")]
        Open,

        [Description("1")]
        Close
    }

    public enum TaskDetailStatus
    {
        Revise = 0,
        Clarify = 1,
        Approve = 2,
        Reject = 3,
        Draft = 4,
        Submit = 5
    }

    public enum TaskStatus
    {
        [Description("In Progress")]
        InProgress = 0,
        [Description("Revise")]
        Revise = 1,
        [Description("Approved")]
        Approved = 2,
        [Description("Rejected")]
        Rejected = 3,
        [Description("Draft")]
        Draft = 4
    }

    public enum FPTStatusTypes
    {
        New = 1,
        PreTender,
        ManageVendor,
        Aanwijzing,
        VendorEntry,
        BidOpening,
        Clarification,
        NegotiationConfiguration,
        SubmittedforConfiguration,
        ApprovedforConfiguration,
        Negotiation,
        DoneNegotiation,
        ReNegotiation,
        StopNegotiation,
        ReTender,
        VendorRecommendation,
        SubmitVendorWinner,
        FPTUnverified,
        FPTVerified,
        VendorUnverified,
        VendorVerified,
        SubmittedforSPApproval,
        ApprovedVendorSP,
        FPTPark,
        FPTUnpark,
        Deleted = 99,
    }
    public enum ScheduleStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Draft_Reschedule = 3,
        Draft_Cancellation = 4,
        Submitted_Reschedule = 5,
        Submitted_Cancellation = 6,
        Rescheduled = 7,
        Cancelled = 99

    }
    public enum NotificationStatus
    {
        Draft = 0,
        Verified = 1,
        Sent = 2,
        Fail = 3,
        Deleted = 99

    }

    public enum MinutesStatus
    {
        Draft = 0,
        Verified = 1,
        Approved = 2,
        Deleted = 99
    }

    public enum CartStatus
    {
        [Description("0")]
        Draft = 0,

        [Description("1")]
        Submitted = 1,

        [Description("2")]
        Approved = 2,

        [Description("99")]
        Deleted = 99
    }
}