using AutoMapper;
using ClickFlow.BLL.DTOs.FeedbackDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> CreateFeedback(FeedbackCreateDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Feedback>();

                var feedback = _mapper.Map<Feedback>(dto);
                feedback.Timestamp = DateTime.Now;

                await repo.CreateAsync(feedback);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Phản hồi đã được tạo thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> UpdateFeedback(FeedbackUpdateDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Feedback>();

                var feedback = await repo.GetSingleAsync(new QueryBuilder<Feedback>()
                    .WithPredicate(x => x.Id == dto.Id)
                    .WithTracking(false)
                    .Build());

                if (feedback == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Phản hồi không tồn tại." };
                }

                _mapper.Map(dto, feedback);
                await repo.UpdateAsync(feedback);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Phản hồi đã được cập nhật thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> DeleteFeedback(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.GetRepo<Feedback>();

                var feedback = await repo.GetSingleAsync(new QueryBuilder<Feedback>()
                    .WithPredicate(x => x.Id == id)
                    .WithTracking(false)
                    .Build());

                if (feedback == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Phản hồi không tồn tại." };
                }

                await repo.DeleteAsync(feedback);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                return new BaseResponse { IsSuccess = true, Message = "Phản hồi đã được xóa thành công." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<PaginatedList<FeedbackResponseDTO>> GetAllFeedbacks(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Feedback>();
            var feedbacks = repo.Get(new QueryBuilder<Feedback>()
                .WithInclude(x => x.Campaign, x => x.Feedbacker)
                .Build());

            var pagedFeedbacks = await PaginatedList<Feedback>.CreateAsync(feedbacks, pageIndex, pageSize);
            var result = _mapper.Map<List<FeedbackResponseDTO>>(pagedFeedbacks);
            return new PaginatedList<FeedbackResponseDTO>(result, pagedFeedbacks.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<FeedbackResponseDTO>> GetFeedbacksByCampaignId(int campaignId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Feedback>();
            var feedbacks = repo.Get(new QueryBuilder<Feedback>()
                .WithPredicate(x => x.CampaignId == campaignId)
                .WithInclude(x => x.Campaign, x => x.Feedbacker)
                .Build());

            var pagedFeedbacks = await PaginatedList<Feedback>.CreateAsync(feedbacks, pageIndex, pageSize);
            var result = _mapper.Map<List<FeedbackResponseDTO>>(pagedFeedbacks);
            return new PaginatedList<FeedbackResponseDTO>(result, pagedFeedbacks.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<FeedbackResponseDTO>> GetFeedbacksByFeedbackerId(int feedbackerId, int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<Feedback>();
            var feedbacks = repo.Get(new QueryBuilder<Feedback>()
                .WithPredicate(x => x.FeedbackerId == feedbackerId)
                .WithInclude(x => x.Campaign, x => x.Feedbacker)
                .Build());

            var pagedFeedbacks = await PaginatedList<Feedback>.CreateAsync(feedbacks, pageIndex, pageSize);
            var result = _mapper.Map<List<FeedbackResponseDTO>>(pagedFeedbacks);
            return new PaginatedList<FeedbackResponseDTO>(result, pagedFeedbacks.TotalItems, pageIndex, pageSize);
        }

        public async Task<FeedbackResponseDTO> GetFeedbackById(int id)
        {
            var repo = _unitOfWork.GetRepo<Feedback>();
            var feedback = await repo.GetSingleAsync(new QueryBuilder<Feedback>()
                .WithPredicate(x => x.Id == id)
                .WithInclude(x => x.Campaign, x => x.Feedbacker)
                .Build());

            if (feedback == null)
            {
                return null;
            }

            return _mapper.Map<FeedbackResponseDTO>(feedback);
        }
    }
}

