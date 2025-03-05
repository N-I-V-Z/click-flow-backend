using ClickFlow.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
        public int? AdvertiserId { get; set; }
        public int? PublisherId { get; set; }
        public int? WalletId { get; set; }

        // Navigation properties
        public Advertiser? Advertiser { get; set; }
        public Publisher? Publisher { get; set; }
        public Wallet? Wallet { get; set; }
    }
}
