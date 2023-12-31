﻿using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/favors")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class FavorsController : ApiController
    {
        private readonly IBlogFavoriteService blogFavoriteService;
        private readonly IBlogService blogService;

        public FavorsController(IBlogFavoriteService blogFavoriteService, IBlogService blogService)
        {
            this.blogFavoriteService = blogFavoriteService;
            this.blogService = blogService;
        }

        /// <summary>
        /// 收藏的博客列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("blogs")]
        public async Task<IActionResult> GetBlogFavoriteList([FromQuery] GetBlogFavoriteListRequest req)
        {
            var result = await blogFavoriteService.PagedAsync(UId, req.PageIndex, req.PageSize, req.OffSet, false);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    await blogService.GetBlogRelation(item, UId);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };
                return Ok(Success("获取收藏的博客列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }
    }
}