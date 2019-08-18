using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(50)]
        public string  Name { get; set; }

        [ StringLength(50)]
        public string Address { get; set; }

        [ StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string  Tel { get; set; }

        [Required,StringLength(50)]
        public string Email   { get; set; }

        [Required]
        public string Password   { get; set; }

        public DateTime? LastLoginDate   { get; set; }
    }
}
