using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HomeStay.Models
{
    public partial class BUIHAITRIEUSACHDBContext : DbContext
    {
        public BUIHAITRIEUSACHDBContext()
        {
        }

        public BUIHAITRIEUSACHDBContext(DbContextOptions<BUIHAITRIEUSACHDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Sach> Saches { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=HaiTrieu;Initial Catalog=BUIHAITRIEUSACHDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sach>(entity =>
            {
                entity.HasKey(e => e.PkSachId)
                    .HasName("PK__Sach__9EDAB489D9CBE2E2");

                entity.ToTable("Sach");

                entity.Property(e => e.PkSachId).HasColumnName("PK_SachId");

                entity.Property(e => e.TacGia).HasMaxLength(255);

                entity.Property(e => e.TenSach).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
