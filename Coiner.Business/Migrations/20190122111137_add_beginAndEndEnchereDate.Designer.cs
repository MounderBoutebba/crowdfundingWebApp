﻿// <auto-generated />
using Coiner.Business.Context;
using Coiner.Business.Models;
using Coiner.Business.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Coiner.Business.Migrations
{
    [DbContext(typeof(CoinerContext))]
    [Migration("20190122111137_add_beginAndEndEnchereDate")]
    partial class add_beginAndEndEnchereDate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Coiner.Business.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Town");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Coiner.Business.Models.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("PDFPath");

                    b.Property<int>("ProjectId");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("Coiner.Business.Models.Coin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("CoinPrice");

                    b.Property<int>("CoinStatus");

                    b.Property<decimal>("CoinValue");

                    b.Property<int>("CoinsMonetizedNumber");

                    b.Property<int>("CoinsNumber");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("ProjectId");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UsedNumber");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Coins");
                });

            modelBuilder.Entity("Coiner.Business.Models.Discussion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AnswerContent");

                    b.Property<DateTime>("AnswerCreation");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("ProjectId");

                    b.Property<string>("QuestionContent");

                    b.Property<DateTime>("QuestionCreation");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Discussions");
                });

            modelBuilder.Entity("Coiner.Business.Models.Document", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("Extention");

                    b.Property<string>("Path");

                    b.Property<int>("ProjectId");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NotificationOutputType");

                    b.Property<int>("NotificationUpdateFrequency");

                    b.Property<int>("SendDay");

                    b.Property<DateTimeOffset>("SendTime");

                    b.HasKey("Id");

                    b.ToTable("NotificationConfiguration");
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationProduced", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AppReadStatus");

                    b.Property<DateTimeOffset>("AppReadTime");

                    b.Property<string>("Content");

                    b.Property<DateTimeOffset>("CreateDate");

                    b.Property<int>("NotificationOutputType");

                    b.Property<int>("NotificationTemplateId");

                    b.Property<int>("ProjectId");

                    b.Property<string>("Title");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("NotificationTemplateId");

                    b.ToTable("NotificationProduced");
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationSent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NotificationProducedId");

                    b.Property<string>("SendEmail");

                    b.Property<string>("SendPhone");

                    b.Property<bool>("SendStatus");

                    b.Property<DateTimeOffset>("SendTime");

                    b.HasKey("Id");

                    b.HasIndex("NotificationProducedId")
                        .IsUnique();

                    b.ToTable("NotificationSent");
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<int>("NotificationCategory");

                    b.Property<int>("NotificationConfigurationId");

                    b.Property<int>("NotificationType");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("NotificationConfigurationId")
                        .IsUnique();

                    b.ToTable("NotificationTemplate");
                });

            modelBuilder.Entity("Coiner.Business.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivityType");

                    b.Property<DateTime?>("BeginEnchereDate");

                    b.Property<DateTime?>("BeginEstimatedDate");

                    b.Property<string>("BusinessPlan");

                    b.Property<int>("Career_EngagementYears");

                    b.Property<int>("CommissionTokenStirblock");

                    b.Property<DateTime>("CreationDate");

                    b.Property<DateTime?>("EndEnchereDate");

                    b.Property<decimal>("FundingGoal");

                    b.Property<int>("FundraisingPeriod");

                    b.Property<decimal>("PercentageAsset");

                    b.Property<string>("ProductName");

                    b.Property<string>("Product_BillDescription");

                    b.Property<DateTime?>("Product_SalesPercepective");

                    b.Property<decimal>("Product_TVA");

                    b.Property<string>("ProjectAddress");

                    b.Property<string>("ProjectDescription");

                    b.Property<string>("ProjectName");

                    b.Property<string>("ProjectPresentation");

                    b.Property<int>("ProjectStatus");

                    b.Property<int>("ProjectType");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<decimal>("ReceivedFunding");

                    b.Property<DateTime?>("Society_CreationDate");

                    b.Property<string>("Society_LegaleIdentification");

                    b.Property<string>("Society_Name");

                    b.Property<string>("Society_StructureType");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.Property<DateTime?>("ValidationDate");

                    b.Property<string>("WebLink");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Coiner.Business.Models.ProjectImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("Path");

                    b.Property<int>("ProjectId");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectImages");
                });

            modelBuilder.Entity("Coiner.Business.Models.ProjectUpdate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("NewsContent");

                    b.Property<int>("ProjectId");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectUpdate");
                });

            modelBuilder.Entity("Coiner.Business.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActivationToken");

                    b.Property<string>("Address");

                    b.Property<DateTime?>("BirthDay");

                    b.Property<string>("BlockChainAddress");

                    b.Property<string>("BlockChainPublicKey");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<int>("Gender");

                    b.Property<int>("IsActive");

                    b.Property<string>("Job");

                    b.Property<bool>("Kyc");

                    b.Property<bool>("KycNotificationSent");

                    b.Property<string>("LastName");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<int>("Provider");

                    b.Property<string>("Siren");

                    b.Property<string>("Tva");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserCoinsNumber");

                    b.Property<int>("UserType");

                    b.Property<string>("WalletId");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Coiner.Business.Models.UserImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("Path");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserImage");
                });

            modelBuilder.Entity("Coiner.Business.Models.UserWallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("DisappeardCoinsNumber");

                    b.Property<int>("FirstUsedCoinsNumber");

                    b.Property<int>("RemovedCoinsNumber");

                    b.Property<int>("SecondUsedCoinsNumber");

                    b.Property<int>("ThirdUsedCoinsNumber");

                    b.Property<int>("UnusedCoinsNumber");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserWallet");
                });

            modelBuilder.Entity("Coiner.Business.Models.Address", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.Bill", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("Bills")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithMany("Bills")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.Coin", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("Coins")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithMany("Coins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.Discussion", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("Discussions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.Document", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("Documents")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationProduced", b =>
                {
                    b.HasOne("Coiner.Business.Models.NotificationTemplate", "NotificationTemplate")
                        .WithMany("NotificationsProduced")
                        .HasForeignKey("NotificationTemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationSent", b =>
                {
                    b.HasOne("Coiner.Business.Models.NotificationProduced", "NotificationProduced")
                        .WithOne("NotificationSent")
                        .HasForeignKey("Coiner.Business.Models.NotificationSent", "NotificationProducedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.NotificationTemplate", b =>
                {
                    b.HasOne("Coiner.Business.Models.NotificationConfiguration", "NotificationConfiguration")
                        .WithOne("NotificationTemplate")
                        .HasForeignKey("Coiner.Business.Models.NotificationTemplate", "NotificationConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.Project", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithMany("Projects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.ProjectImage", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("ProjectImages")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.ProjectUpdate", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project")
                        .WithMany("ProjectUpdates")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.UserImage", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithOne("UserImage")
                        .HasForeignKey("Coiner.Business.Models.UserImage", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Coiner.Business.Models.UserWallet", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithOne("UserWallet")
                        .HasForeignKey("Coiner.Business.Models.UserWallet", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
