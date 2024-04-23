using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Common;

namespace HomeStay.Models
{
    public partial class HomestayDBContext : DbContext
    {
        public HomestayDBContext()
        {
        }

        public HomestayDBContext(DbContextOptions<HomestayDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<FavouriteRoom> FavouriteRooms { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomImagesDetail> RoomImagesDetails { get; set; } = null!;
        public virtual DbSet<ViewAccount> ViewAccounts { get; set; } = null!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=HaiTrieu;Initial Catalog=HomestayDB;Integrated Security=True");
            }
        }
        // Home Controller
        public List<Room> GetListRoomActive()
        {
            return this.Rooms.FromSqlRaw<Room>("EXECUTE GetActiveRooms").ToList();
        }

        public async Task<List<Booking>> GetHistoryBooking(int customerId)
        {
            SqlParameter parameter = new SqlParameter("@CustomerId", customerId);
            return await this.Bookings.FromSqlRaw<Booking>("EXECUTE GetHistoryBooking @CustomerId", parameter).ToListAsync();
        }

        // Area
        // AdminRoleController
        public async Task<Role> GetRoleById(int? Id)
        {
            var roles = await this.Roles.FromSqlRaw<Role>("EXECUTE GetRoleById @Id", new SqlParameter("@Id", Id)).ToListAsync();
            return roles.FirstOrDefault();
        }

        public void CreateRole(Role role)
        {
            this.Database.ExecuteSqlRaw("EXECUTE CreateRole @name", new SqlParameter("@name", role.RoleName));
        }

