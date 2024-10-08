﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Courses : BaseHandler<CourseItem>
    {
        public Courses() : base("courses")
        {

        }

        public Task<List<CourseItem>> FullTextSearch(string searchText)
        {
            return base.FullTextSearchWithColumn("Title", searchText);
        }
    }
}
