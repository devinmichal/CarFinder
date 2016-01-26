using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace carfinder.Models
{
    public class CarViewModel
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Year { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
        public string Link { get; set; }
        
    }
}