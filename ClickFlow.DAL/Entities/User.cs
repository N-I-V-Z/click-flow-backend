using ClickFlow.DAL.Enums;

namespace ClickFlow.DAL.Entities
{
    public class User 
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public Role Role {  get; set; }
        public Gender Gender { get; set; }
        public string Password { get; set; }
        public string Email {  get; set; }
        public string PhoneNumber {  get; set; }
        public string AvatarURL {  get; set; }
        public bool IsDeleted {  get; set; }
        public int? AdvertiserId {  get; set; }
        public int? PublisherId {  get; set; }
        public int? WalletId {  get; set; }
        public Advertiser? Advertiser { get; set; }
        public Publisher? Publisher { get; set; }
        public Wallet? Wallet { get; set; }

        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }

    }
}
