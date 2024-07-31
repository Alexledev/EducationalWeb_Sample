using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CourseItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public float CourseLength { get; set; }
        public int Students { get; set; }
        public float Rating { get; set; }
        public decimal Price { get; set; }
        public string Topic { get; set; }
        public DateTime PostDate { get; set; }
        public string ImageURL { get; set; }

    }
}
