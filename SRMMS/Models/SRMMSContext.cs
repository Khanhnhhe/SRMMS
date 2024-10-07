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
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DiscountCode> DiscountCodes { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<IngCategory> IngCategories { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<RestaurantInformation> RestaurantInformations { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<TopicOfPost> TopicOfPosts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=(local);database=SRMMS;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccId);

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.CusId).HasColumnName("cus_id");

                entity.Property(e => e.EmpId).HasColumnName("emp_id");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.HasOne(d => d.Cus)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.CusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accounts_Customers");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.EmpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accounts_Employees");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("FK_Accounts_Tables");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.Property(e => e.CatId).HasColumnName("cat_id");

                entity.Property(e => e.CatName)
                    .HasMaxLength(50)
                    .HasColumnName("cat_name");
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
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Combo Detail_Menu");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CusId);

                entity.Property(e => e.CusId).HasColumnName("cus_id");

                entity.Property(e => e.CusFullname)
                    .HasMaxLength(50)
                    .HasColumnName("cus_fullname");

                entity.Property(e => e.CusPhone).HasColumnName("cus_phone");
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

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.DiscountCodes)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_Discount_code_Order");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.Property(e => e.EmpId).HasColumnName("emp_id");

                entity.Property(e => e.EmpAddress)
                    .HasMaxLength(150)
                    .HasColumnName("emp_address");

                entity.Property(e => e.EmpDob)
                    .HasColumnType("date")
                    .HasColumnName("emp_dob");

                entity.Property(e => e.EmpEmail)
                    .HasMaxLength(50)
                    .HasColumnName("emp_email");

                entity.Property(e => e.EmpEndDate)
                    .HasColumnType("date")
                    .HasColumnName("emp_endDate");

                entity.Property(e => e.EmpFirstName)
                    .HasMaxLength(50)
                    .HasColumnName("emp_first_name");

                entity.Property(e => e.EmpGender).HasColumnName("emp_gender");

                entity.Property(e => e.EmpLastName)
                    .HasMaxLength(50)
                    .HasColumnName("emp_last_name");

                entity.Property(e => e.EmpPassword)
                    .HasMaxLength(30)
                    .HasColumnName("emp_password");

                entity.Property(e => e.EmpPhoneNumber).HasColumnName("emp_phoneNumber");

                entity.Property(e => e.EmpRoleId).HasColumnName("emp_role_id");

                entity.Property(e => e.EmpStartDate)
                    .HasColumnType("date")
                    .HasColumnName("emp_startDate");

                entity.Property(e => e.EmpStatus).HasColumnName("emp_status");

                entity.Property(e => e.EmpWard)
                    .HasMaxLength(50)
                    .HasColumnName("emp_ward");

                entity.HasOne(d => d.EmpRole)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.EmpRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Role");
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

                entity.Property(e => e.Feedback1).HasColumnName("feedback");

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.Ratestar).HasColumnName("ratestar");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Acc)
                    .WithMany()
                    .HasForeignKey(d => d.AccId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Accounts");

                entity.HasOne(d => d.Pro)
                    .WithMany()
                    .HasForeignKey(d => d.ProId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Menu");
            });

            modelBuilder.Entity<IngCategory>(entity =>
            {
                entity.HasKey(e => e.CateId);

                entity.ToTable("Ing_Categories");

                entity.Property(e => e.CateId).HasColumnName("cate_id");

                entity.Property(e => e.CateName)
                    .HasMaxLength(50)
                    .HasColumnName("cate_name");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.IngId);

                entity.Property(e => e.IngId).HasColumnName("ing_id");

                entity.Property(e => e.CateId).HasColumnName("cate_id");

                entity.Property(e => e.Discription).HasColumnName("discription");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("date")
                    .HasColumnName("expiration_date");

                entity.Property(e => e.IngInsertDate)
                    .HasColumnType("date")
                    .HasColumnName("ing_insert_date");

                entity.Property(e => e.IngName)
                    .HasMaxLength(50)
                    .HasColumnName("ing_name");

                entity.Property(e => e.IngPrice)
                    .HasColumnType("money")
                    .HasColumnName("ing_price");

                entity.Property(e => e.IngQuantity).HasColumnName("ing_quantity");

                entity.HasOne(d => d.Cate)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.CateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ingredients_Ing_Categories");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.AccId).HasColumnName("acc_id");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("date")
                    .HasColumnName("order_date");

                entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");

                entity.Property(e => e.TotalMoney)
                    .HasColumnType("money")
                    .HasColumnName("totalMoney");

                entity.HasOne(d => d.Acc)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Accounts");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Status");
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
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Order");

                entity.HasOne(d => d.Pro)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.PostId).HasColumnName("post_id");

                entity.Property(e => e.EmpPostId)
                    .HasMaxLength(50)
                    .HasColumnName("emp_post_id");

                entity.Property(e => e.PostDate)
                    .HasColumnType("date")
                    .HasColumnName("post_date");

                entity.Property(e => e.PostDetail).HasColumnName("post_detail");

                entity.Property(e => e.PostImg).HasColumnName("post_img");

                entity.Property(e => e.PostTitle)
                    .HasMaxLength(50)
                    .HasColumnName("post_title");

                entity.Property(e => e.PostToppicId).HasColumnName("post_toppic_id");

                entity.HasOne(d => d.PostToppic)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PostToppicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_Employees");

                entity.HasOne(d => d.PostToppicNavigation)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PostToppicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_TopicOfPost");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProId)
                    .HasName("PK_Menu");

                entity.Property(e => e.ProId).HasColumnName("pro_id");

                entity.Property(e => e.CatId).HasColumnName("cat_id");

                entity.Property(e => e.IngId).HasColumnName("ing_id");

                entity.Property(e => e.ProCalories)
                    .HasMaxLength(250)
                    .HasColumnName("pro_calories");

                entity.Property(e => e.ProCookingTime).HasColumnName("pro_cooking_time");

                entity.Property(e => e.ProDiscription).HasColumnName("pro_discription");

                entity.Property(e => e.ProImg).HasColumnName("pro_img");

                entity.Property(e => e.ProName)
                    .HasMaxLength(200)
                    .HasColumnName("pro_name");

                entity.Property(e => e.ProPrice)
                    .HasColumnType("money")
                    .HasColumnName("pro_price");

                entity.Property(e => e.ProStatus).HasColumnName("pro_status");

                entity.Property(e => e.ProWarning).HasColumnName("pro_warning");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Menu_Categories");

                entity.HasOne(d => d.Ing)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.IngId)
                    .HasConstraintName("FK_Menu_Ingredients");
            });

            modelBuilder.Entity<RestaurantInformation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Restaurant_Information");

                entity.Property(e => e.ResAdress)
                    .HasMaxLength(200)
                    .HasColumnName("res_adress");

                entity.Property(e => e.ResEmail).HasColumnName("res_email");

                entity.Property(e => e.ResFacebook).HasColumnName("res_facebook");

                entity.Property(e => e.ResName).HasColumnName("res_name");

                entity.Property(e => e.ResPhone).HasColumnName("res_phone");
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

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.TableName)
                    .HasMaxLength(50)
                    .HasColumnName("table_name");

                entity.Property(e => e.TableQrcode).HasColumnName("table_QRCode");
            });

            modelBuilder.Entity<TopicOfPost>(entity =>
            {
                entity.HasKey(e => e.TopicId);

                entity.ToTable("TopicOfPost");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.TopicName)
                    .HasMaxLength(50)
                    .HasColumnName("topic_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
