using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FincaFenix.Entities.DTOs
{
    public class MaterialOrderDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public char? Unit { get; set; }
    }
}
