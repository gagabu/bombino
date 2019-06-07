﻿// <auto-generated />
using BombinoBomberBot.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BombinoBomberBot.Model.Migrations
{
    [DbContext(typeof(BomberBotContext))]
    [Migration("20190607114103_UserStats")]
    partial class UserStats
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BombinoBomberBot.Model.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("TelegramChatId");

                    b.Property<string>("Title");

                    b.Property<int>("Trolls");

                    b.HasKey("Id");

                    b.HasIndex("TelegramChatId")
                        .IsUnique();

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("BombinoBomberBot.Model.RoomUser", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<int>("UserId");

                    b.HasKey("RoomId", "UserId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("RoomUsers");
                });

            modelBuilder.Entity("BombinoBomberBot.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<int>("TelegramUserId");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("TelegramUserId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BombinoBomberBot.Model.UserStats", b =>
                {
                    b.Property<int>("RoomId");

                    b.Property<int>("UserId");

                    b.Property<int>("Wins");

                    b.HasKey("RoomId", "UserId");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("UserStats");
                });

            modelBuilder.Entity("BombinoBomberBot.Model.RoomUser", b =>
                {
                    b.HasOne("BombinoBomberBot.Model.Room", "Room")
                        .WithMany("Users")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BombinoBomberBot.Model.User", "User")
                        .WithMany("Rooms")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BombinoBomberBot.Model.UserStats", b =>
                {
                    b.HasOne("BombinoBomberBot.Model.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BombinoBomberBot.Model.User", "User")
                        .WithMany("UserStats")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}