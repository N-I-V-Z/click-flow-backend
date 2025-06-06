using AutoMapper;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.DTOs.UserDetailDTOs;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;

namespace ClickFlow.BLL.Services.Implements
{
    public class UserDetailService : IUserDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse> CreateUpdateUserDetail(UserDetailRequestDTO dto, int userId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<UserDetail>();
                await _unitOfWork.BeginTransactionAsync();

                var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
                                                .WithPredicate(x => x.ApplicationUserId == userId)
                                                .Build());
                if (any)
                {
                    var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
                                                                .WithPredicate(x => x.ApplicationUserId == userId)
                                                                .Build());
                    if (userId != userDetail.ApplicationUserId) return new BaseResponse { IsSuccess = false, Message = "Người dùng không khớp." };

                    var updateUserDetail = _mapper.Map(dto, userDetail);
                    await repo.UpdateAsync(updateUserDetail);
                }
                else
                {
                    var userDetail = _mapper.Map<UserDetail>(dto);
                    userDetail.ApplicationUserId = userId;
                    await repo.CreateAsync(userDetail);
                }
                var saver = await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
                if (!saver) return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
                return new BaseResponse { IsSuccess = true, Message = "Lưu dữ liệu thành công" };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<BaseResponse> DeleteUserDetail(int userId)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var any = await repo.AnyAsync(new QueryBuilder<UserDetail>()
                                            .WithPredicate(x => x.ApplicationUserId == userId)
                                            .Build());
            if (any)
            {
                var userDetail = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
                                                            .WithPredicate(x => x.ApplicationUserId == userId)
                                                            .Build());
                if (userId != userDetail.ApplicationUserId) return new BaseResponse { IsSuccess = false, Message = "Người dùng không khớp." };
                await repo.DeleteAsync(userDetail);
                var saver = await _unitOfWork.SaveAsync();
                if (!saver) return new BaseResponse { IsSuccess = false, Message = "Xóa dữ liệu thất bại" };
                return new BaseResponse { IsSuccess = true, Message = "Xóa dữ liệu thành công" };
            }

            return new BaseResponse { IsSuccess = false, Message = "Không tồn tại người dùng." };
        }

        public async Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetails(int pageIndex, int pageSize)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
                                        .WithPredicate(x => true)
                                        .Build());
            var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
            var resultDTO = _mapper.Map<List<UserDetailResponseDTO>>(pagedRecords);
            return new PaginatedList<UserDetailResponseDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<UserDetailResponseDTO>> GetAllUserDetailsByName(int pageIndex, int pageSize, string? name)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var loadedRecords = repo.Get(new QueryBuilder<UserDetail>()
                                        .WithPredicate(x => true)
                                        .Build());
            if (!string.IsNullOrEmpty(name))
            {
                loadedRecords = loadedRecords.Where(x => x.User.FullName.Contains(name));
            }
            var pagedRecords = await PaginatedList<UserDetail>.CreateAsync(loadedRecords, pageIndex, pageSize);
            var resultDTO = _mapper.Map<List<UserDetailResponseDTO>>(pagedRecords);
            return new PaginatedList<UserDetailResponseDTO>(resultDTO, pagedRecords.TotalItems, pageIndex, pageSize);
        }

        public async Task<UserDetailResponseDTO> GetUserDetailByUserId(int userId)
        {
            var repo = _unitOfWork.GetRepo<UserDetail>();
            var response = await repo.GetSingleAsync(new QueryBuilder<UserDetail>()
                                                    .WithPredicate(x => x.ApplicationUserId == userId)
                                                    .WithTracking(false)
                                                    .Build());
            if (response == null) return null;
            return _mapper.Map<UserDetailResponseDTO>(response);
        }
    }
}
