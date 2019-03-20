using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Product
    {
        
       
            public int Id { get; set; }
            public string SiteId { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Brand { get; set; }
            public double Price { get; set; }
            public double UsdPrice { get; set; }
            public int Quantity { get; set; }
            public string Image { get; set; }
            public string Url { get; set; }
            public string Status { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
        
    }
}
