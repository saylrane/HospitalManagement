using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Medicine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }


          public ICollection<Inventory> Inventory { get; set; } = new List<Inventory>();

    }
}
