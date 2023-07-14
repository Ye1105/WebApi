namespace Manager.Core.Enums
{
    /// <summary>
    /// luence DocumentTypeEnum
    /// </summary>
    public enum DocType
    {
        /// <summary>
        /// 博客
        /// </summary>
        BLOG = 0,

        /// <summary>
        /// 账号
        /// </summary>
        ACCOUNT = 1
    }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public enum ServerType
    {
        [EnumDescription("测试")]
        TEST = 0,

        [EnumDescription("正式")]
        F0RMAL = 1,
    }

    /// <summary>
    /// 腾讯COS
    /// </summary>
    public enum TencentCos
    {
        VIDEO = 0,
        PICTURE = 1
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        [EnumDescription("禁用")]
        DISABLE = 0,

        [EnumDescription("启用")]
        ENABLE = 1,

        [EnumDescription("审核中")]
        UNDER_REVIEW = 2,

        [EnumDescription("审核失败")]
        AUDIT_FAILURE = 3,
    }

    public enum Relation
    {
        [EnumDescription("关注")]
        FOCUS = 0,

        [EnumDescription("特别关注")]
        SPECIAL_FOCUS = 1,
    }

    /// <summary>
    /// 关注关系
    /// </summary>
    public enum FocusRelation
    {
        [EnumDescription("自己")]
        SELF = -2,

        [EnumDescription("关注")]
        FOCUS = 0,

        [EnumDescription("未关注")]
        UNFOCUS = -1,

        [EnumDescription("特别关注")]
        SPECIAL_FOCUS = 1,
    }

    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationType
    {
        [EnumDescription("粉丝")]
        FAN = 1,

        [EnumDescription("关注")]
        FOCUS = 2,
    }

    /// <summary>
    /// 关注渠道
    /// </summary>
    public enum FocusChannel
    {
        [EnumDescription("网页端")]
        WEB = 0,

        [EnumDescription("移动端")]
        MOBILE = 1,
    }

    /// <summary>
    /// 评论类型
    /// </summary>
    public enum CommentType
    {
        [EnumDescription("评论")]
        COMMENT = 0,

        [EnumDescription("一级回复")]
        REPLY_FIRST_LEVEL = 1,

        [EnumDescription("二级回复")]
        REPLY_SECOND_LEVEL = 2
    }

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum Sex
    {
        [EnumDescription("男性")]
        MALE = 0,

        [EnumDescription("女性")]
        FEMALE = 1,

        [EnumDescription("中性")]
        NEUTRAL = 2
    }

    public enum EmotionEnum
    {
        [EnumDescription("未知")]
        UNKNOWN = -1,

        [EnumDescription("单身")]
        SINGLE = 0,

        [EnumDescription("求交往")]
        ASK_FOR_CONTACT = 1,

        [EnumDescription("暗恋中")]
        IN_CRUSH = 2,

        [EnumDescription("暧昧中")]
        AMBIGUOUS = 3,

        [EnumDescription("恋爱中")]
        IN_LOVE = 4,

        [EnumDescription("订婚")]
        GOT_ENGAGED = 5,

        [EnumDescription("已婚")]
        MARRIRED = 6,

        [EnumDescription("分居")]
        SEPARATED = 7,

        [EnumDescription("离异")]
        DIVORCED = 8,

        [EnumDescription("丧偶")]
        WIDOWED = 9,
    }

    /// <summary>
    /// 增删改查
    /// </summary>
    public enum CrudEnum
    {
        [EnumDescription("增加")]
        CREATE = 0,

        [EnumDescription("读取")]
        READ = 1,

        [EnumDescription("更新")]
        UPDATE = 2,

        [EnumDescription("删除")]
        DELETE = 3,

        [EnumDescription("未知")]
        UNKNOWN = 4,
    }

    /// <summary>
    /// 状态是否置顶
    /// </summary>
    public enum BoolType
    {
        [EnumDescription("否")]
        NO = 0,

        [EnumDescription("是")]
        YES = 1
    }

    public enum BlogType
    {
        [EnumDescription("全部")]
        ALL = 0,

        [EnumDescription("文本")]
        TEXT = 1,

        [EnumDescription("头条文章")]
        ARTCILE = 2,

        [EnumDescription("图片")]
        IMAGE = 3,

        [EnumDescription("音乐")]
        MUSIC = 4,

        [EnumDescription("视频")]
        VIDEO = 5
    }

    /// <summary>
    /// 博客分类
    /// </summary>
    public enum BlogSort
    {
        [EnumDescription("公开")]
        PUBLIC = 0,

        [EnumDescription("仅自己可见")]
        PRIVATE = 1,

        [EnumDescription("好友圈")]
        FRIEND = 2,

        [EnumDescription("粉丝")]
        FAN = 3,

        [EnumDescription("热推")]
        HOT_PUSH = 4,

        [EnumDescription("广告")]
        ADVERTISE = 5
    }

    public enum BlogScope
    {
        [EnumDescription("[主页的博客]=>我自己+朋友圈+我是粉丝的博客")]
        HOME = 1,

        [EnumDescription("[朋友圈的博客]=>只查询朋友圈的博客")]
        FRIEND = 2,

        [EnumDescription("[特别关注的博客]=>只查询特别关注的博客")]
        FOCUS = 3,

        [EnumDescription("[自定义分组的博客]=>只查询自定义分组的博客")]
        GROUP = 4,
    }

    public enum ForwardScope
    {
        /// <summary>
        /// blog_forward 中的 BuId 是当前登录网站的用户 id
        /// </summary>
        [EnumDescription("【@我的】动态")]
        AT = 0,

        /// <summary>
        /// 我关注的人转发了我的评论或者博客
        /// </summary>
        [EnumDescription("【@我的】【关注人】的动态")]
        FOCUS = 1,

        /// <summary>
        /// 转发我的原创blog
        /// </summary>
        [EnumDescription("【@我的】【原创】动态")]
        ORIGION = 2
    }

    public enum ApiResultStatus
    {
        #region 信息响应

        /// <summary>
        /// 这个临时响应表明，迄今为止的所有内容都是可行的，客户端应该继续请求，如果已经完成，则忽略它。
        /// </summary>
        CONTINUE = 100,

        /// <summary>
        /// 该代码是响应客户端的 Upgrade (en-US) 请求头发送的，指明服务器即将切换的协议。
        /// </summary>
        SWITCHING_PROTOCOLS = 101,

        /// <summary>
        /// 此代码表示服务器已收到并正在处理该请求，但当前没有响应可用。
        /// </summary>
        PROCESSING = 102,

        /// <summary>
        /// 此状态代码主要用于与 Link 链接头一起使用，以允许用户代理在服务器准备响应阶段时开始预加载 preloading 资源。
        /// </summary>
        EARLY_HINTS = 103,

        #endregion 信息响应

        #region 成功响应

        /// <summary>
        /// 请求成功。成功的含义取决于 HTTP 方法：
        /// <para>GET: 资源已被提取并在消息正文中传输。</para>
        /// <para>HEAD: 实体标头位于消息正文中。</para>
        /// <para>PUT or POST: 描述动作结果的资源在消息体中传输。</para>
        /// <para>TRACE: 消息正文包含服务器收到的请求消息。</para>
        /// </summary>
        OK = 200,

        /// <summary>
        /// 该请求已成功，并因此创建了一个新的资源。这通常是在 POST 请求，或是某些 PUT 请求之后返回的响应。
        /// </summary>
        CREATED = 201,

        /// <summary>
        ///请求已经接收到，但还未响应，没有结果。意味着不会有一个异步的响应去表明当前请求的结果，预期另外的进程和服务去处理请求，或者批处理。
        /// </summary>
        ACCEPTED = 202,

        /// <summary>
        ///服务器已成功处理了请求，但返回的实体头部元信息不是在原始服务器上有效的确定集合，而是来自本地或者第三方的拷贝。当前的信息可能是原始版本的子集或者超集。例如，包含资源的元数据可能导致原始服务器知道元信息的超集。使用此状态码不是必须的，而且只有在响应不使用此状态码便会返回200 OK的情况下才是合适的
        /// </summary>
        NON_AUTHORITATIVE_INFORMATION = 203,

        /// <summary>
        /// 对于该请求没有的内容可发送，但头部字段可能有用。用户代理可能会用此时请求头部信息来更新原来资源的头部缓存字段。
        /// </summary>
        NO_CONTENT = 204,

        /// <summary>
        /// 告诉用户代理重置发送此请求的文档。
        /// </summary>
        RESET_CONTENT = 205,

        /// <summary>
        /// 当从客户端发送Range范围标头以只请求资源的一部分时，将使用此响应代码。
        /// </summary>
        PARTIAL_CONTENT = 206,

        /// <summary>
        /// 对于多个状态代码都可能合适的情况，传输有关多个资源的信息。
        /// </summary>
        MULTI_STATUS = 207,

        /// <summary>
        /// 在 DAV 里面使用 <dav:propstat> 响应元素以避免重复枚举多个绑定的内部成员到同一个集合。
        /// </summary>
        ALREADY_REPORTED = 208,

        /// <summary>
        /// 服务器已经完成了对资源的GET请求，并且响应是对当前实例应用的一个或多个实例操作结果的表示。
        /// </summary>
        IM_USED = 209,

        #endregion 成功响应

        #region 重定向消息

        /// <summary>
        /// 请求拥有多个可能的响应。用户代理或者用户应当从中选择一个。（没有标准化的方法来选择其中一个响应，但是建议使用指向可能性的 HTML 链接，以便用户可以选择。）
        /// </summary>
        MULTIPLE_CHOICE = 300,

        /// <summary>
        /// 请求资源的 URL 已永久更改。在响应中给出了新的 URL。
        /// </summary>
        MOVED_PERMANENTLY = 301,

        /// <summary>
        /// 此响应代码表示所请求资源的 URI 已 暂时 更改。未来可能会对 URI 进行进一步的改变。因此，客户机应该在将来的请求中使用这个相同的 URI。
        /// </summary>
        FOUND = 302,

        /// <summary>
        /// 服务器发送此响应，以指示客户端通过一个 GET 请求在另一个 URI 中获取所请求的资源。
        /// </summary>
        SEE_OTHER = 303,

        /// <summary>
        /// 这是用于缓存的目的。它告诉客户端响应还没有被修改，因此客户端可以继续使用相同的缓存版本的响应。
        /// </summary>
        NOT_MODIFIED = 304,

        /// <summary>
        /// 在 HTTP 规范中定义，以指示请求的响应必须被代理访问。由于对代理的带内配置的安全考虑，它已被弃用。
        /// </summary>
        USE_PROXY = 305,

        /// <summary>
        /// 此响应代码不再使用；它只是保留。它曾在 HTTP/1.1 规范的早期版本中使用过。
        /// </summary>
        UNUSED = 306,

        #endregion 重定向消息

        #region 客户端错误响应

        /// <summary>
        /// 由于被认为是客户端错误（例如，错误的请求语法、无效的请求消息帧或欺骗性的请求路由），服务器无法或不会处理请求。
        /// </summary>
        BAD_REQUEST = 400,

        /// <summary>
        /// 虽然 HTTP 标准指定了"unauthorized"，但从语义上来说，这个响应意味着"unauthenticated"。也就是说，客户端必须对自身进行身份验证才能获得请求的响应。
        /// </summary>
        UNAUTHORIZED = 401,

        /// <summary>
        /// 此响应代码保留供将来使用。创建此代码的最初目的是将其用于数字支付系统，但是此状态代码很少使用，并且不存在标准约定。
        /// </summary>
        PAYMENT_REQUIRED = 402,

        /// <summary>
        /// 客户端没有访问内容的权限；也就是说，它是未经授权的，因此服务器拒绝提供请求的资源。与 401 Unauthorized 不同，服务器知道客户端的身份。
        /// </summary>
        FORBIDDEN = 403,

        /// <summary>
        /// 服务器找不到请求的资源。在浏览器中，这意味着无法识别 URL。在 API 中，这也可能意味着端点有效，但资源本身不存在。服务器也可以发送此响应，而不是 403 Forbidden，以向未经授权的客户端隐藏资源的存在。这个响应代码可能是最广为人知的，因为它经常出现在网络上。
        /// </summary>
        NOT_FOUND = 404,

        /// <summary>
        /// 服务器知道请求方法，但目标资源不支持该方法。例如，API 可能不允许调用DELETE来删除资源。
        /// </summary>
        METHOD_NOT_ALLOWED = 405,

        /// <summary>
        /// 当 web 服务器在执行服务端驱动型内容协商机制后，没有发现任何符合用户代理给定标准的内容时，就会发送此响应。
        /// </summary>
        NOT_ACCEPTABLE = 406,

        /// <summary>
        /// 类似于 401 Unauthorized 但是认证需要由代理完成。
        /// </summary>
        PROXY_AUTHENTICATION_REQUIRED = 407,

        /// <summary>
        /// 此响应由一些服务器在空闲连接上发送，即使客户端之前没有任何请求。这意味着服务器想关闭这个未使用的连接。由于一些浏览器，如 Chrome、Firefox 27+ 或 IE9，使用 HTTP 预连接机制来加速冲浪，所以这种响应被使用得更多。还要注意的是，有些服务器只是关闭了连接而没有发送此消息。
        /// </summary>
        REQUEST_TIMEOUT = 408,

        /// <summary>
        /// 当请求与服务器的当前状态冲突时，将发送此响应。
        /// </summary>
        CONFLICT = 409,

        /// <summary>
        /// 当请求的内容已从服务器中永久删除且没有转发地址时，将发送此响应。客户端需要删除缓存和指向资源的链接。HTTP 规范打算将此状态代码用于“有限时间的促销服务”。API 不应被迫指出已使用此状态代码删除的资源。
        ///</summary>
        GONE = 410,

        /// <summary>
        /// 服务端拒绝该请求因为 Content-Length 头部字段未定义但是服务端需要它。
        ///</summary>
        LENGTH_REQUIRED = 411,

        /// <summary>
        /// 客户端在其头文件中指出了服务器不满足的先决条件。
        ///</summary>
        PRECONDITION_FAILED = 412,

        /// <summary>
        /// 请求实体大于服务器定义的限制。服务器可能会关闭连接，或在标头字段后返回重试 Retry-After。
        ///</summary>
        PAYLOAD_TOO_LARGE = 413,

        /// <summary>
        /// 客户端请求的 URI 比服务器愿意接收的长度长。
        ///</summary>
        URL_TOO_LONG = 414,

        /// <summary>
        /// 服务器不支持请求数据的媒体格式，因此服务器拒绝请求。
        /// </summary>
        UNSUPPORTED_MEDIA_TYPE = 415,

        /// <summary>
        /// 无法满足请求中 Range 标头字段指定的范围。该范围可能超出了目标 URI 数据的大小。
        /// </summary>
        RANGE_NOT_SATISFIABLE = 416,

        /// <summary>
        /// 此响应代码表示服务器无法满足 Expect 请求标头字段所指示的期望。
        /// </summary>
        EXPECTATION_FAILED = 417,

        /// <summary>
        /// 服务端拒绝用茶壶煮咖啡。笑话，典故来源茶壶冲泡咖啡
        /// </summary>
        IM_A_TEAPOT = 418,

        /// <summary>
        /// 请求被定向到无法生成响应的服务器。这可以由未配置为针对请求 URI 中包含的方案和权限组合生成响应的服务器发送。
        /// </summary>
        MISDIRECTED_REQUEST = 421,

        /// <summary>
        /// 请求格式正确，但由于语义错误而无法遵循。
        /// </summary>
        UNPROCESSABLE_ENTITY = 422,

        /// <summary>
        /// 正在访问的资源已锁定。
        /// </summary>
        LOCKED = 423,

        /// <summary>
        /// 由于前一个请求失败，请求失败。
        /// </summary>
        FAILED_DEPENDENCY = 424,

        /// <summary>
        /// 表示服务器不愿意冒险处理可能被重播的请求。
        /// </summary>
        TOO_EARLY = 425,

        /// <summary>
        /// 服务器拒绝使用当前协议执行请求，但在客户端升级到其他协议后可能愿意这样做。 服务端发送带有Upgrade (en-US) 字段的 426 响应 来表明它所需的协议（们）。
        /// </summary>
        UPGRAGE_REQUIRED = 426,

        /// <summary>
        /// 源服务器要求请求是有条件的。此响应旨在防止'丢失更新'问题，即当第三方修改服务器上的状态时，客户端 GET 获取资源的状态，对其进行修改并将其 PUT 放回服务器，从而导致冲突。
        /// </summary>
        PRECONDITION_REQUIRED = 428,

        /// <summary>
        /// 用户在给定的时间内发送了太多请求（"限制请求速率"）
        /// </summary>
        TOO_MANY_REQUESTS = 429,

        /// <summary>
        /// 服务器不愿意处理请求，因为其头字段太大。在减小请求头字段的大小后，可以重新提交请求。
        /// </summary>
        REQUEST_HEADER_FIELDS_TOO_LARGE = 431,

        /// <summary>
        /// 用户代理请求了无法合法提供的资源，例如政府审查的网页。
        /// </summary>
        UNAVAILABLE_FOR_LEGAL_REASONS = 451,

        #endregion 客户端错误响应

        #region 服务端错误响应

        /// <summary>
        /// 服务器遇到了不知道如何处理的情况。
        /// </summary>
        INTERNAL_SERVER_ERROR = 500,

        /// <summary>
        /// 服务器不支持请求方法，因此无法处理。服务器需要支持的唯二方法（因此不能返回此代码）是 GET and HEAD.
        /// </summary>
        NOT_IMPLEMENTED = 501,

        /// <summary>
        /// 此错误响应表明服务器作为网关需要得到一个处理这个请求的响应，但是得到一个错误的响应。
        /// </summary>
        BAD_GATEWAY = 502,

        /// <summary>
        /// 服务器没有准备好处理请求。常见原因是服务器因维护或重载而停机。请注意，与此响应一起，应发送解释问题的用户友好页面。这个响应应该用于临时条件和如果可能的话，HTTP 标头 Retry-After 字段应该包含恢复服务之前的估计时间。网站管理员还必须注意与此响应一起发送的与缓存相关的标头，因为这些临时条件响应通常不应被缓存。
        /// </summary>
        SERVICE_UNAVAILABLE = 503,

        /// <summary>
        /// 当服务器充当网关且无法及时获得响应时，会给出此错误响应。
        /// </summary>
        GATEWAY_TIMEOUT = 504,

        /// <summary>
        /// 服务器不支持请求中使用的 HTTP 版本。，会给出此错误响应。
        /// </summary>
        HTTP_VERSION_NOT_SUPPORTED = 505,

        /// <summary>
        /// 服务器存在内部配置错误：所选的变体资源被配置为参与透明内容协商本身，因此不是协商过程中的适当终点。
        /// </summary>
        VARIANT_ALSO_NEGOTIATES = 506,

        /// <summary>
        /// 无法在资源上执行该方法，因为服务器无法存储成功完成请求所需的表示。
        /// </summary>
        INSUFFICIENT_STORAGE = 507,

        /// <summary>
        /// 服务器在处理请求时检测到无限循环。
        /// </summary>
        LOOP_DETECTED = 508,

        /// <summary>
        /// 服务器需要对请求进行进一步扩展才能完成请求。
        /// </summary>
        NOT_EXTENDED = 510,

        /// <summary>
        /// 指示客户端需要进行身份验证才能获得网络访问权限。
        /// </summary>
        NETWORK_AUTHENTICATION_REQUIRED = 511

        #endregion 服务端错误响应
    }
}