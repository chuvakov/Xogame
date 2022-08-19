﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using XOgame.Core;

#nullable disable

namespace XOgame.Core.Migrations
{
    [DbContext(typeof(XOgameContext))]
    [Migration("20220813090426_AddLoginAndPassword")]
    partial class AddLoginAndPassword
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("XOgame.Core.Models.Emoji", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("EmojiGroupId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmojiGroupId");

                    b.ToTable("Emojis");
                });

            modelBuilder.Entity("XOgame.Core.Models.EmojiGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EmojiGroups");
                });

            modelBuilder.Entity("XOgame.Core.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("RoomId")
                        .HasColumnType("integer");

                    b.Property<int?>("UserTurnId")
                        .HasColumnType("integer");

                    b.Property<int?>("WinnerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserTurnId");

                    b.HasIndex("WinnerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("XOgame.Core.Models.GameProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CellNumber")
                        .HasColumnType("integer");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("GameProgresses");
                });

            modelBuilder.Entity("XOgame.Core.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CurrentGameId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CurrentGameId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("XOgame.Core.Models.SettingsSound", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsEnabledDraw")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabledLose")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabledStartGame")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabledStep")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsEnabledWin")
                        .HasColumnType("boolean");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("SettingsSounds");
                });

            modelBuilder.Entity("XOgame.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CurrentRoomId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsReady")
                        .HasColumnType("boolean");

                    b.Property<string>("Login")
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("PathToAvatar")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CurrentRoomId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("XOgame.Core.Models.UserGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FigureType")
                        .HasColumnType("integer");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGames");
                });

            modelBuilder.Entity("XOgame.Core.Models.Emoji", b =>
                {
                    b.HasOne("XOgame.Core.Models.EmojiGroup", "EmojiGroup")
                        .WithMany()
                        .HasForeignKey("EmojiGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmojiGroup");
                });

            modelBuilder.Entity("XOgame.Core.Models.Game", b =>
                {
                    b.HasOne("XOgame.Core.Models.Room", "Room")
                        .WithMany("Games")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("XOgame.Core.Models.User", "UserTurn")
                        .WithMany()
                        .HasForeignKey("UserTurnId");

                    b.HasOne("XOgame.Core.Models.User", "Winner")
                        .WithMany()
                        .HasForeignKey("WinnerId");

                    b.Navigation("Room");

                    b.Navigation("UserTurn");

                    b.Navigation("Winner");
                });

            modelBuilder.Entity("XOgame.Core.Models.GameProgress", b =>
                {
                    b.HasOne("XOgame.Core.Models.Game", "Game")
                        .WithMany("GameProgresses")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XOgame.Core.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("XOgame.Core.Models.Room", b =>
                {
                    b.HasOne("XOgame.Core.Models.Game", "CurrentGame")
                        .WithMany()
                        .HasForeignKey("CurrentGameId");

                    b.Navigation("CurrentGame");
                });

            modelBuilder.Entity("XOgame.Core.Models.SettingsSound", b =>
                {
                    b.HasOne("XOgame.Core.Models.User", "User")
                        .WithOne("SettingsSound")
                        .HasForeignKey("XOgame.Core.Models.SettingsSound", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("XOgame.Core.Models.User", b =>
                {
                    b.HasOne("XOgame.Core.Models.Room", "CurrentRoom")
                        .WithMany("Users")
                        .HasForeignKey("CurrentRoomId");

                    b.Navigation("CurrentRoom");
                });

            modelBuilder.Entity("XOgame.Core.Models.UserGame", b =>
                {
                    b.HasOne("XOgame.Core.Models.Game", "Game")
                        .WithMany("UserGames")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XOgame.Core.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("XOgame.Core.Models.Game", b =>
                {
                    b.Navigation("GameProgresses");

                    b.Navigation("UserGames");
                });

            modelBuilder.Entity("XOgame.Core.Models.Room", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("XOgame.Core.Models.User", b =>
                {
                    b.Navigation("SettingsSound");
                });
#pragma warning restore 612, 618
        }
    }
}
