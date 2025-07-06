using ClickFlow.BLL.Helpers.Mapper;
using ClickFlow.DAL.EF;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Repositories;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ClickFlow.API.ConfigExtensions
{
	public static class ServiecsExtensions
	{
		//Unit Of Work
		public static void AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		//RepoBase
		public static void AddRepoBase(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));
		}

		//BLL Services
		public static void AddBLLServices(this IServiceCollection services)
		{
			services.Scan(scan => scan
					.FromAssemblies(Assembly.Load("ClickFlow.BLL"))
					.AddClasses(classes => classes.Where(type => type.Namespace == $"ClickFlow.BLL.Services.Implements" && type.Name.EndsWith("Service")))
					.AsImplementedInterfaces()
					.WithScopedLifetime());
		}

		// Auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}

		// Swagger
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "ClickFlow API", Version = "v1" });
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							}
						},
						new string[]{}
					}
				});
			});
		}

		//Seed Data
		public static async Task SeedData(this IServiceProvider serviceProvider)
		{
			using var context = new ClickFlowContext(serviceProvider.GetRequiredService<DbContextOptions<ClickFlowContext>>());

			// Lấy môi trường hiện tại (Development, Production)
			var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

			if (env.IsDevelopment())
			{
				await context.Database.EnsureDeletedAsync();
				await context.Database.EnsureCreatedAsync();
			}

			#region Seeding Roles
			if (!context.Roles.Any())
			{
				await context.Roles.AddRangeAsync(
					new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" },
					new IdentityRole<int> { Name = "Advertiser", NormalizedName = "ADVERTISER" },
					new IdentityRole<int> { Name = "Publisher", NormalizedName = "PUBLISHER" }
				);
				await context.SaveChangesAsync();
			}
			#endregion

			#region Seeding Admin
			if (!context.Users.Any(x => x.Role == Role.Admin))
			{
				// Pass: Admin@123
				var admin = new ApplicationUser { FullName = "admin", Role = Role.Admin, UserName = "admin", NormalizedUserName = "ADMIN", Email = "admin@email.com", NormalizedEmail = "ADMIN@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP", ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a", PhoneNumber = "0999999999", LockoutEnabled = true };
				await context.Users.AddAsync(admin);
				await context.SaveChangesAsync();
				await context.Wallets.AddAsync(new Wallet { Balance = 0, UserId = admin.Id });
				await context.SaveChangesAsync();
			}
			#endregion

			#region Seeding Plan
			if (!context.Plans.Any())
			{
				await context.Plans.AddAsync(new Plan
				{
					Name = "Free",
					MaxCampaigns = 2,
					MaxClicksPerMonth = 500,
					MaxConversionsPerMonth = 50,
					IsActive = true,
					Description = "Gói Free vĩnh viễn: giới hạn 2 campaign, 500 clicks và 50 conversions mỗi tháng.",
					Price = 0,
					DurationDays = null
				});

				await context.SaveChangesAsync();
			}
			#endregion

			if (env.IsDevelopment())
			{
				#region Seeding Users
				if (!context.Users.Any(x => x.Role == Role.Publisher && x.Role == Role.Advertiser))
				{
					await context.Users.AddRangeAsync(
						// Publishers - Pass: Publisher@123
						new ApplicationUser { FullName = "Nguyễn Văn Publisher", Role = Role.Publisher, UserName = "publisher", NormalizedUserName = "PUBLISHER", Email = "publisher@email.com", NormalizedEmail = "PUBLISHER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0988888888", LockoutEnabled = true },
						new ApplicationUser { FullName = "Trần Thị Minh", Role = Role.Publisher, UserName = "publisher2", NormalizedUserName = "PUBLISHER2", Email = "publisher2@email.com", NormalizedEmail = "PUBLISHER2@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0987654321", LockoutEnabled = true },
						new ApplicationUser { FullName = "Lê Hoàng Long", Role = Role.Publisher, UserName = "publisher3", NormalizedUserName = "PUBLISHER3", Email = "publisher3@email.com", NormalizedEmail = "PUBLISHER3@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0976543210", LockoutEnabled = true },
						new ApplicationUser { FullName = "Phạm Thúy Hằng", Role = Role.Publisher, UserName = "publisher4", NormalizedUserName = "PUBLISHER4", Email = "publisher4@email.com", NormalizedEmail = "PUBLISHER4@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0965432109", LockoutEnabled = true },
						new ApplicationUser { FullName = "Võ Minh Tuấn", Role = Role.Publisher, UserName = "publisher5", NormalizedUserName = "PUBLISHER5", Email = "publisher5@email.com", NormalizedEmail = "PUBLISHER5@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0954321098", LockoutEnabled = true },

						// Advertisers - Pass: Advertiser@123
						new ApplicationUser { FullName = "Công ty ABC", Role = Role.Advertiser, UserName = "advertiser", NormalizedUserName = "ADVERTISER", Email = "advertiser@email.com", NormalizedEmail = "ADVERTISER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0977777777", LockoutEnabled = true },
						new ApplicationUser { FullName = "Công ty XYZ Tech", Role = Role.Advertiser, UserName = "advertiser2", NormalizedUserName = "ADVERTISER2", Email = "advertiser2@email.com", NormalizedEmail = "ADVERTISER2@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0966666666", LockoutEnabled = true },
						new ApplicationUser { FullName = "Green Foods Ltd", Role = Role.Advertiser, UserName = "advertiser3", NormalizedUserName = "ADVERTISER3", Email = "advertiser3@email.com", NormalizedEmail = "ADVERTISER3@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0955555555", LockoutEnabled = true },
						new ApplicationUser { FullName = "Fashion House", Role = Role.Advertiser, UserName = "advertiser4", NormalizedUserName = "ADVERTISER4", Email = "advertiser4@email.com", NormalizedEmail = "ADVERTISER4@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0944444444", LockoutEnabled = true },
						new ApplicationUser { FullName = "Travel Vietnam", Role = Role.Advertiser, UserName = "advertiser5", NormalizedUserName = "ADVERTISER5", Email = "advertiser5@email.com", NormalizedEmail = "ADVERTISER5@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0933333333", LockoutEnabled = true }
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding UserRoles
				if (!context.UserRoles.Any())
				{
					await context.UserRoles.AddRangeAsync(
						// Admin
						new IdentityUserRole<int> { UserId = 1, RoleId = 1 },

						// Publishers
						new IdentityUserRole<int> { UserId = 2, RoleId = 3 },
						new IdentityUserRole<int> { UserId = 3, RoleId = 3 },
						new IdentityUserRole<int> { UserId = 4, RoleId = 3 },
						new IdentityUserRole<int> { UserId = 5, RoleId = 3 },
						new IdentityUserRole<int> { UserId = 6, RoleId = 3 },

						// Advertisers
						new IdentityUserRole<int> { UserId = 7, RoleId = 2 },
						new IdentityUserRole<int> { UserId = 8, RoleId = 2 },
						new IdentityUserRole<int> { UserId = 9, RoleId = 2 },
						new IdentityUserRole<int> { UserId = 10, RoleId = 2 },
						new IdentityUserRole<int> { UserId = 11, RoleId = 2 }
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Publishers
				if (!context.Publishers.Any())
				{
					await context.Publishers.AddRangeAsync(
						new Publisher { Id = 2, UserId = 2 },
						new Publisher { Id = 3, UserId = 3 },
						new Publisher { Id = 4, UserId = 4 },
						new Publisher { Id = 5, UserId = 5 },
						new Publisher { Id = 6, UserId = 6 }
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Advertisers
				if (!context.Advertisers.Any())
				{
					await context.Advertisers.AddRangeAsync(
						new Advertiser
						{
							Id = 7,
							CompanyName = "ABC Corporation",
							IntroductionWebsite = "https://abc-corp.com",
							StaffSize = 150,
							Industry = Industry.Education,
							UserId = 7
						},
						new Advertiser
						{
							Id = 8,
							CompanyName = "XYZ Tech Solutions",
							IntroductionWebsite = "https://xyz-tech.com",
							StaffSize = 85,
							Industry = Industry.Education,
							UserId = 8
						},
						new Advertiser
						{
							Id = 9,
							CompanyName = "Green Foods Limited",
							IntroductionWebsite = "https://greenfoods.vn",
							StaffSize = 45,
							Industry = Industry.FoodAndBeverage,
							UserId = 9
						},
						new Advertiser
						{
							Id = 10,
							CompanyName = "Fashion House Vietnam",
							IntroductionWebsite = "https://fashionhouse.vn",
							StaffSize = 75,
							Industry = Industry.Other,
							UserId = 10
						},
						new Advertiser
						{
							Id = 11,
							CompanyName = "Travel Vietnam Co.,Ltd",
							IntroductionWebsite = "https://travelvietnam.com",
							StaffSize = 30,
							Industry = Industry.Tourism,
							UserId = 11
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Wallets
				if (!context.Wallets.Any(x => x.ApplicationUser.Role == Role.Publisher || x.ApplicationUser.Role == Role.Advertiser))
				{
					await context.Wallets.AddRangeAsync(
						// Publishers wallets
						new Wallet { Balance = 2500000, UserId = 2 },
						new Wallet { Balance = 1800000, UserId = 3 },
						new Wallet { Balance = 950000, UserId = 4 },
						new Wallet { Balance = 3200000, UserId = 5 },
						new Wallet { Balance = 750000, UserId = 6 },

						// Advertisers wallets
						new Wallet { Balance = 15000000, UserId = 7 },
						new Wallet { Balance = 8500000, UserId = 8 },
						new Wallet { Balance = 6200000, UserId = 9 },
						new Wallet { Balance = 12000000, UserId = 10 },
						new Wallet { Balance = 4500000, UserId = 11 }
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Transactions
				if (!context.Transactions.Any())
				{
					var wallets = context.Wallets.ToList();
					foreach (var wallet in wallets)
					{
						await context.Transactions.AddRangeAsync(
							new Transaction { Balance = wallet.Balance + 10_000, Amount = 10_000, TransactionType = TransactionType.Deposit, PaymentDate = DateTime.UtcNow.AddMinutes(-15), WalletId = wallet.Id, Status = true },
							new Transaction { Balance = wallet.Balance, Amount = 10_000, TransactionType = TransactionType.Withdraw, PaymentDate = DateTime.UtcNow.AddMinutes(-10), WalletId = wallet.Id, Status = false },
							new Transaction { Balance = wallet.Balance - 10_000, Amount = 10_000, TransactionType = TransactionType.Withdraw, PaymentDate = DateTime.UtcNow.AddMinutes(-5), WalletId = wallet.Id, Status = false },
							new Transaction { Balance = wallet.Balance, Amount = 10_000, TransactionType = TransactionType.Deposit, PaymentDate = DateTime.UtcNow, WalletId = wallet.Id, Status = true }
							);
					}
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Campaigns
				if (!context.Campaigns.Any())
				{
					await context.Campaigns.AddRangeAsync(
						// ABC Corporation campaigns
						new Campaign
						{
							AdvertiserId = 7,
							Budget = 5000000,
							Commission = 25000,
							Description = "Quảng cáo sản phẩm phần mềm quản lý doanh nghiệp ABC ERP",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
							Name = "ABC ERP - Giải pháp quản lý toàn diện",
							OriginURL = "https://abc-corp.com/erp",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Education,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851222/wcaqmitrttp7iiri3x8t.jpg",
							AverageStarRate = 4.8,
							Percents = 15
						},
						new Campaign
						{
							AdvertiserId = 7,
							Budget = 3000000,
							Commission = 18000,
							Description = "Khóa học lập trình online miễn phí cho sinh viên",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
							Name = "ABC Code Academy - Học lập trình miễn phí",
							OriginURL = "https://abc-corp.com/academy",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Education,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851547/lb8cyg207pet4ico46sz.jpg",
							AverageStarRate = 4.9,
							Percents = 8
						},

						// XYZ Tech campaigns
						new Campaign
						{
							AdvertiserId = 8,
							Budget = 2800000,
							Commission = 15000,
							Description = "Ứng dụng mobile banking thế hệ mới với bảo mật cao",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(4)),
							Name = "XYZ Banking App - Ngân hàng trong tay bạn",
							OriginURL = "https://xyz-tech.com/banking",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Education,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851626/jojc35fbdbncr0ctie1c.png",
							AverageStarRate = 4.6,
							Percents = 12
						},
						new Campaign
						{
							AdvertiserId = 8,
							Budget = 1500000,
							Commission = 8000,
							Description = "Dịch vụ bảo trì website và ứng dụng chuyên nghiệp",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(8)),
							Name = "XYZ Maintenance Service",
							OriginURL = "https://xyz-tech.com/maintenance",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
							Status = CampaignStatus.Pending,
							TypeCampaign = Industry.Education,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851222/wcaqmitrttp7iiri3x8t.jpg",
							Percents = 0
						},

						// Green Foods campaigns
						new Campaign
						{
							AdvertiserId = 9,
							Budget = 4200000,
							Commission = 22000,
							Description = "Thực phẩm organic tươi ngon, an toàn cho sức khỏe",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
							Name = "Green Foods - Thực phẩm sạch cho cuộc sống xanh",
							OriginURL = "https://greenfoods.vn/organic",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-25)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.FoodAndBeverage,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851547/lb8cyg207pet4ico46sz.jpg",
							AverageStarRate = 4.7,
							Percents = 18
						},
						new Campaign
						{
							AdvertiserId = 9,
							Budget = 1800000,
							Commission = 12000,
							Description = "Combo gia đình tiết kiệm với thực phẩm tươi sống",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(3)),
							Name = "Green Family Combo",
							OriginURL = "https://greenfoods.vn/family-combo",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.FoodAndBeverage,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851626/jojc35fbdbncr0ctie1c.png",
							AverageStarRate = 4.4,
							Percents = 6
						},

						// Fashion House campaigns
						new Campaign
						{
							AdvertiserId = 10,
							Budget = 6500000,
							Commission = 35000,
							Description = "Bộ sưu tập thời trang xuân hè 2025 dành cho giới trẻ",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(4)),
							Name = "Fashion House Spring Summer 2025",
							OriginURL = "https://fashionhouse.vn/spring-summer-2025",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-40)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Other,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851222/wcaqmitrttp7iiri3x8t.jpg",
							AverageStarRate = 4.8,
							Percents = 22
						},
						new Campaign
						{
							AdvertiserId = 10,
							Budget = 2200000,
							Commission = 16000,
							Description = "Phụ kiện thời trang cao cấp cho phái đẹp",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(5)),
							Name = "Fashion Accessories Collection",
							OriginURL = "https://fashionhouse.vn/accessories",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-18)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Other,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851547/lb8cyg207pet4ico46sz.jpg",
							AverageStarRate = 4.5,
							Percents = 11
						},

						// Travel Vietnam campaigns
						new Campaign
						{
							AdvertiserId = 11,
							Budget = 3800000,
							Commission = 28000,
							Description = "Tour du lịch Hạ Long - Sapa 4 ngày 3 đêm giá ưu đãi",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(8)),
							Name = "Tour Hạ Long - Sapa 4N3Đ",
							OriginURL = "https://travelvietnam.com/halong-sapa-tour",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-12)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Tourism,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851626/jojc35fbdbncr0ctie1c.png",
							AverageStarRate = 4.6,
							Percents = 14
						},
						new Campaign
						{
							AdvertiserId = 11,
							Budget = 2500000,
							Commission = 18000,
							Description = "Khám phá vẻ đẹp miền Tây với tour Cần Thơ - An Giang",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
							Name = "Tour Miền Tây 3N2Đ",
							OriginURL = "https://travelvietnam.com/mien-tay-tour",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-8)),
							Status = CampaignStatus.Activing,
							TypeCampaign = Industry.Tourism,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851222/wcaqmitrttp7iiri3x8t.jpg",
							AverageStarRate = 4.3,
							Percents = 9
						},
						new Campaign
						{
							AdvertiserId = 11,
							Budget = 1200000,
							Commission = 8500,
							Description = "Dịch vụ đặt vé máy bay giá rẻ toàn quốc",
							IsDeleted = false,
							EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
							Name = "Đặt vé máy bay giá rẻ",
							OriginURL = "https://travelvietnam.com/flight-booking",
							StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)),
							Status = CampaignStatus.Pending,
							TypeCampaign = Industry.Tourism,
							TypePay = TypePay.CPC,
							Image = "http://res.cloudinary.com/detykxgzs/image/upload/v1742851547/lb8cyg207pet4ico46sz.jpg",
							Percents = 0
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Reports
				if (!context.Reports.Any())
				{
					await context.Reports.AddRangeAsync(
						new Report
						{
							AdvertiserId = 7,
							CampaignId = 1,
							CreateAt = DateTime.UtcNow.AddDays(-5),
							EvidenceURL = "https://evidence.com/report1",
							PublisherId = 2,
							Reason = "Nội dung quảng cáo không phù hợp với website của tôi",
							Status = ReportStatus.Pending
						},
						new Report
						{
							AdvertiserId = 8,
							CampaignId = 3,
							CreateAt = DateTime.UtcNow.AddDays(-3),
							EvidenceURL = "https://evidence.com/report2",
							PublisherId = 3,
							Reason = "Click rate thấp hơn cam kết",
							Status = ReportStatus.Processing
						},
						new Report
						{
							AdvertiserId = 9,
							CampaignId = 5,
							CreateAt = DateTime.UtcNow.AddDays(-7),
							EvidenceURL = "https://evidence.com/report3",
							PublisherId = 4,
							Reason = "Sản phẩm không đúng như mô tả",
							Status = ReportStatus.Rejected
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding UserDetail
				if (!context.UserDetail.Any())
				{
					await context.UserDetail.AddRangeAsync(
						// Admin
						new UserDetail
						{
							ApplicationUserId = 1,
							DateOfBirth = new DateTime(1985, 3, 15),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "123 Nguyễn Văn Cừ, Quận 5, TP.HCM"
						},

						// Publishers
						new UserDetail
						{
							ApplicationUserId = 2,
							DateOfBirth = new DateTime(1990, 8, 20),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "456 Lê Văn Việt, Quận 9, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 3,
							DateOfBirth = new DateTime(1988, 12, 5),
							Gender = Gender.Female,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "789 Võ Văn Tần, Quận 3, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 4,
							DateOfBirth = new DateTime(1992, 6, 10),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "321 Cách Mạng Tháng 8, Quận 10, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 5,
							DateOfBirth = new DateTime(1987, 4, 25),
							Gender = Gender.Female,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "654 Hoàng Văn Thụ, Quận Phú Nhuận, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 6,
							DateOfBirth = new DateTime(1991, 11, 18),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "987 Nguyễn Thị Minh Khai, Quận 1, TP.HCM"
						},

						// Advertisers
						new UserDetail
						{
							ApplicationUserId = 7,
							DateOfBirth = new DateTime(1983, 9, 12),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "159 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 8,
							DateOfBirth = new DateTime(1986, 2, 28),
							Gender = Gender.Female,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "753 Trường Chinh, Quận Tân Bình, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 9,
							DateOfBirth = new DateTime(1989, 7, 3),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "246 Lý Thường Kiệt, Quận 11, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 10,
							DateOfBirth = new DateTime(1984, 1, 14),
							Gender = Gender.Female,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "369 Nam Kỳ Khởi Nghĩa, Quận 3, TP.HCM"
						},
						new UserDetail
						{
							ApplicationUserId = 11,
							DateOfBirth = new DateTime(1982, 10, 7),
							Gender = Gender.Male,
							AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
							Address = "147 Pasteur, Quận 1, TP.HCM"
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Feedbacks
				if (!context.Feedbacks.Any())
				{
					await context.Feedbacks.AddRangeAsync(
						// Feedbacks cho Campaign ABC ERP
						new Feedback
						{
							CampaignId = 1,
							Description = "Campaign rất tốt, ROI cao và dễ convert. Sẽ tiếp tục hợp tác!",
							StarRate = 5,
							Timestamp = DateTime.UtcNow.AddDays(-2),
							FeedbackerId = 2
						},
						new Feedback
						{
							CampaignId = 1,
							Description = "Sản phẩm ổn, nhưng landing page cần cải thiện thêm.",
							StarRate = 4,
							Timestamp = DateTime.UtcNow.AddDays(-5),
							FeedbackerId = 3
						},

						// Feedbacks cho Campaign ABC Code Academy
						new Feedback
						{
							CampaignId = 2,
							Description = "Khóa học hay và bổ ích, audience phản hồi tích cực.",
							StarRate = 5,
							Timestamp = DateTime.UtcNow.AddDays(-1),
							FeedbackerId = 4
						},
						new Feedback
						{
							CampaignId = 2,
							Description = "Commission ổn nhưng thời gian chạy hơi dài.",
							StarRate = 4,
							Timestamp = DateTime.UtcNow.AddDays(-3),
							FeedbackerId = 5
						},

						// Feedbacks cho XYZ Banking App
						new Feedback
						{
							CampaignId = 3,
							Description = "App banking hiện đại, dễ thuyết phục khách hàng đăng ký.",
							StarRate = 5,
							Timestamp = DateTime.UtcNow.AddDays(-4),
							FeedbackerId = 6
						},

						// Feedbacks cho Green Foods
						new Feedback
						{
							CampaignId = 5,
							Description = "Sản phẩm organic chất lượng tốt, khách hàng hài lòng.",
							StarRate = 5,
							Timestamp = DateTime.UtcNow.AddDays(-6),
							FeedbackerId = 2
						},
						new Feedback
						{
							CampaignId = 6,
							Description = "Combo gia đình hợp lý, phù hợp với target audience.",
							StarRate = 4,
							Timestamp = DateTime.UtcNow.AddDays(-7),
							FeedbackerId = 3
						},

						// Feedbacks cho Fashion House
						new Feedback
						{
							CampaignId = 7,
							Description = "Bộ sưu tập đẹp, trendy và thu hút giới trẻ.",
							StarRate = 5,
							Timestamp = DateTime.UtcNow.AddDays(-8),
							FeedbackerId = 4
						},

						// Feedbacks cho Travel Vietnam
						new Feedback
						{
							CampaignId = 9,
							Description = "Tour du lịch được thiết kế tốt, giá cả hợp lý.",
							StarRate = 4,
							Timestamp = DateTime.UtcNow.AddDays(-9),
							FeedbackerId = 5
						},
						new Feedback
						{
							CampaignId = 10,
							Description = "Tour miền Tây rất thú vị, khách hàng đánh giá cao.",
							StarRate = 4,
							Timestamp = DateTime.UtcNow.AddDays(-10),
							FeedbackerId = 6
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding CampaignParticipations
				if (!context.CampaignParticipations.Any())
				{
					await context.CampaignParticipations.AddRangeAsync(
						// Publisher 2 tham gia nhiều campaigns
						new CampaignParticipation
						{
							CampaignId = 1, // ABC ERP
							CreateAt = DateTime.Now,
							PublisherId = 2,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 5, // Green Foods
							CreateAt = DateTime.UtcNow.AddDays(-20),
							PublisherId = 3,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 7, // Fashion House Spring Summer
							CreateAt = DateTime.UtcNow.AddDays(-15),
							PublisherId = 4,
							Status = CampaignParticipationStatus.Participated,
						},

						// Publisher 3 tham gia
						new CampaignParticipation
						{
							CampaignId = 1, // ABC ERP
							CreateAt = DateTime.UtcNow.AddDays(-22),
							PublisherId = 5,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 3, // XYZ Banking App
							CreateAt = DateTime.UtcNow.AddDays(-18),
							PublisherId = 6,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 6, // Green Family Combo
							CreateAt = DateTime.Now,
							PublisherId = 3,
							Status = CampaignParticipationStatus.Participated,
						},

						// Publisher 4
						new CampaignParticipation
						{
							CampaignId = 2, // ABC Code Academy
							CreateAt = DateTime.Now,
							PublisherId = 4,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 7, // Fashion House Spring Summer
							CreateAt = DateTime.Now,
							PublisherId = 4,
							Status = CampaignParticipationStatus.Participated,
						},

						// Publisher 5
						new CampaignParticipation
						{
							CampaignId = 2, // ABC Code Academy
							CreateAt = DateTime.Now,
							PublisherId = 5,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 9, // Tour Hạ Long - Sapa
							CreateAt = DateTime.Now,
							PublisherId = 5,
							Status = CampaignParticipationStatus.Participated,
						},

						// Publisher 6
						new CampaignParticipation
						{
							CampaignId = 3, // XYZ Banking App
							CreateAt = DateTime.Now,
							PublisherId = 6,
							Status = CampaignParticipationStatus.Participated,
						},
						new CampaignParticipation
						{
							CampaignId = 10, // Tour Miền Tây
							CreateAt = DateTime.Now,
							PublisherId = 6,
							Status = CampaignParticipationStatus.Participated,
						},

						// Một số pending và rejected
						new CampaignParticipation
						{
							CampaignId = 8, // Fashion Accessories
							CreateAt = DateTime.Now,
							PublisherId = 2,
							Status = CampaignParticipationStatus.Pending,
						},
						new CampaignParticipation
						{
							CampaignId = 4, // XYZ Maintenance Service (Pending campaign)
							CreateAt = DateTime.Now,
							PublisherId = 3,
							Status = CampaignParticipationStatus.Pending,
						},
						new CampaignParticipation
						{
							CampaignId = 11, // Đặt vé máy bay (Pending campaign)
							CreateAt = DateTime.Now,
							PublisherId = 4,
							Status = CampaignParticipationStatus.Rejected,
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Traffics
				if (!context.Traffics.Any())
				{
					await context.Traffics.AddRangeAsync(
						// Traffic cho CampaignParticipation 1 (Publisher 2 - Campaign ABC ERP)
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 1,
							DeviceType = "Desktop",
							IpAddress = "118.69.71.45",
							IsValid = true,
							ReferrerURL = "https://techblog.vn/abc-erp-review",
							Timestamp = DateTime.UtcNow.AddDays(-20),
							ClickId = Guid.NewGuid().ToString(),
						},
						new Traffic
						{
							Browser = "Safari",
							CampaignParticipationId = 1,
							DeviceType = "Mobile",
							IpAddress = "14.169.85.142",
							IsValid = true,
							ReferrerURL = "https://techblog.vn/erp-solutions",
							Timestamp = DateTime.UtcNow.AddDays(-18),
							ClickId = Guid.NewGuid().ToString(),
						},
						new Traffic
						{
							Browser = "Firefox",
							CampaignParticipationId = 1,
							DeviceType = "Desktop",
							IpAddress = "123.21.58.97",
							IsValid = true,
							ReferrerURL = "https://techblog.vn/business-software",
							Timestamp = DateTime.UtcNow.AddDays(-15),
							ClickId = Guid.NewGuid().ToString()
						},

						// Traffic cho CampaignParticipation 2 (Publisher 2 - Green Foods)
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 2,
							DeviceType = "Mobile",
							IpAddress = "171.244.136.78",
							IsValid = true,
							ReferrerURL = "https://healthyfood.vn/green-foods",
							Timestamp = DateTime.UtcNow.AddDays(-16),
							ClickId = Guid.NewGuid().ToString()
						},
						new Traffic
						{
							Browser = "Edge",
							CampaignParticipationId = 2,
							DeviceType = "Desktop",
							IpAddress = "113.185.44.219",
							IsValid = true,
							ReferrerURL = "https://healthyfood.vn/organic-review",
							Timestamp = DateTime.UtcNow.AddDays(-12),
							ClickId = Guid.NewGuid().ToString()
						},

						// Traffic cho CampaignParticipation 4 (Publisher 3 - ABC ERP)
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 4,
							DeviceType = "Desktop",
							IpAddress = "27.72.145.33",
							IsValid = true,
							ReferrerURL = "https://biztech.com.vn/erp-software",
							Timestamp = DateTime.UtcNow.AddDays(-19),
							ClickId = Guid.NewGuid().ToString()
						},
						new Traffic
						{
							Browser = "Safari",
							CampaignParticipationId = 4,
							DeviceType = "Tablet",
							IpAddress = "116.109.25.184",
							IsValid = true,
							ReferrerURL = "https://biztech.com.vn/business-solutions",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-14)
						},

						// Traffic cho CampaignParticipation 5 (Publisher 3 - XYZ Banking)
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 5,
							DeviceType = "Mobile",
							IpAddress = "125.235.4.167",
							IsValid = true,
							ReferrerURL = "https://fintech.vn/banking-apps",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-13)
						},
						new Traffic
						{
							Browser = "Firefox",
							CampaignParticipationId = 5,
							DeviceType = "Desktop",
							IpAddress = "210.245.87.92",
							IsValid = true,
							ReferrerURL = "https://fintech.vn/mobile-banking",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-10)
						},

						// Traffic cho các CampaignParticipation khác
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 7, // Publisher 4 - ABC Code Academy
							DeviceType = "Desktop",
							IpAddress = "103.57.149.201",
							IsValid = true,
							ReferrerURL = "https://codingvn.com/free-courses",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-8)
						},
						new Traffic
						{
							Browser = "Safari",
							CampaignParticipationId = 9, // Publisher 5 - ABC Code Academy
							DeviceType = "Mobile",
							IpAddress = "1.54.238.119",
							IsValid = true,
							ReferrerURL = "https://learntech.vn/programming",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-7)
						},
						new Traffic
						{
							Browser = "Edge",
							CampaignParticipationId = 10, // Publisher 5 - Tour Hạ Long Sapa
							DeviceType = "Desktop",
							IpAddress = "58.187.96.245",
							IsValid = true,
							ReferrerURL = "https://dulichvn.com/tour-mien-bac",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-3)
						},

						// Một số traffic invalid để test
						new Traffic
						{
							Browser = "Chrome",
							CampaignParticipationId = 1,
							DeviceType = "Desktop",
							IpAddress = "192.168.1.1", // Local IP - invalid
							IsValid = false,
							ReferrerURL = "https://techblog.vn/test",
							ClickId = Guid.NewGuid().ToString(),
							Timestamp = DateTime.UtcNow.AddDays(-5)
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Conversion
				if (!context.Conversions.Any())
				{
					var traffics = await context.Traffics.ToListAsync();
					foreach (var traffic in traffics)
					{
						await context.Conversions.AddRangeAsync(
							new Conversion
							{
								ClickId = traffic.ClickId,
								EventType = ConversionEventType.Sale,
								OrderId = Guid.NewGuid().ToString(),
								Revenue = 100_000,
								Timestamp = DateTime.UtcNow
							},
							new Conversion
							{
								ClickId = traffic.ClickId,
								EventType = ConversionEventType.Install,
								OrderId = Guid.NewGuid().ToString(),
								Revenue = 100_000,
								Timestamp = DateTime.UtcNow
							}
							);
					}
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Courses
				if (!context.Courses.Any())
				{
					var now = DateTime.UtcNow;

					await context.Courses.AddRangeAsync(
						new Course
						{
							Title = "Digital Marketing cơ bản",
							CreateAt = now,
							UpdateAt = now,
							CreateById = 1, // Replace with actual user ID
							Price = 199000,
							AvgRate = 4.5,
							LessonLearned = "Hiểu tổng quan về Digital Marketing và các nền tảng cơ bản.",
							Content = "Khóa học bao gồm giới thiệu về Digital Marketing, quảng cáo Facebook, và Google Ads.",
							Description = "Dành cho người mới bắt đầu tìm hiểu về Digital Marketing."
						},
						new Course
						{
							Title = "Tối ưu Affiliate Marketing",
							CreateAt = now,
							UpdateAt = now,
							CreateById = 1,
							Price = 249000,
							AvgRate = 4.6,
							LessonLearned = "Biết cách tối ưu tỷ lệ chuyển đổi và sử dụng A/B testing.",
							Content = "Khóa học tập trung vào cải thiện hiệu quả Affiliate thông qua chiến lược tối ưu.",
							Description = "Phù hợp với người đã có kinh nghiệm cơ bản về Affiliate Marketing."
						},
						new Course
						{
							Title = "SEO từ A-Z",
							CreateAt = now,
							UpdateAt = now,
							CreateById = 1,
							Price = 299000,
							AvgRate = 4.7,
							LessonLearned = "Thành thạo nghiên cứu từ khóa, xây dựng liên kết và các nguyên lý SEO.",
							Content = "Khóa học giúp học viên xây dựng chiến lược SEO hiệu quả.",
							Description = "Tăng thứ hạng website trên Google và thu hút lưu lượng tự nhiên."
						},
						new Course
						{
							Title = "Facebook Ads nâng cao",
							CreateAt = now,
							UpdateAt = now,
							CreateById = 1,
							Price = 269000,
							AvgRate = 4.4,
							LessonLearned = "Tối ưu hình ảnh quảng cáo, đối tượng mục tiêu và nâng cao kỹ thuật Facebook Ads.",
							Content = "Dành cho những ai muốn nâng cấp kỹ năng quảng cáo trên nền tảng Facebook.",
							Description = "Nâng cao hiệu quả quảng cáo với các chiến thuật tiên tiến."
						},
						new Course
						{
							Title = "Affiliate Blueprint chuyên sâu",
							CreateAt = now,
							UpdateAt = now,
							CreateById = 1,
							Price = 349000,
							AvgRate = 4.8,
							LessonLearned = "Xây dựng hệ thống, chọn niche phù hợp và mở rộng chiến lược kinh doanh.",
							Content = "Chiến lược toàn diện để phát triển hệ thống Affiliate từ đầu đến chuyên sâu.",
							Description = "Khóa học chuyên sâu dành cho những người muốn phát triển hệ thống Affiliate thực chiến."
						}
					);

					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Videos
				if (!context.Videos.Any())
				{
					await context.Videos.AddRangeAsync(
						// Videos for Digital Marketing course (CourseId = 1)
						new Video
						{
							Link = "https://youtu.be/digital_marketing_intro",
							CourseId = 1,
							Title = "Giới thiệu Digital Marketing cơ bản"
						},
						new Video
						{
							Link = "https://youtu.be/facebook_ads_basics",
							CourseId = 1,
							Title = "Facebook Ads cơ bản"
						},
						new Video
						{
							Link = "https://youtu.be/google_ads_tutorial",
							CourseId = 1,
							Title = "Google Ads cho người mới"
						},

						// Videos for Affiliate Optimization course (CourseId = 2)
						new Video
						{
							Link = "https://youtu.be/affiliate_conversion",
							CourseId = 2,
							Title = "Tối ưu tỷ lệ chuyển đổi"
						},
						new Video
						{
							Link = "https://youtu.be/ab_testing",
							CourseId = 2,
							Title = "A/B Testing hiệu quả"
						},
						new Video
						{
							Link = "https://youtu.be/landing_page_optimize",
							CourseId = 2,
							Title = "Tối ưu Landing Page"
						},

						// Videos for SEO course (CourseId = 3)
						new Video
						{
							Link = "https://youtu.be/seo_fundamentals",
							CourseId = 3,
							Title = "Nguyên lý SEO cơ bản"
						},
						new Video
						{
							Link = "https://youtu.be/keyword_research",
							CourseId = 3,
							Title = "Nghiên cứu từ khóa"
						},
						new Video
						{
							Link = "https://youtu.be/link_building",
							CourseId = 3,
							Title = "Chiến lược xây dựng liên kết"
						},

						// Videos for Facebook Ads course (CourseId = 4)
						new Video
						{
							Link = "https://youtu.be/fb_ads_advanced",
							CourseId = 4,
							Title = "Facebook Ads nâng cao"
						},
						new Video
						{
							Link = "https://youtu.be/ad_creative",
							CourseId = 4,
							Title = "Tối ưu hình ảnh quảng cáo"
						},
						new Video
						{
							Link = "https://youtu.be/audience_targeting",
							CourseId = 4,
							Title = "Targeting khách hàng mục tiêu"
						},

						// Videos for Affiliate Blueprint course (CourseId = 5)
						new Video
						{
							Link = "https://youtu.be/affiliate_system",
							CourseId = 5,
							Title = "Xây dựng hệ thống Affiliate"
						},
						new Video
						{
							Link = "https://youtu.be/niche_selection",
							CourseId = 5,
							Title = "Chọn niche hiệu quả"
						},
						new Video
						{
							Link = "https://youtu.be/scaling_strategies",
							CourseId = 5,
							Title = "Chiến lược mở rộng"
						}
					);
					await context.SaveChangesAsync();
				}
				#endregion

				#region Seeding Posts
				if (!context.Posts.Any())
				{
					var admin = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
					var publisher1 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "publisher");
					var advertiser1 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "advertiser");

					if (admin != null && publisher1 != null && advertiser1 != null)
					{
						await context.Posts.AddRangeAsync(
							new Post
							{
								Title = "Chào mừng đến với ClickFlow!",
								Content = "ClickFlow là nền tảng kết nối nhà quảng cáo và nhà xuất bản hiệu quả nhất hiện nay. Chúng tôi cung cấp giải pháp toàn diện cho các chiến dịch tiếp thị liên kết.",
								CreatedAt = DateTime.UtcNow.AddDays(-10),
								AuthorId = admin.Id,
								Topic = Topic.Other,
								LikeCount = 325,
								FeedbackNumber = 24,
								IsDeleted = false
							},
							new Post
							{
								Title = "Hướng dẫn sử dụng nền tảng cho nhà xuất bản mới",
								Content = "Bài viết này sẽ hướng dẫn bạn từng bước đăng ký và bắt đầu kiếm tiền với ClickFlow. Bao gồm: đăng ký tài khoản, xác minh, chọn chiến dịch và triển khai quảng cáo.",
								CreatedAt = DateTime.UtcNow.AddDays(-8),
								AuthorId = admin.Id,
								Topic = Topic.Other,
								LikeCount = 187,
								FeedbackNumber = 15,
								IsDeleted = false
							},
							new Post
							{
								Title = "5 mẹo tăng hiệu suất quảng cáo Affiliate",
								Content = "1. Chọn đúng sản phẩm phù hợp với audience\n2. Tối ưu landing page\n3. Sử dụng A/B testing\n4. Tập trung vào mobile experience\n5. Phân tích và điều chỉnh thường xuyên",
								CreatedAt = DateTime.UtcNow.AddDays(-5),
								AuthorId = publisher1.Id,
								Topic = Topic.Tips,
								LikeCount = 243,
								FeedbackNumber = 32,
								IsDeleted = false
							},
							new Post
							{
								Title = "Cập nhật chính sách thanh toán mới từ tháng 6/2025",
								Content = "Để cải thiện trải nghiệm người dùng, chúng tôi sẽ áp dụng chính sách thanh toán mới:\n- Thanh toán hàng tuần thay vì hàng tháng\n- Phí rút tiền giảm 50%\n- Hỗ trợ thêm 3 phương thức thanh toán mới",
								CreatedAt = DateTime.UtcNow.AddDays(-3),
								AuthorId = admin.Id,
								Topic = Topic.Tips,
								LikeCount = 156,
								FeedbackNumber = 18,
								IsDeleted = false
							},
							new Post
							{
								Title = "Cách tạo chiến dịch hiệu quả cho nhà quảng cáo",
								Content = "Là nhà quảng cáo, bạn cần chú ý:\n1. Xác định rõ mục tiêu chiến dịch\n2. Thiết lập ngân sách hợp lý\n3. Cung cấp creative chất lượng\n4. Đặt mức hoa hồng cạnh tranh\n5. Cung cấp thông tin minh bạch về sản phẩm",
								CreatedAt = DateTime.UtcNow.AddDays(-2),
								AuthorId = advertiser1.Id,
								Topic = Topic.Tips,
								LikeCount = 98,
								FeedbackNumber = 7,
								IsDeleted = false
							},
							new Post
							{
								Title = "Câu hỏi thường gặp về hệ thống tracking",
								Content = "Q: Làm sao để kiểm tra số click?\nA: Vào Dashboard > Báo cáo > Chi tiết traffic\n\nQ: Tại sao số click giữa hệ thống và Google Analytics khác nhau?\nA: Do phương pháp tracking khác nhau, chúng tôi chỉ tính click hợp lệ",
								CreatedAt = DateTime.UtcNow.AddDays(-1),
								AuthorId = admin.Id,
								Topic = Topic.QA,
								LikeCount = 76,
								FeedbackNumber = 5,
								IsDeleted = false
							}
						);

						await context.SaveChangesAsync();
					}
				}
				#endregion

				#region Seeding Comments
				if (!context.Comments.Any())
				{
					var posts = await context.Posts.ToListAsync();
					var users = await context.Users.Take(5).ToListAsync();

					if (posts.Count > 0 && users.Count > 0)
					{
						var comments = new List<Comment>();

						// Comment cho bài viết đầu tiên
						comments.Add(new Comment
						{
							Content = "Bài viết rất hữu ích, cảm ơn admin!",
							CreatedAt = DateTime.UtcNow.AddDays(-9),
							AuthorId = users[1].Id,
							PostId = posts[0].Id,
							IsDeleted = false
						});

						comments.Add(new Comment
						{
							Content = "Mình mới tham gia, mong được hỗ trợ thêm!",
							CreatedAt = DateTime.UtcNow.AddDays(-8),
							AuthorId = users[2].Id,
							PostId = posts[0].Id,
							IsDeleted = false
						});

						// Reply cho comment trên
						comments.Add(new Comment
						{
							Content = "Bạn có thể đặt câu hỏi cụ thể ở đây hoặc qua ticket hỗ trợ nhé!",
							CreatedAt = DateTime.UtcNow.AddDays(-8).AddHours(2),
							AuthorId = users[0].Id, // admin
							PostId = posts[0].Id,
							ParentCommentId = 2,
							IsDeleted = false
						});

						// Comment cho bài viết thứ 2
						comments.Add(new Comment
						{
							Content = "Hướng dẫn rất chi tiết, mình đã làm theo và kiếm được thu nhập đầu tiên!",
							CreatedAt = DateTime.UtcNow.AddDays(-7),
							AuthorId = users[3].Id,
							PostId = posts[1].Id,
							IsDeleted = false
						});

						// Comment cho bài viết thứ 3
						comments.Add(new Comment
						{
							Content = "Mình áp dụng tip số 3 và thấy CTR tăng lên 20%, cảm ơn tác giả!",
							CreatedAt = DateTime.UtcNow.AddDays(-4),
							AuthorId = users[4].Id,
							PostId = posts[2].Id,
							IsDeleted = false
						});

						comments.Add(new Comment
						{
							Content = "Bạn có thể chia sẻ thêm về cách bạn làm A/B testing được không?",
							CreatedAt = DateTime.UtcNow.AddDays(-4).AddHours(3),
							AuthorId = users[1].Id,
							PostId = posts[2].Id,
							IsDeleted = false
						});

						// Comment cho bài viết thứ 4
						comments.Add(new Comment
						{
							Content = "Chính sách mới quá tuyệt, nhất là phần giảm phí rút tiền!",
							CreatedAt = DateTime.UtcNow.AddDays(-2),
							AuthorId = users[2].Id,
							PostId = posts[3].Id,
							IsDeleted = false
						});

						await context.Comments.AddRangeAsync(comments);
						await context.SaveChangesAsync();
					}
				}
				#endregion

				#region Seeding Conversations
				if (!context.Conversations.Any())
				{
					// Lấy các user có vai trò khác nhau
					var admin = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
					var publisher1 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "publisher");
					var publisher2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "publisher2");
					var advertiser1 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "advertiser");
					var advertiser2 = await context.Users.FirstOrDefaultAsync(u => u.UserName == "advertiser2");

					if (admin != null && publisher1 != null && publisher2 != null && advertiser1 != null && advertiser2 != null)
					{
						await context.Conversations.AddRangeAsync(
							// Admin chat với publisher
							new Conversation
							{
								CreatedAt = DateTime.UtcNow.AddDays(-5),
								User1Id = admin.Id,
								User2Id = publisher1.Id,
							},
							// Admin chat với advertiser
							new Conversation
							{
								CreatedAt = DateTime.UtcNow.AddDays(-4),
								User1Id = admin.Id,
								User2Id = advertiser1.Id,
							},
							// Publisher chat với advertiser
							new Conversation
							{
								CreatedAt = DateTime.UtcNow.AddDays(-3),
								User1Id = publisher1.Id,
								User2Id = advertiser1.Id,
							},
							// Publisher chat với publisher khác
							new Conversation
							{
								CreatedAt = DateTime.UtcNow.AddDays(-2),
								User1Id = publisher1.Id,
								User2Id = publisher2.Id,
							},
							// Advertiser chat với advertiser khác
							new Conversation
							{
								CreatedAt = DateTime.UtcNow.AddDays(-1),
								User1Id = advertiser1.Id,
								User2Id = advertiser2.Id,
							}
						);

						await context.SaveChangesAsync();
					}
				}
				#endregion

				#region Seeding Messages
				if (!context.Messages.Any())
				{
					var conversations = await context.Conversations.ToListAsync();

					if (conversations.Count > 0)
					{
						var messages = new List<Message>();

						// Tin nhắn trong conversation 1 (admin - publisher1)
						messages.Add(new Message
						{
							ConversationId = conversations[0].Id,
							IsRead = true,
							SenderId = conversations[0].User1Id, // admin
							SentAt = DateTime.UtcNow.AddDays(-5).AddHours(1),
							Text = "Chào bạn, mình có thể giúp gì cho bạn?",
							Type = MessageType.Text
						});

						messages.Add(new Message
						{
							ConversationId = conversations[0].Id,
							IsRead = true,
							SenderId = conversations[0].User2Id, // publisher1
							SentAt = DateTime.UtcNow.AddDays(-5).AddHours(1).AddMinutes(15),
							Text = "Mình mới tham gia, muốn hỏi về cách rút tiền ạ",
							Type = MessageType.Text
						});

						messages.Add(new Message
						{
							ConversationId = conversations[0].Id,
							IsRead = true,
							SenderId = conversations[0].User1Id,
							SentAt = DateTime.UtcNow.AddDays(-5).AddHours(1).AddMinutes(20),
							Text = "Bạn vào mục Wallet > Withdraw, sau đó điền thông tin ngân hàng và số tiền muốn rút nhé",
							Type = MessageType.Text
						});

						// Tin nhắn trong conversation 2 (admin - advertiser1)
						messages.Add(new Message
						{
							ConversationId = conversations[1].Id,
							IsRead = true,
							SenderId = conversations[1].User2Id, // advertiser1
							SentAt = DateTime.UtcNow.AddDays(-4).AddHours(2),
							Text = "Chào admin, chiến dịch của tôi bị từ chối, lý do là gì vậy?",
							Type = MessageType.Text
						});

						messages.Add(new Message
						{
							ConversationId = conversations[1].Id,
							IsRead = true,
							SenderId = conversations[1].User1Id, // admin
							SentAt = DateTime.UtcNow.AddDays(-4).AddHours(2).AddMinutes(10),
							Text = "Do landing page của bạn chứa nội dung không phù hợp, vui lòng chỉnh sửa và gửi lại nhé",
							Type = MessageType.Text
						});

						await context.Messages.AddRangeAsync(messages);
						await context.SaveChangesAsync();
					}
				}
				#endregion

				#region Seeding UserPlans
				if (!context.UserPlans.Any())
				{
					await context.UserPlans.AddRangeAsync(
						new UserPlan
						{
							PlanId = 1,
							CurrentClicks = 0,
							CurrentCampaigns = 0,
							CurrentConversions = 0,
							ExpirationDate = null,
							UserId = 2,
						},
						new UserPlan
						{
							PlanId = 1,
							CurrentClicks = 0,
							CurrentCampaigns = 0,
							CurrentConversions = 0,
							ExpirationDate = null,
							UserId = 3,
						},
						new UserPlan
						{
							PlanId = 1,
							CurrentClicks = 0,
							CurrentCampaigns = 0,
							CurrentConversions = 0,
							ExpirationDate = null,
							UserId = 4,
						},
						new UserPlan
						{
							PlanId = 1,
							CurrentClicks = 0,
							CurrentCampaigns = 0,
							CurrentConversions = 0,
							ExpirationDate = null,
							UserId = 5,
						},
						new UserPlan
						{
							PlanId = 1,
							CurrentClicks = 0,
							CurrentCampaigns = 0,
							CurrentConversions = 0,
							ExpirationDate = null,
							UserId = 6,
						}
						);
					await context.SaveChangesAsync();
				}
				#endregion
			}
		}
	}
}
