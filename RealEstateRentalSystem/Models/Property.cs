using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateRentalSystem.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string PropertyType { get; set; }
        public int Rooms { get; set; }
        public decimal Area { get; set; }
        public int? Floor { get; set; }
        public decimal PricePerMonth { get; set; }
        public bool IsRented { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Отображаемый статус для UI
        public string StatusText => IsRented ? "Арендована" : "Свободна";
    }
}

