using System.ComponentModel;

namespace com.SML.BIGTRONS.Enum
{
    public enum VendorBidTypes
    {
        [Description("VB001")]
        GrandTotal=7777,
        [Description("VB002")]
        SubItem,
        [Description("VB003")]
        LastOffer,
        [Description("VB004")]
        Fee=8888,
        [Description("VB005")]
        AfterFee=9999
    }

    public enum AdditionalInfoItems
    {
        ScheduleStart = 1,
        ScheduleEnd = 2,
        Duration = 3,
        MaintenancePeriod = 4,
        Guarantee = 5,
        ContractType = 6,
        PaymentMethod = 7,
        P3 = 8,
        CompanyID = 9,
        TenderType = 10,
        ScheduleStartManual = 11,
        ScheduleEndManual = 12,
        DurationManual = 13,
    }
}