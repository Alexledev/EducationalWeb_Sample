﻿
namespace EducationalWeb_Sample.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime PostDate { get; set; }
        public string Category { get; set; }
        public string? Author { get; set; }
        public string ImageURL { get; set; }
    }
}
