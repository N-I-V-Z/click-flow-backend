using AutoMapper;
using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.DTOs.ConversationDTOs;
using ClickFlow.BLL.DTOs.MessageDTOs;
using ClickFlow.BLL.Helpers.Fillters;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class ConversationService : IConversationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		protected virtual QueryBuilder<Conversation> CreateQueryBuilder(string? search = null)
		{
			var queryBuilder = new QueryBuilder<Conversation>()
								.WithTracking(false);

			if (!string.IsNullOrEmpty(search))
			{
				var predicate = FilterHelper.BuildSearchExpression<Conversation>(search);
				queryBuilder.WithPredicate(predicate);
			}

			return queryBuilder;
		}

		public async Task<ConversationResponseDTO> GetOrCreateAsync(int user1Id, int user2Id)
		{
			try
			{
				var conRepo = _unitOfWork.GetRepo<Conversation>();

				var queryBuilder = CreateQueryBuilder()
					.WithPredicate(x => (x.User1Id == user1Id && user2Id == user2Id) || (x.User1Id == user2Id && x.User2Id == user1Id))
					.WithInclude(x => x.User1, x => x.User2);

				var conData = await conRepo.GetSingleAsync(queryBuilder.Build());

				if (conData != null) return _mapper.Map<ConversationResponseDTO>(conData);

				var newCon = new Conversation
				{
					User1Id = user1Id,
					User2Id = user2Id,
					CreatedAt = DateTime.UtcNow
				};

				await conRepo.CreateAsync(newCon);
				var saver = await _unitOfWork.SaveAsync();
				if (!saver)
				{
					return null;
				}
				return _mapper.Map<ConversationResponseDTO>(newCon);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<List<ConversationResponseListDTO>> GetUserConversationsAsync(int userId)
		{
			try
			{
				var conRepo = _unitOfWork.GetRepo<Conversation>();
				var queryBuilder = CreateQueryBuilder()
					.WithPredicate(x => x.User1Id == userId || x.User2Id == userId)
					.WithInclude(x => x.Messages, x => x.User2, x => x.User1, x => x.User1.UserDetail, x => x.User2.UserDetail);
				var conData = await conRepo.GetAllAsync(queryBuilder.Build());

				return conData.Select(conversation => {
					var partner = conversation.User1Id == userId ? conversation.User2 : conversation.User1;
					var partnerId = conversation.User1Id == userId ? conversation.User2Id : conversation.User1Id;

					var lastMessage = conversation.Messages?
						.OrderByDescending(m => m.SentAt)
						.FirstOrDefault();

					var unreadCount = conversation.Messages?
						.Count(m => m.SenderId != userId && m.IsRead == false) ?? 0;

					return new ConversationResponseListDTO
					{
						Id = conversation.Id,
						ParnerId = partnerId,
						Parner = _mapper.Map<ApplicationUserResponseDTO>(partner),
						LastMessage = lastMessage != null ? _mapper.Map<MessageResponseDTO>(lastMessage) : null,
						UnreadCount = unreadCount
					};
				}).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}

		public async Task<ConversationResponseDTO> GetConversasionByIdAsync(int conversationId)
		{
			try
			{
				var conRepo = _unitOfWork.GetRepo<Conversation>();
				var queryBuilder = CreateQueryBuilder()
					.WithPredicate(x => x.Id == conversationId)
					.WithInclude(x => x.Messages, x => x.User2, x => x.User1);
				var conData = await conRepo.GetSingleAsync(queryBuilder.Build());

				return _mapper.Map<ConversationResponseDTO>(conData);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}
}
