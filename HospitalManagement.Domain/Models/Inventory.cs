using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Inventory
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }
        public Medicine Medicine { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    } 
}
