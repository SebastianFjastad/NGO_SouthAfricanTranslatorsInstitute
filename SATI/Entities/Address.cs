using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using SATI.Models;

namespace SATI.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        [DisplayName("Street 1")]
        public string Line1 { get; set; }
        [DisplayName("Street 2")]
        public string Line2 { get; set; }
        [DisplayName("Suburb")]
        public string Line3 { get; set; }
        [DisplayName("Town/City")]
        public string Town { get; set; }
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        public User User { get; set; }
        public override string ToString()
        {
            string[] elements = {Line1, Line2, Line3, Town, PostalCode};
            return string.Join(",", elements.Where(e => !string.IsNullOrEmpty(e)));
        }  
    }
}