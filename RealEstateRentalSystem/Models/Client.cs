using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateRentalSystem.Models
{
 
        public class Client
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string PassportData { get; set; }
            public bool IsActive { get; set; }
            public DateTime RegisteredAt { get; set; }

            public string StatusText => IsActive ? "Активен" : "Неактивен";
        }
    }

