﻿using ClickFlow.BLL.DTOs.VnPayDTOs;
using ClickFlow.BLL.Helpers.Config;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Queries;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace ClickFlow.BLL.Services.Implements
{
	public class VnPayService(VnPayConfiguration vnPayConfig, IUnitOfWork unitOfWork) : IVnPayService
	{
		private readonly VnPayConfiguration _vnPayConfig = vnPayConfig;
		private readonly IUnitOfWork _unitOfWork = unitOfWork;

		public async Task<string> CreatePaymentUrlAsync(int userId, HttpContext context, VnPayRequestDTO vnPayRequest)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var walletRepo = _unitOfWork.GetRepo<Wallet>();
				var wallet = walletRepo.GetSingleAsync(
					new QueryBuilder<Wallet>()
						.WithPredicate(w => w.UserId == userId)
						.WithTracking(false)
						.Build())
					.GetAwaiter().GetResult();

				if (wallet == null)
					throw new Exception("Ví không tồn tại.");

				var transaction = new Transaction
				{
					WalletId = wallet.Id,
					TransactionType = TransactionType.Deposit,
					Amount = (int)vnPayRequest.Amount,
					Status = false, // pending
					PaymentDate = DateTime.UtcNow,
					Balance = wallet.Balance
				};

				var transactionRepo = _unitOfWork.GetRepo<Transaction>();
				await transactionRepo.CreateAsync(transaction);
				await _unitOfWork.SaveAsync();

				await _unitOfWork.CommitTransactionAsync();

				var vnpay = new VnPayLibrary();

				vnpay.AddRequestData("vnp_Version", _vnPayConfig.Version);
				vnpay.AddRequestData("vnp_Command", _vnPayConfig.Command);
				vnpay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
				vnpay.AddRequestData("vnp_Amount", (vnPayRequest.Amount * 100).ToString());

				Console.BackgroundColor = ConsoleColor.Green;
				Console.WriteLine(vnPayRequest.Amount);
				Console.ResetColor();

				vnpay.AddRequestData("vnp_CreateDate", vnPayRequest.CreatedDate.ToString("yyyyMMddHHmmss"));
				vnpay.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrCode);
				vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
				vnpay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);

				vnpay.AddRequestData("vnp_OrderInfo", "Nạp tiền vào ClickFlow mã: " + vnPayRequest.OrderId);
				vnpay.AddRequestData("vnp_OrderType", "other");
				vnpay.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
				vnpay.AddRequestData("vnp_TxnRef", vnPayRequest.OrderId.ToString());

				var paymentUrl = vnpay.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);

				return paymentUrl;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public VnPayResponseDTO PaymentExcute(IQueryCollection collection)
		{
			var vnpay = new VnPayLibrary();


			foreach (var (key, value) in collection)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value.ToString());
				}
			}

			var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
			var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
			var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
			var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
			var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

			bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnPayConfig.HashSecret);

			if (!checkSignature)
			{
				return new VnPayResponseDTO { IsSuccess = false, VnPayResponseCode = vnp_ResponseCode, Amount = 0 };
			}

			if (!long.TryParse(vnp_Amount, out var rawAmount))
			{
				rawAmount = 0;
			}
			var actualAmount = rawAmount / 100.0;

			var result = new VnPayResponseDTO
			{

				IsSuccess = true,
				PaymentMethod = "VnPay",
				OrderDescription = vnp_OrderInfo,
				OrderId = vnp_orderId.ToString(),
				TransactionId = vnp_TransactionId,
				Token = vnp_SecureHash,
				VnPayResponseCode = vnp_ResponseCode,
				Amount = actualAmount,
			};

			return result;
		}
	}
}
