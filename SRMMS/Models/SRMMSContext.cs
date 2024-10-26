using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SRMMS.Models
{
    public partial class SRMMSContext : DbContext
    {
        public SRMMSContext()
        {
        }

        public SRMMSContext(DbContextOptions<SRMMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Combo> Combos { get; set; } = null!;
        public virtual DbSet<ComboDetail> ComboDetails { get; set; } = null!;
        public virtual DbSet<DiscountCode> DiscountCodes { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<PointList> PointLists { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database = SRMMS;uid=sa;pwd=sa123;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccId);

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .HasMaxLength(50)
                    .HasColumnName("full_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Accounts_Role");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.Property(e => e.CatId).HasColumnName("cat_id");

                entity.Property(e => e.CatName)
                    .HasMaxLength(50)
                    .HasColumnName("cat_name");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Combo>(entity =>
            {
                entity.ToTable("Combo");

                entity.Property(e => e.ComboId).HasColumnName("combo_id");

                entity.Property(e => e.ComboDiscription).HasColumnName("combo_discription");

                entity.Property(e => e.ComboImg).HasColumnName("combo_img");

                entity.Property(e => e.ComboMoney)
                    .HasColumnType("money")
                    .HasColumnName("combo_money");

                entity.Property(e => e.ComboName)
                    .HasMaxLength(200)
                    .HasColumnName("combo_name");

                entity.Property(e => e.ComboStatus).HasColumnName("combo_status");
            });

            modelBuilder.Entity<ComboDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Combo_Detail");

                entity.Property(e => e.ComboId).HasColumnName("combo_id");

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.HasOne(d => d.Combo)
                    .WithMany()
                    .HasForeignKey(d => d.ComboId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Combo Detail_Combo");

                entity.HasOne(d => d.Pro)
                    .WithMany()
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK_Combo Detail_Menu");
            });

            modelBuilder.Entity<DiscountCode>(entity =>
            {
                entity.HasKey(e => e.CodeId);

                entity.ToTable("Discount_code");

                entity.Property(e => e.CodeId)
                    .ValueGeneratedNever()
                    .HasColumnName("code_id");

                entity.Property(e => e.CodeDetail)
                    .HasMaxLength(250)
                    .HasColumnName("code_detail");

                entity.Property(e => e.DiscountValue).HasColumnName("discount_value");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("end_date");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Feedback");

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Feedback1)
                    .HasMaxLength(300)
                    .HasColumnName("feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");

                entity.Property(e => e.RateStar).HasColumnName("rate_star");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Acc)
                    .WithMany()
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK_Feedback_Accounts");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.CodeId).HasColumnName("code_id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");

                entity.Property(e => e.Staus).HasColumnName("staus");

                entity.Property(e => e.TotalMoney)
                    .HasColumnType("money")
                    .HasColumnName("totalMoney");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK_Order_Accounts");

                entity.HasOne(d => d.Code)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CodeId)
                    .HasConstraintName("FK_Order_Discount_code");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order_Details");

                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.Quantiity).HasColumnName("quantiity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Order_Details_Order");

                entity.HasOne(d => d.Pro)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<PointList>(entity =>
            {
                entity.HasKey(e => e.PointId);

                entity.ToTable("Point_List");

                entity.Property(e => e.PointId)
                    .ValueGeneratedNever()
                    .HasColumnName("point_id");

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.NumberPonit).HasColumnName("number_ponit");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.PointLists)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK_Point_List_Accounts");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProId)
                    .HasName("PK_Menu");

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.CatId).HasColumnName("cat_id");

                entity.Property(e => e.ProCalories)
                    .HasMaxLength(250)
                    .HasColumnName("pro_calories");

                entity.Property(e => e.ProDiscription).HasColumnName("pro_discription");

                entity.Property(e => e.ProImg).HasColumnName("pro_img");

                entity.Property(e => e.ProName)
                    .HasMaxLength(200)
                    .HasColumnName("pro_name");

                entity.Property(e => e.ProPrice)
                    .HasColumnType("money")
                    .HasColumnName("pro_price");

                entity.Property(e => e.ProStatus).HasColumnName("pro_status");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("FK_Menu_Categories");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .HasColumnName("description");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.ToTable("Table");

                entity.Property(e => e.TableId)
                    .ValueGeneratedNever()
                    .HasColumnName("table_id");

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.QrCode)
                    .HasMaxLength(50)
                    .HasColumnName("QR_code");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TableName)
                    .HasMaxLength(50)
                    .HasColumnName("table_name");

                entity.Property(e => e.TimeBooking)
                    .HasColumnType("date")
                    .HasColumnName("time_booking");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Tables)
                    .HasForeignKey(d => d.AccId)
                    .HasConstraintName("FK_Table_Accounts");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
