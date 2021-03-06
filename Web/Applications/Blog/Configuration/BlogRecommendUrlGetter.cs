﻿//<TunynetCopyright>
//--------------------------------------------------------------
//<version>V0.5</verion>
//<createdate>2012-11-23</createdate>
//<author>zhaok</author>
//<email>zhaok@tunynet.com</email>
//<log date="2012-11-23" version="0.5">创建</log>
//--------------------------------------------------------------
//</TunynetCopyright>

using Tunynet.Common;
using Spacebuilder.Common;

namespace Spacebuilder.Blog
{
    /// <summary>
    /// 文章推荐Url获取器
    /// </summary>
    public class BlogRecommendUrlGetter : IRecommendUrlGetter
    {
        /// <summary>
        /// 租户类型Id
        /// </summary>
        public string TenantTypeId
        {
            get { return TenantTypeIds.Instance().BlogThread(); }
        }

        /// <summary>
        /// 详细页面地址
        /// </summary>
        /// <param name="itemId">推荐内容Id</param>
        /// <returns></returns>
        public string RecommendItemDetail(long itemId)
        {
            BlogThread blogThread = new BlogService().Get(itemId);
            if (blogThread == null)
                return string.Empty;
            string userName = UserIdToUserNameDictionary.GetUserName(blogThread.UserId);
            return SiteUrls.Instance().BlogDetail(userName, itemId);
        }
    }
}