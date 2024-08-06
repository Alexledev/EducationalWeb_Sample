using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Blogs : BaseHandler<BlogItem>
    {
        public Blogs() : base("blogs")
        {

        }
    }
}
