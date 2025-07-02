using AutoMapper;
using ClickFlow.BLL.DTOs.MessageDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
	public class MessageService : BaseServices<Message, MessageResponseDTO>, IMessageService
	{

		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICloudinaryService _cloudinaryService;

		public MessageService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService) : base(unitOfWork, mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_cloudinaryService = cloudinaryService;
		}

		public async Task<List<MessageResponseDTO>> GetMessagesAsync(int conversationId)
		{
			var messageRepo = _unitOfWork.GetRepo<Message>();
			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(x => x.ConversationId == conversationId).WithOrderBy(x => x.OrderBy(x => x.SentAt))
				.WithInclude(x => x.Sender, x => x.Conversation);

			var messageData = await messageRepo.GetAllAsync(queryBuilder.Build());
			return _mapper.Map<List<MessageResponseDTO>>(messageData);
		}

		public async Task MarkMessagesAsReadAsync(int conversationId, int readerId)
		{
			var messageRepo = _unitOfWork.GetRepo<Message>();
			var queryBuilder = CreateQueryBuilder()
				.WithPredicate(x => x.ConversationId == conversationId && x.SenderId != readerId && !x.IsRead);

			var unreadMessages = await messageRepo.GetAllAsync(queryBuilder.Build());

			if (unreadMessages.Any())
			{
				foreach (var message in unreadMessages)
				{
					message.IsRead = true;
					await messageRepo.UpdateAsync(message);
				}

				await _unitOfWork.SaveChangesAsync();
			}
		}

		public async Task<MessageResponseDTO> SendMessageAsync(int senderId, MessageSendDTO dto)
		{
			string fileUrl = null;
			if (dto.File != null)
			{
				var uploadedFile = await _cloudinaryService.UploadImageAsync(dto.File);
				fileUrl = uploadedFile.Url.ToString();

			}

			var newMessage = _mapper.Map<Message>(dto);
			newMessage.SenderId = senderId;
			newMessage.FileUrl = fileUrl;
			newMessage.SentAt = DateTime.Now;

			var messageRepo = _unitOfWork.GetRepo<Message>();
			await messageRepo.CreateAsync(newMessage);

			var saver = await _unitOfWork.SaveAsync();
			if (!saver)
			{
				return null;
			}
			return _mapper.Map<MessageResponseDTO>(newMessage);
		}
	}
}
