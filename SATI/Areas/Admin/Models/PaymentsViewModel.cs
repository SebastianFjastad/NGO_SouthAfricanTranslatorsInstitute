using System.Collections.Generic;
using System.Linq;
using SATI.Entities;

namespace SATI.Areas.Admin.Models
{
    public class PaymentsViewModel
    {
        public PaymentsViewModel(List<Payment> payments)
        {
            PaymentCategories = new List<PaymentCategory>();
            PaymentMethods = new List<PaymentMethod>();
            Payments = payments.OrderByDescending(p => p.PaymentDate).ToList();
        }

        public string MemberId { get; set; }
        public List<PaymentCategory> PaymentCategories { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public List<Payment> Payments { get; set; }
    }
}