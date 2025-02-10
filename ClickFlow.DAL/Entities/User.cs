
using Microsoft.AspNetCore.Identity;

namespace ClickFlow.DAL.Entities
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
    }
}
