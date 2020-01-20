
namespace SATI.Models
{
    public enum MemberType
    {
        Individual = 1,
        Company = 2
    }

    public enum AvailabilityType
    {
        FullTime = 1,
        PartTime = 2,
        None = 3
    }

    public enum CardPaymentType
    {
        Credit = 1,
        Debit = 2
    }

    public enum Role
    {
        Admin = 1,
        Member,
        Public,
        AccountingSystem
    }

    public enum TabType
    {
        Details = 1,
        General, 
        Capabilities, 
        Languages,
        Payments
    }
}