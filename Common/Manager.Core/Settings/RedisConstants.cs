namespace Manager.Core.Settings
{
    public class RedisConstants
    {
        /// <summary>
        /// 【Key】 JWT_REFRESH_TOKEN
        /// </summary>
        public static readonly string JWT_REFRESH_TOKEN = "jwt_refresh_token:";

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
        public static readonly string PREFIX_USER_GROUP = "usergroup:";


        /// <summary>
        ///【前缀】博客点赞数量
        /// </summary>
        public static readonly string PREFIX_BLOG_LIKE_COUNT = "blog_like_count:";

        /// <summary>
        ///【前缀】博客用户是否点赞
        /// </summary>
        public static readonly string PREFIX_BLOG_ISLIKE = "blog_islike:";

        /// <summary>
        ///【前缀】点赞博客分页
        /// </summary>
        public static readonly string PREFIX_BLOG_LIKE_PAGED = "blog_like_paged:";

        //------------------------------------------------------------------------

        /// <summary>
        ///【前缀】博客收藏数量
        /// </summary>
        public static readonly string PREFIX_BLOG_FAVOR_COUNT = "blog_favor_count:";

        /// <summary>
        ///【前缀】博客用户是否收藏
        /// </summary>
        public static readonly string PREFIX_BLOG_ISFAVOR = "blog_isfavor:";

        /// <summary>
        ///【前缀】收藏博客分页
        /// </summary>
        public static readonly string PREFIX_BLOG_FAVOR_PAGED = "blog_favor_paged:";

        //------------------------------------------------------------------------

        /// <summary>
        ///【前缀】评论点赞数量
        /// </summary>
        public static readonly string PREFIX_COMMENT_LIKE_COUNT = "comment_like_count:";

        /// <summary>
        ///【前缀】当前用户是否点赞
        /// </summary>
        public static readonly string PREFIX_COMMENT_ISLIKE = "comment_islike:";

        //------------------------------------------------------------------------

        /// <summary>
        ///【前缀】转发点赞数量
        /// </summary>
        public static readonly string PREFIX_FORWARD_LIKE_COUNT = "forward_like_Count:";

        /// <summary>
        ///【前缀】转发用户是否点赞
        /// </summary>
        public static readonly string PREFIX_FORWARD_ISLIKE = "forward_islike:";


        //------------------------------------------------------------------------

        /// <summary>
        ///【前缀】图片点赞数量
        /// </summary>
        public static readonly string PREFIX_IMAGE_LIKE_COUNT = "image_like_count:";

        /// <summary>
        ///【前缀】图片用户是否点赞
        /// </summary>
        public static readonly string PREFIX_IMAGE_ISLIKE = "image_islike:";

        //------------------------------------------------------------------------

        /// <summary>
        ///【前缀】博客视频点赞数量
        /// </summary>
        public static readonly string PREFIX_VIDEO_LIKE_COUNT = "video_like_count:";

        /// <summary>
        ///【前缀】博客视频用户是否点赞
        /// </summary>
        public static readonly string PREFIX_VIDEO_ISLIKE = "video_islike:";
    }
}