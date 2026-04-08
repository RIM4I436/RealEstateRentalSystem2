using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateRentalSystem.Models
{
    public class RentalContract
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyAddress { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

  
        public int DurationDays => (EndDate - StartDate).Days;

       
        public decimal TotalCost => MonthlyPayment * (DurationDays / 30);
    }
    }
