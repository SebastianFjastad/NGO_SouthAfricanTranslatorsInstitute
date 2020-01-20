
namespace SATI.Entities
{
    public class PaymentCategory
    {
        public int PaymentCategoryId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; }

    }
}