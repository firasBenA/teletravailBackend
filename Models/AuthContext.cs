using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestApi.Entities;

namespace TestApi.Models
{
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public virtual DbSet<Boat> Boats { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<FeedBack> FeedBacks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<Boat>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.IdFeedBack).HasColumnName("idFeedBack");
                entity.Property(e => e.Image).HasColumnType("image");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdFeedBackNavigation).WithMany(p => p.Boats)
                    .HasForeignKey(d => d.IdFeedBack)
                    .HasConstraintName("FK_Boats_FeedBack");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chat");

                entity.Property(e => e.Date).HasColumnType("date");
                entity.Property(e => e.Message)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Table_1");

                entity.ToTable("FeedBack");

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_User_1");

                entity.ToTable("User");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.HasOne(d => d.IdBoatNavigation).WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdBoat)
                    .HasConstraintName("FK_User_Boats");

                entity.HasOne(d => d.IdChatNavigation).WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdChat)
                    .HasConstraintName("FK_User_Chat");

                entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("FK_User_Role");
            });

            // Call the base class method to apply configurations from IdentityDbContext
            base.OnModelCreating(modelBuilder);
        }
    }
}
