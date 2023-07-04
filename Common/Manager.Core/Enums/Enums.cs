namespace Manager.Core.Enums
{
    /// <summary>
    /// luence DocumentTypeEnum
    /// </summary>
    public enum DocTypeEnum
    {
        blog = 0,
        account = 1
    }

    public enum ServerStatusEnum
    {
        [EnumDescription("测试")]
        Test = 0,

        [EnumDescription("正式")]
        Formal = 1,
    }

    public enum TencentCosEnum
    {
        Video = 0,
        Picture = 1
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        [EnumDescription("启用")]
        Enable = 0,

        [EnumDescription("禁用")]
        Disable = 1,

        [EnumDescription("审核中")]
        UnderReview = 2,

        [EnumDescription("审核失败")]
        AuditFailure = 3,
    }

    /// <summary>
    /// 接口状态
    /// </summary>
    public enum ApiResultStatus
    {
        [EnumDescription("成功")]
        Success = 200,

        [EnumDescription("失败")]
        Fail = 400,

        [EnumDescription("未授权")]
        UnAuthorized = 403,
    }

    /// <summary>
    /// 关注关系
    /// </summary>
    public enum FocusRelationEnum
    {
        [EnumDescription("自己")]
        Self = -2,

        [EnumDescription("未关注")]
        UnFocus = -1,

        [EnumDescription("关注")]
        Focus = 0,

        [EnumDescription("特别关注")]
        SpecialFocus = 1,
    }

    /// <summary>
    /// 关注渠道
    /// </summary>
    public enum FocusChannelEnum
    {
        [EnumDescription("网页端")]
        Web = 0,

        [EnumDescription("移动端")]
        Mobile = 1,
    }

    /// <summary>
    /// 评论类型
    /// </summary>
    public enum CommentTypeEnum
    {
        [EnumDescription("评论")]
        Comment = 0,

        [EnumDescription("一级回复")]
        ReplyFir = 1,

        [EnumDescription("二级回复")]
        ReplySec = 2
    }

    public enum RelationEnum
    {
        Focus = 0,
        SpecialFocus = 1,
    }

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum SexEnum
    {
        [EnumDescription("男性")]
        Male = 0,

        [EnumDescription("女性")]
        Female = 1,

        [EnumDescription("中性")]
        Neutral = 2
    }

    public enum EmotionEnum
    {
        [EnumDescription("未知")]
        Unkown = -1,

        [EnumDescription("单身")]
        Single = 0,

        [EnumDescription("求交往")]
        AskForContact = 1,

        [EnumDescription("暗恋中")]
        InCrush = 2,

        [EnumDescription("暧昧中")]
        Ambiguous = 3,

        [EnumDescription("恋爱中")]
        InLove = 4,

        [EnumDescription("订婚")]
        GotEngaged = 5,

        [EnumDescription("已婚")]
        Married = 6,

        [EnumDescription("分居")]
        Separated = 7,

        [EnumDescription("离异")]
        Divorced = 8,

        [EnumDescription("丧偶")]
        Widowed = 9,
    }

    /// <summary>
    /// 增删改查
    /// </summary>
    public enum CrudEnum
    {
        [EnumDescription("增加")]
        Create = 0,

        [EnumDescription("读取")]
        Read = 1,

        [EnumDescription("更新")]
        Update = 2,

        [EnumDescription("删除")]
        Delete = 3,

        [EnumDescription("未知")]
        Unknow = 4,
    }

    /// <summary>
    /// 状态是否置顶
    /// </summary>
    public enum TopEnum
    {
        [EnumDescription("不置顶")]
        no = 0,

        [EnumDescription("置顶")]
        yes = 1,
    }

    public enum BlogType
    {
        [EnumDescription("全部")]
        All = -1,

        [EnumDescription("图片")]
        Image = 0,

        [EnumDescription("视频")]
        Video = 1,

        [EnumDescription("头条文章")]
        Article = 2,

        [EnumDescription("音乐")]
        Music = 3,

        [EnumDescription("文字")]
        Text = 4
    }

    public enum BlogSort
    {
        [EnumDescription("公开")]
        Public = 0,

        [EnumDescription("仅自己可见")]
        Private = 1,

        [EnumDescription("好友圈")]
        Friend = 2,

        [EnumDescription("粉丝")]
        Fan = 3,

        [EnumDescription("热推")]
        Hotpush = 4,

        [EnumDescription("广告")]
        Advertise = 5
    }

    public enum BlogScope
    {
        [EnumDescription("[主页的博客]=>我自己+朋友圈+我是粉丝的博客")]
        Home = 1,

        [EnumDescription("[朋友圈的博客]=>只查询朋友圈的博客")]
        Friend = 2,

        [EnumDescription("[特别关注的博客]=>只查询特别关注的博客")]
        Focus = 3,

        [EnumDescription("[自定义分组的博客]=>只查询自定义分组的博客")]
        Group = 4,
    }

    public enum ForwardScope
    {
        /// <summary>
        /// blog_forward 中的 BuId 是当前登录网站的用户 id
        /// </summary>
        [EnumDescription("【@我的】动态")]
        At = 0,

        /// <summary>
        /// 我关注的人转发了我的评论或者博客
        /// </summary>
        [EnumDescription("【@我的】【关注人】的动态")]
        Focus = 1,

        /// <summary>
        /// 转发我的原创blog
        /// </summary>
        [EnumDescription("【@我的】【原创】动态")]
        Orgion = 2
    }
}