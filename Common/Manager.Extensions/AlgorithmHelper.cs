namespace Manager.Extensions
{
    public class AlgorithmHelper
    {
        /// https://www.jianshu.com/p/88a2999c807d
        /// <summary>
        /// 获取博客热门排行的分数
        /// w:博客点赞数、评论数、转发数、收藏数
        /// i:博客作者的影响因子
        /// t:发布以来的时长
        /// g:衰减的重力参数
        /// </summary>
        /// <param name="created">时间</param>
        /// <param name="like">点赞数</param>
        /// <param name="comment">评论数</param>
        /// <param name="forward">转发数</param>
        /// <param name="favorite">收藏数</param>
        /// <returns></returns>
        public static decimal GetTopScore(DateTime created, long like = 0, long comment = 0, long forward = 0, long favorite = 0)
        {
            var i = (decimal)10;
            var w = Convert.ToDecimal(like * 1 + comment * 2 + forward * 4 + favorite * 3) / 10;
            var t = DateHelper.ConvertDateTimeToLong(created);
            return (w + i) / Convert.ToDecimal((Math.Pow((t + 1), 1.5)));
        }
    }
}