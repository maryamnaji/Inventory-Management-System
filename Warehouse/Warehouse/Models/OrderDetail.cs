using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Models
{
    public class OrderDetail
    {
        [Key]
        public int ID { get; set; }

        //Relations
        [Required, ForeignKey("Product")]
        public int ProductId { get; set; }   
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

    }
}
