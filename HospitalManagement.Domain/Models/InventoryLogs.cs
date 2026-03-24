using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class InventoryLogs
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }
        public Medicine Medicine { get; set; }
         
        public int QuantityChanged { get; set; }
        public string ActionType { get; set; } //Manual update/ Dispensed
        public string Remarks { get; set; }
    } 
}
