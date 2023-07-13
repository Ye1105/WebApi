namespace Manager.Core.Settings
{
    public class RedisConstants
    {
        /// <summary>
        /// 【Key】 JWT_REFRESH_TOKEN
        /// </summary>
        public static readonly string JWT_REFRESH_TOKEN = "jwt_refresh_token";

        /// <summary>
        /// 用户信息 | 头像 | 封面
        /// </summary>
        public static readonly string PREFIX_ACCOUNT_INFO = "account_info:";

        /// <summary>
        /// 图片列表 【每个图片博客对应的图片列表】
        /// </summary>
        public static readonly string PREFIE_IMAGE = "blog_images:";

        /// <summary>
        /// 视频 【每个视频博客对应的视频】
        /// </summary>
        public static readonly string PREFIX_VIDEO = "blog_videos:";

        /// <summary>
        /// 头像分页列表
        /// </summary>
        public static readonly string PREFIX_AVATAR_PAGED = "avatar_paged:";

        /// <summary>
        /// 封面分页列表
        /// </summary>
        public static readonly string PREFIX_COVER_PAGED = "cover_paged:";

        /// <summary>
        /// 用户分组
        /// </summary>
        public static readonly string PREFIX_USER_GROUP= "usergroup:";
    }
}