using System;
using SATI.Models;

namespace SATI.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Comment { get; set; }
        public int FinancialYear { get; set; }
        public CardPaymentType? CardPaymentType { get; set; }

        public int PaymentCategoryId { get; set; }
        public PaymentCategory PaymentCategory { get; set; }

        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}