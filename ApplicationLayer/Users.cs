using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Users : BaseHandler<UserItem>
    {
        public Users() : base("users")
        {

        }

        public async Task<bool> Login(UserItem newUser)
        {
            List<UserItem> tableResult = await this.GetItemsByColumn("userName", newUser.UserName);

            return tableResult?.Any((t) => t.Password == newUser.Password) == true;
        }
    }
}
