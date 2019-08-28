using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BombinoBomberBot.Model
{
    public class BomberBotContext : DbContext
    {
        public BomberBotContext(DbContextOptions<BomberBotContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ForNpgsqlUseIdentityAlwaysColumns();
            builder.Entity<Room>().HasIndex(x => x.TelegramChatId).IsUnique();
            builder.Entity<Room>().HasMany(x => x.Users);
            
            builder.Entity<User>().HasIndex(x => x.TelegramUserId).IsUnique();
            builder.Entity<User>().HasMany(x => x.Rooms);
            builder.Entity<User>().HasMany(x => x.UserStats);

            builder.Entity<RoomUser>().HasKey(x => new { x.RoomId, x.UserId });
            builder.Entity<RoomUser>().HasIndex(x => x.RoomId);
            builder.Entity<RoomUser>().HasIndex(x => x.UserId);
            builder.Entity<RoomUser>()
                   .HasOne(cm => cm.Room)
                   .WithMany(c => c.Users)
                   .HasForeignKey(m => m.RoomId);
            builder.Entity<RoomUser>()
                   .HasOne(cm => cm.User)
                   .WithMany(m => m.Rooms)
                   .HasForeignKey(c => c.UserId);

            builder.Entity<UserStats>().HasKey(x => new { x.RoomId, x.UserId });
            builder.Entity<UserStats>().HasIndex(x => x.RoomId);
            builder.Entity<UserStats>().HasOne(x => x.Room);
            builder.Entity<UserStats>().HasOne(x => x.User).WithMany(x => x.UserStats);
        }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RoomUser> RoomUsers { get; set; }

        public DbSet<UserStats> UserStats { get; set; }
    }

    public class Room
    {
        public Room()
        {
            Users = new List<RoomUser>();
        }

        public int Id { get; set; }

        public long TelegramChatId { get; set; }

        public IList<RoomUser> Users { get; set; }

        public int Trolls { get; set; }

        public string Title { get; set; }
    }
    
    public class UserStats
    {
        public int RoomId { get; set; }

        public Room Room { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int Wins { get; set; }
    }

    public class User
    {
        public User()
        {
            Rooms = new List<RoomUser>();
            UserStats = new List<UserStats>();
        }

        public int Id { get; set; }

        public int TelegramUserId { get; set; }

        public IList<RoomUser> Rooms { get; set; }

        public IList<UserStats> UserStats { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class RoomUser
    {
        public int RoomId { get; set; } 

        public Room Room { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}