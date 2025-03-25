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

            if (env.IsDevelopment())
            {
                #region Seeding Users
                if (!context.Users.Any())
                {
                    await context.Users.AddRangeAsync(
                        // Pass: Admin@123
                        new ApplicationUser { FullName = "admin", Role = Role.Admin, UserName = "admin", NormalizedUserName = "ADMIN", Email = "admin@email.com", NormalizedEmail = "ADMIN@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP", ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a", PhoneNumber = "0999999999", LockoutEnabled = true },
                        // Pass: Publisher@123
                        new ApplicationUser { FullName = "publisher", Role = Role.Publisher, UserName = "publisher", NormalizedUserName = "PUBLISHER", Email = "publisher@email.com", NormalizedEmail = "PUBLISHER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0988888888", LockoutEnabled = true },
                        // Pass: Advertiser@123
                        new ApplicationUser { FullName = "advertiser", Role = Role.Advertiser, UserName = "advertiser", NormalizedUserName = "ADVERTISER", Email = "advertiser@email.com", NormalizedEmail = "ADVERTISER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0977777777", LockoutEnabled = true }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding UserRoles
                if (!context.UserRoles.Any())
                {
                    await context.UserRoles.AddRangeAsync(
                        new IdentityUserRole<int> { UserId = 1, RoleId = 1 },
                        new IdentityUserRole<int> { UserId = 2, RoleId = 3 },
                        new IdentityUserRole<int> { UserId = 3, RoleId = 2 }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding Publishers
                if (!context.Publishers.Any())
                {
                    await context.Publishers.AddRangeAsync(
                        new Publisher
                        {
                            Id = 2,
                            UserId = 2
                        }
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
                            Id = 3,
                            CompanyName = "ABC",
                            IntroductionWebsite = "ABC",
                            StaffSize = 0,
                            Industry = Industry.FoodAndBeverage,
                            UserId = 3
                        }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding Wallets
                if (!context.Wallets.Any())
                {
                    await context.Wallets.AddRangeAsync(
                        new Wallet { Balance = 0, UserId = 2 },
                        new Wallet { Balance = 0, UserId = 3 }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding Campaigns
                if (!context.Campaigns.Any())
                {
                    await context.Campaigns.AddRangeAsync(
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 1000000,
                            Commission = 10000,
                            Description = "ABC",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
                            Name = "ABC",
                            OriginURL = "https://youtube.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Activing,
                            TypeCampaign = Industry.Education,
                            TypePay = TypePay.CPC,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            AverageStarRate = 4.9,
                            Percents = 10
                        },
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 2000000,
                            Commission = 15000,
                            Description = "DEF",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
                            Name = "DEF",
                            OriginURL = "https://facebook.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Activing,
                            TypeCampaign = Industry.FoodAndBeverage,
                            TypePay = TypePay.CPC,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            AverageStarRate = 4.5,
                            Percents = 15
                        },
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 500000,
                            Commission = 5000,
                            Description = "XYZ",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(3)),
                            Name = "XYZ",
                            OriginURL = "https://twitter.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Pending,
                            TypeCampaign = Industry.Education,
                            TypePay = TypePay.CPS,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            Percents = 20
                        },
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 1000000,
                            Commission = 10000,
                            Description = "Chiến dịch quảng cáo sản phẩm ABC",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
                            Name = "Chiến dịch ABC",
                            OriginURL = "https://youtube.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Activing,
                            TypeCampaign = Industry.Education,
                            TypePay = TypePay.CPC,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            AverageStarRate = 4.5
                        },
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 2000000,
                            Commission = 20000,
                            Description = "Chiến dịch quảng cáo sản phẩm XYZ",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)),
                            Name = "Chiến dịch XYZ",
                            OriginURL = "https://google.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Activing,
                            TypeCampaign = Industry.Tourism,
                            TypePay = TypePay.CPA,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            AverageStarRate = 4.6
                        },
                        new Campaign
                        {
                            AdvertiserId = 3,
                            Budget = 1500000,
                            Commission = 15000,
                            Description = "Chiến dịch quảng cáo sản phẩm DEF",
                            IsDeleted = false,
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(9)),
                            Name = "Chiến dịch DEF",
                            OriginURL = "https://facebook.com",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                            Status = CampaignStatus.Pending,
                            TypeCampaign = Industry.FoodAndBeverage,
                            TypePay = TypePay.CPC,
                            Image = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp"
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
                            AdvertiserId = 3,
                            CampaignId = 1,
                            CreateAt = DateTime.UtcNow,
                            EvidenceURL = "https://abc.com",
                            PublisherId = 2,
                            Reason = "ABC",
                            Status = ReportStatus.Pending
                        }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding UserDetail
                if (!context.UserDetail.Any())
                {
                    await context.UserDetail.AddRangeAsync(
                        new UserDetail
                        {
                            ApplicationUserId = 1, // Admin
                            DateOfBirth = new DateTime(1990, 1, 1),
                            Gender = Gender.Male,
                            AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            Address = "123 Admin Street"
                        },
                        new UserDetail
                        {
                            ApplicationUserId = 2, // Publisher
                            DateOfBirth = new DateTime(1995, 5, 5),
                            Gender = Gender.Female,
                            AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            Address = "456 Publisher Street"
                        },
                        new UserDetail
                        {
                            ApplicationUserId = 3, // Advertiser
                            DateOfBirth = new DateTime(1985, 10, 10),
                            Gender = Gender.Male,
                            AvatarURL = "https://res.cloudinary.com/detykxgzs/image/upload/v1741893995/kmneq8vnkryegmurnknr.webp",
                            Address = "789 Advertiser Street"
                        }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding Feedbacks
                if (!context.Feedbacks.Any())
                {
                    await context.Feedbacks.AddRangeAsync(
                        new Feedback
                        {
                            CampaignId = 1,
                            Description = "Great campaign!",
                            StarRate = 5,
                            Timestamp = DateTime.UtcNow,
                            FeedbackerId = 2
                        },
                        new Feedback
                        {
                            CampaignId = 2,
                            Description = "Could be better.",
                            StarRate = 3,
                            Timestamp = DateTime.UtcNow.AddDays(-1),
                            FeedbackerId = 2
                        }

                    );
                    await context.SaveChangesAsync();
                }
                #endregion

                #region Seeding CampaignParticipations
                if (!context.CampaignParticipations.Any())
                {
                    await context.CampaignParticipations.AddRangeAsync(
                        new CampaignParticipation
                        {
                            CampaignId = 1,
                            CreateAt = DateTime.UtcNow,
                            PublisherId = 2,
                            Status = CampaignParticipationStatus.Pending,
                        },
                        new CampaignParticipation
                        {
                            CampaignId = 2,
                            CreateAt = DateTime.UtcNow,
                            PublisherId = 2,
                            Status = CampaignParticipationStatus.Participated,
                        },
                        new CampaignParticipation
                        {
                            CampaignId = 3,
                            CreateAt = DateTime.UtcNow,
                            PublisherId = 2,
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
                        new Traffic
                        {
                            Browser = "Chrome",
                            CampaignParticipationId = 1,
                            DeviceType = "Mobile",
                            IpAddress = "1.1.1.1",
                            IsValid = true,
                            ReferrerURL = "http://abc.com",
                            Revenue = 1000,
                            Timestamp = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
                #endregion
            }
        }
    }
}
