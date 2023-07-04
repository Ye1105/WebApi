namespace Manager.Core.Settings
{
    public class RedisConstants
    {
        /// <summary>
        /// 【存储过程】最受欢迎博客排行
        /// </summary>
        public static readonly string Proc_SelRankTop = "SelRankTop";

        /// <summary>
        /// 【Key】 jwt_refresh_token
        /// </summary>
        public static readonly string JwtRefreshToken = "JwtRefreshToken";

        /// <summary>
        /// 用户信息 | 头像 | 封面
        /// </summary>
        public static readonly string Prefix_AccountInfoAndAvatarAndCover = "AccountInfoAndAvatarAndCover:";

        /// <summary>
        /// 图片列表 【每个图片博客对应的图片列表】
        /// </summary>
        public static readonly string Prefix_ImagesBelongToEachBlog = "Images:";

        /// <summary>
        /// 视频 【每个视频博客对应的视频】
        /// </summary>
        public static readonly string Prefix_VideoBelongToEachBlog = "Videos:";

        /// <summary>
        /// 【热榜】
        /// </summary>
        public static readonly string RankTop = "RankTop";
    }
}