        public void UpdateRole(Role role)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter{ ParameterName= "@id" , Value = role.RoleId},
                new SqlParameter{ ParameterName= "@name" , Value = role.RoleName},
            };
            this.Database.ExecuteSqlRaw("EXECUTE UpdateRole @id, @name", sqlParameters.ToArray());
        }

        public void DeleteRole(int id)
        {
            this.Database.ExecuteSqlRaw("EXECUTE DeleteRole @id", new SqlParameter("@id", id));
        }

        //Account Controller
        public List<ViewAccount> GetListAccount()
        {
            return this.ViewAccounts.FromSqlRaw<ViewAccount>("EXECUTE GetListAccount").ToList();
        }

        public Account GetAccountById(int? id)
        {
            var account = this.Accounts.FromSqlRaw<Account>("EXECUTE GetAccountById @id", new SqlParameter("@Id", id)).ToList();
            return account.FirstOrDefault();
        }

        public ViewAccount GetAccountVsCateGoryById(int? id)
        {
            var account = this.ViewAccounts.FromSqlRaw<ViewAccount>("EXECUTE GetAccountVsCateGoryById @id", new SqlParameter("@Id", id)).ToList();
            return account.FirstOrDefault();
        }

        public void CreateAccount(Account acc)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@RoleId", acc.RoleId),
                new SqlParameter("@AccountName", acc.AccountName),
                new SqlParameter("@Email", acc.Email),
                new SqlParameter("@PhoneNumber", acc.PhoneNumber),
                new SqlParameter("@Active", acc.Active),
                new SqlParameter("@Password", acc.Password),
                new SqlParameter("@Cccd", acc.Cccd)
            };
            this.Database.ExecuteSqlRaw("EXECUTE CreateAccount @RoleId, @AccountName, @Email, @PhoneNumber, @Active, @Password, @Cccd ", sqlParameters.ToArray());
        }

        public void UpdateAccount(Account acc)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Accountid", acc.AccountId),
                new SqlParameter("@RoleId", acc.RoleId),
                new SqlParameter("@AccountName", acc.AccountName),
                new SqlParameter("@Email", acc.Email),
                new SqlParameter("@PhoneNumber", acc.PhoneNumber),
                new SqlParameter("@Active", acc.Active),
                new SqlParameter("@Password", acc.Password),
                new SqlParameter("@Cccd", acc.Cccd)
            };
            this.Database.ExecuteSqlRaw("EXECUTE UpdateAccount @Accountid, @RoleId, @AccountName, @Email, @PhoneNumber, @Active, @Password, @Cccd ", sqlParameters.ToArray());
        }

        public void DeleteAccount(int? id)
        {
            this.Database.ExecuteSqlRaw("EXECUTE DeleteAccount @Id", new SqlParameter("@Id", id));
        }

        // Customer Controller
        public List<Customer> GetListCustomer()
        {
            return this.Customers.FromSqlRaw<Customer>("EXECUTE GetListCustomer").ToList();
        }

        public Customer GetCustomerById(int? id)
        {
            var customers = this.Customers.FromSqlRaw<Customer>("EXECUTE GetCustomerById @id", new SqlParameter("@Id", id)).ToList();
            return customers.FirstOrDefault();
        }

        public void CreateCustomer(Customer customer)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Birthday", customer.Birthday),
                new SqlParameter("@Avatar", customer.Avatar),
                new SqlParameter("@Address", customer.Address),
                new SqlParameter("@PhoneNumber", customer.PhoneNumber),
                new SqlParameter("@Active", customer.Active),
                new SqlParameter("@Password", customer.Password),
                new SqlParameter("@Email", customer.Email)
            };

            Database.ExecuteSqlRaw("EXECUTE CreateCustomer @FullName, @Birthday, @Avatar, @Address, @PhoneNumber, @Active, @Password, @Email", sqlParameters.ToArray());
        }

        public void UpdateCustomer(Customer customer)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", customer.CustomerId),
                new SqlParameter("@FullName", customer.FullName),
                new SqlParameter("@Birthday", customer.Birthday),
                new SqlParameter("@Avatar", customer.Avatar),
                new SqlParameter("@Address", customer.Address),
                new SqlParameter("@PhoneNumber", customer.PhoneNumber),
                new SqlParameter("@Active", customer.Active),
                new SqlParameter("@Password", customer.Password),
                new SqlParameter("@Email", customer.Email)
            };

            Database.ExecuteSqlRaw("EXECUTE UpdateCustomer @CustomerId, @FullName, @Birthday, @Avatar, @Address, @PhoneNumber, @Active, @Password, @Email", sqlParameters.ToArray());
        }

        public void DeleteCustomer(int? id)
        {
            this.Database.ExecuteSqlRaw("EXECUTE DeleteCustomer @CustomerId", new SqlParameter("@CustomerId", id));
        }

        // Area Room controller

        public List<Room> GetListRoom(int? status, int? categoryID, string searchValue = "")
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Active", status.HasValue ? (object)status : DBNull.Value) ,
                new SqlParameter("@CategoryId", categoryID.HasValue ? (object)categoryID : DBNull.Value),
                new SqlParameter("@searchValue", searchValue),
            };
            return this.Rooms.FromSqlRaw("EXECUTE GetListRoom @Active, @CategoryId, @searchValue", sqlParameters.ToArray()).ToList();
        }

        public Room GetRoomById(int? id)
        {
            var room = this.Rooms.FromSqlRaw<Room>("EXECUTE GetRoomById @RoomId", new SqlParameter("@RoomId", id)).ToList();
            return room.FirstOrDefault();
        }

        public void CreateRoom(Room room)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Title", room.Title),
                new SqlParameter("@Detail", room.Detail),
                new SqlParameter("@Price", room.Price),
                new SqlParameter("@Area", room.Area),
                new SqlParameter("@Capacity", room.Capacity),
                new SqlParameter("@Description", room.Description),
                new SqlParameter("@Active", room.Active),
                new SqlParameter("@Status", room.Status),
                new SqlParameter("@CategoryId", room.CategoryId),
                new SqlParameter("@Avatar", room.Avatar),
              //  new SqlParameter("@createAt", room.CreateAt)
              //@createAt
            };

            this.Database.ExecuteSqlRaw("EXECUTE CreateRoom @Title, @Detail, @Price, @Area, @Capacity, @Description, @Active, @Status, @CategoryId, @Avatar ", parameters.ToArray());
        }

        public void UpdateRoom(Room room)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@RoomId", room.RoomId),
                new SqlParameter("@Title", room.Title),
                new SqlParameter("@Detail", room.Detail),
                new SqlParameter("@Price", room.Price),
                new SqlParameter("@Area", room.Area),
                new SqlParameter("@Capacity", room.Capacity),
                new SqlParameter("@Description", room.Description),
                new SqlParameter("@Active", room.Active),
                new SqlParameter("@Status", room.Status),
                new SqlParameter("@CategoryId", room.CategoryId),
                new SqlParameter("@Avatar", room.Avatar)
            };
            this.Database.ExecuteSqlRaw("EXECUTE UpdateRoom @RoomId, @Title, @Detail, @Price, @Area, @Capacity, @Description, @Active, @Status, @CategoryId, @Avatar", parameters.ToArray());
        }

        public void DeleteRoom(int? id)
        {
            this.Database.ExecuteSqlRaw("EXECUTE DeleteRoom @RoomId", new SqlParameter("@RoomId", id));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ViewAccount>(e =>
            {
                // Không có khóa chính
                e.HasNoKey();
            });
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountName).HasMaxLength(100);

                entity.Property(e => e.Cccd).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.CheckIn).HasColumnType("date");

                entity.Property(e => e.CheckOut).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Customer");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Payment");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Booking_Room");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.CategoryName).HasMaxLength(255);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Avatar).HasMaxLength(255);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<FavouriteRoom>(entity =>
            {
                entity.ToTable("FavouriteRoom");

                entity.Property(e => e.Area).HasMaxLength(255);

                entity.Property(e => e.Avatar).HasMaxLength(100);

                entity.Property(e => e.Detail).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.FavouriteRooms)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteRoom_Customer");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.FavouriteRooms)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FavouriteRoom_Booking");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.BankName).HasMaxLength(100);

                entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Rating");

                entity.Property(e => e.Comment).HasMaxLength(255);

                entity.Property(e => e.Rating1).HasColumnName("Rating");

                entity.HasOne(d => d.Booking)
                    .WithMany()
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_Rating_Booking");

                entity.HasOne(d => d.Customer)
                    .WithMany()
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Rating_Customer");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.Area).HasMaxLength(255);
               // entity.Property(e => e.CreateAt)
                 //     .HasColumnType("datetime")
                    //  .HasColumnName("createAt");
                entity.Property(e => e.Avatar).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Detail).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Room_Category");
            });

            modelBuilder.Entity<RoomImagesDetail>(entity =>
            {
                entity.HasKey(e => e.RoomId)
                    .HasName("PK__RoomImag__3286393944D18528");

                entity.ToTable("RoomImagesDetail");

                entity.Property(e => e.RoomId).ValueGeneratedNever();

                entity.Property(e => e.Images).HasMaxLength(255);

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Room)
                    .WithOne(p => p.RoomImagesDetail)
                    .HasForeignKey<RoomImagesDetail>(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ImageDetail_Room");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        // HomeController 
    }
}