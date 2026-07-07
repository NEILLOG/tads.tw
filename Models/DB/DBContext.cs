using Microsoft.EntityFrameworkCore;

namespace TADS_Web.Models.DB
{
    public partial class DBContext : DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public virtual DbSet<TbNews> TbNews { get; set; } = null!;
        public virtual DbSet<TbMemberResearch> TbMemberResearch { get; set; } = null!;
        public virtual DbSet<TbBanner> TbBanner { get; set; } = null!;
        public virtual DbSet<TbAnnualMeeting> TbAnnualMeeting { get; set; } = null!;
        public virtual DbSet<TbPageContent> TbPageContent { get; set; } = null!;
        public virtual DbSet<TbAboutContent> TbAboutContent { get; set; } = null!;
        public virtual DbSet<TbAboutPricing> TbAboutPricing { get; set; } = null!;
        public virtual DbSet<TbFileInfo> TbFileInfo { get; set; } = null!;
        public virtual DbSet<TbIdSummary> TbIdSummary { get; set; } = null!;
        public virtual DbSet<TbLog> TbLog { get; set; } = null!;
        public virtual DbSet<TbLoginRecord> TbLoginRecord { get; set; } = null!;
        public virtual DbSet<TbApiLog> TbApiLog { get; set; } = null!;
        public virtual DbSet<TbBackendOperateLog> TbBackendOperateLog { get; set; } = null!;
        public virtual DbSet<TbUser> TbUser { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbNews>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(10).HasColumnName("ID");
                entity.Property(e => e.Category).HasMaxLength(10);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.FileId).HasMaxLength(15).HasColumnName("FileID");
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<TbMemberResearch>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(10).HasColumnName("ID");
                entity.Property(e => e.Title).HasMaxLength(500);
                entity.Property(e => e.FileId).HasMaxLength(15).HasColumnName("FileID");
                entity.Property(e => e.AuthorInfo).HasMaxLength(300);
                entity.Property(e => e.IntroTitle).HasMaxLength(500);
                entity.Property(e => e.Citation).HasMaxLength(1000);
                entity.Property(e => e.AuthorBioName).HasMaxLength(200);
                entity.Property(e => e.AuthorBioPosition).HasMaxLength(300);
                entity.Property(e => e.AuthorBioExpertise).HasMaxLength(1000);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbBanner>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(10).HasColumnName("ID");
                entity.Property(e => e.FileId).HasMaxLength(15).HasColumnName("FileID");
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbAnnualMeeting>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(10).HasColumnName("ID");
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbPageContent>(entity =>
            {
                entity.HasKey(e => e.PageCode).HasName("PK_TbPageContent");
                entity.Property(e => e.PageCode).HasMaxLength(30);
                entity.Property(e => e.PageName).HasMaxLength(50);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbAboutContent>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TbAboutContent");
                entity.Property(e => e.Id).HasMaxLength(20);
                entity.Property(e => e.IntroImageFileId).HasMaxLength(15);
                entity.Property(e => e.OrgChartFileId).HasMaxLength(15);
                entity.Property(e => e.ConstitutionPdfFileId).HasMaxLength(15);
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbAboutPricing>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_TbAboutPricing");
                entity.Property(e => e.Id).HasMaxLength(10);
                entity.Property(e => e.NameZh).HasMaxLength(100);
                entity.Property(e => e.NameEn).HasMaxLength(150);
                entity.Property(e => e.Price).HasMaxLength(30);
            });

            modelBuilder.Entity<TbFileInfo>(entity =>
            {
                entity.HasKey(e => e.FileId).HasName("PK_TB_FileInfo");
                entity.Property(e => e.FileId).HasMaxLength(15).HasColumnName("FileID");
                entity.Property(e => e.CreateUser).HasMaxLength(10);
                entity.Property(e => e.FileName).HasMaxLength(300);
                entity.Property(e => e.FileRealName).HasMaxLength(30);
                entity.Property(e => e.ModifyUser).HasMaxLength(10);
            });

            modelBuilder.Entity<TbIdSummary>(entity =>
            {
                entity.HasKey(e => e.TableName).HasName("PK_TbIdSummary_1");
                entity.Property(e => e.TableName).HasMaxLength(50);
                entity.Property(e => e.MaxId).HasColumnName("MaxID");
            });

            modelBuilder.Entity<TbLog>(entity =>
            {
                entity.HasKey(e => e.Pid).HasName("PK_TB_LoginRecord");
                entity.Property(e => e.Pid).HasColumnName("PID");
                entity.Property(e => e.Action).HasMaxLength(200);
                entity.Property(e => e.Ip).HasMaxLength(50).HasColumnName("IP");
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
            });

            modelBuilder.Entity<TbLoginRecord>(entity =>
            {
                entity.HasKey(e => e.Pid).HasName("PK_TB_S_LoginRecord");
                entity.Property(e => e.Pid).HasColumnName("PID");
                entity.Property(e => e.Account).HasMaxLength(30);
                entity.Property(e => e.Ip).HasMaxLength(50).HasColumnName("IP");
                entity.Property(e => e.LoginMsg).HasMaxLength(20);
                entity.Property(e => e.MemberId).HasMaxLength(10).HasColumnName("MemberID");
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.Sso).HasColumnName("SSO");
                entity.Property(e => e.Ssoresult).HasColumnName("SSOResult");
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.UserId).HasMaxLength(10).HasColumnName("UserID");
            });

            modelBuilder.Entity<TbApiLog>(entity =>
            {
                entity.HasKey(e => e.Pid).HasName("PK_API_Log");
                entity.Property(e => e.Pid).HasColumnName("PID");
                entity.Property(e => e.Ipaddr).HasMaxLength(45).HasColumnName("IPAddr");
            });

            modelBuilder.Entity<TbBackendOperateLog>(entity =>
            {
                entity.HasKey(e => e.Pid).HasName("PK_TbOperateLog");
                entity.Property(e => e.Pid).HasColumnName("PID");
                entity.Property(e => e.Action).HasMaxLength(20);
                entity.Property(e => e.DataKey).HasMaxLength(30);
                entity.Property(e => e.Feature).HasMaxLength(20);
                entity.Property(e => e.Ip).HasMaxLength(50).HasColumnName("IP");
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.UserId).HasMaxLength(10).HasColumnName("UserID");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.Pid).HasName("PK_TbUser");
                entity.Property(e => e.Pid).HasColumnName("PID");
                entity.Property(e => e.Account).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(100);
                entity.Property(e => e.UserName).HasMaxLength(50);
                entity.HasIndex(e => e.Account).IsUnique();
            });
        }
    }
}
