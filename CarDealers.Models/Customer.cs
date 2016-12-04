
namespace CarDealers.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Customer
    {

        public Customer()
        {
            this.Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
      
        public DateTime DateOfBirth { get; set; }

        public bool isYoungDriver { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
