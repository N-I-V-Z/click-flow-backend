using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.DAL.Entities;

namespace ClickFlow.BLL.Services.Interfaces
{
	public interface IConversationService
	{
		Task<ConversationResponseDTO> GetOrCreateAsync(int user1Id, int user2Id);
		Task<List<ConversationResponseListDTO>> GetUserConversationsAsync(int userId);
		Task<ConversationResponseDTO> GetConversasionByIdAsync(int conversationId);
	}
}
