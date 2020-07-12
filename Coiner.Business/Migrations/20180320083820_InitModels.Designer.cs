﻿// <auto-generated />
using Coiner.Business.Context;
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
    [Migration("20180320083820_InitModels")]
    partial class InitModels
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

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Addresses");
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

            modelBuilder.Entity("Coiner.Business.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityField");

                    b.Property<DateTime>("CreationDate");

                    b.Property<decimal>("FundingGoal");

                    b.Property<string>("ProjectDescription");

                    b.Property<string>("ProjectName");

                    b.Property<int>("ProjectPeriod");

                    b.Property<int>("ProjectStatus");

                    b.Property<int>("ProjectType");

                    b.Property<DateTime>("PublishDate");

                    b.Property<decimal>("ReceivedFunding");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("ValidationDate");

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

            modelBuilder.Entity("Coiner.Business.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BirthDay");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<int>("Gender");

                    b.Property<string>("Job");

                    b.Property<string>("LastName");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<DateTime>("UpdateDate");

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

            modelBuilder.Entity("Coiner.Business.Models.Address", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithOne("Address")
                        .HasForeignKey("Coiner.Business.Models.Address", "UserId")
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

            modelBuilder.Entity("Coiner.Business.Models.Document", b =>
                {
                    b.HasOne("Coiner.Business.Models.Project", "Project")
                        .WithMany("Documents")
                        .HasForeignKey("ProjectId")
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

            modelBuilder.Entity("Coiner.Business.Models.UserImage", b =>
                {
                    b.HasOne("Coiner.Business.Models.User", "User")
                        .WithOne("UserImage")
                        .HasForeignKey("Coiner.Business.Models.UserImage", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
