using Manager.Core.Models.Accounts;
using Manager.Core.Models.Blogs;
using Manager.Core.Models.Logs;
using Manager.Core.Models.Modules;
using Manager.Core.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Manager.Infrastructure.Database
{
    public partial class H5Context : DbContext
    {
        //https://blog.51cto.com/u_15316096/5328601 参照写法
        public H5Context()
        {
        }

        public H5Context(DbContextOptions<H5Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region

            #region 后端

            modelBuilder.Entity<AccountInfo>()
            .HasOne(accountInfo => accountInfo.Cover)
            .WithOne(avatar => avatar.AccountInfo)
            .HasForeignKey<AccountInfo>(accountInfo => accountInfo.CoverId);

            modelBuilder.Entity<AccountInfo>()
            .HasOne(accountInfo => accountInfo.Avatar)
            .WithOne(avatar => avatar.AccountInfo)
            .HasForeignKey<AccountInfo>(accountInfo => accountInfo.AvatarId);

            #endregion 后端

            #endregion

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        #region Api

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountInfo> Account_Info { get; set; }
        public virtual DbSet<AccountRole> Account_Role { get; set; }
        public virtual DbSet<RoleInfo> Role_Info { get; set; }
        public virtual DbSet<RolePermission> Role_Permission { get; set; }

        public virtual DbSet<LogMailSMS> Log_Mail_SMS { get; set; }
        public virtual DbSet<LogTencentSMS> Log_Tencent_SMS { get; set; }
        public virtual DbSet<LogAvatar> Log_Avatar { get; set; }
        public virtual DbSet<LogCover> Log_Cover { get; set; }

        public virtual DbSet<UserGroup> User_Group { get; set; }
        public virtual DbSet<UserFocus> User_Focus { get; set; }
        public virtual DbSet<UserTopic> User_Topic { get; set; }

        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<BlogLike> Blog_Like { get; set; }
        public virtual DbSet<BlogFavorite> Blog_Favorite { get; set; }

        public virtual DbSet<BlogVideo> Blog_Video { get; set; }
        public virtual DbSet<BlogVideoLike> Blog_Video_Like { get; set; }

        public virtual DbSet<BlogImage> Blog_Image { get; set; }
        public virtual DbSet<BlogImageLike> Blog_Image_Like { get; set; }

        public virtual DbSet<BlogComment> Blog_Comment { get; set; }
        public virtual DbSet<BlogCommentLike> Blog_Comment_Like { get; set; }

        public virtual DbSet<BlogForward> Blog_Forward { get; set; }
        public virtual DbSet<BlogForwardLike> Blog_Forward_Like { get; set; }

        public virtual DbSet<BlogTopic> Blog_Topic { get; set; }

        public virtual DbSet<ModuleInfo> Module_Info { get; set; }

        #endregion
    }
}