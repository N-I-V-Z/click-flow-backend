using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.DTOs.ConversationDTOs
{
    public class ConversationResponseDTO
    {
        public int Id { get; set; }

        public int User1Id { get; set; }
        public ApplicationUser User1 { get; set; }

        public int User2Id { get; set; }
        public ApplicationUser User2 { get; set; }

        public DateTime? CreatedAt { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
