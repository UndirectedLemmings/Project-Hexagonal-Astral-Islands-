using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Hexagonal_Astral_Islands.Models
{
    public class PrimitiveMapViewModel
    {
      public string Data { get; set; }

        public PrimitiveMapViewModel(string data)
        {
            Data = data;
        }
    }
}
