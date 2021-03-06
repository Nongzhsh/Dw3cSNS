﻿//<TunynetCopyright>
//--------------------------------------------------------------
//<copyright>拓宇网络科技有限公司 2005-2012</copyright>
//<version>V0.5</verion>
//<createdate>2012-10-16</createdate>
//<author>jiangshl</author>
//<email>jiangshl@tunynet.com</email>
//<log date="2012-10-16" version="0.5">创建</log>
//--------------------------------------------------------------
//</TunynetCopyright>

using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode;
using Spacebuilder.Common;
using Tunynet.Common;
using Tunynet.Utilities;
using Tunynet;

namespace Spacebuilder.Blog
{
    /// <summary>
    /// 日志正文解析器
    /// </summary>
    public class BlogBodyProcessor : IBodyProcessor
    {

        public string Process(string body, string tenantTypeId, long associateId, long userId)
        {
            //解析at用户
            AtUserService atUserService = new AtUserService(TenantTypeIds.Instance().BlogThread());
            body = atUserService.ResolveBodyForDetail(body, associateId, userId, AtUserTagGenerate);

            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            IEnumerable<Attachment> attachments = attachmentService.GetsByAssociateId(associateId);
            if (attachments != null && attachments.Count() > 0)
            {
                IList<BBTag> bbTags = new List<BBTag>();
                string htmlTemplate = "<div class=\"tn-annexinlaid\"><a href=\"javascript:;\" target=\"_blank\" menu=\"#attachement-artdialog-{4}\">{0}</a>（<em>{1}</em>{2}，<em>下载次数：{3}</em>）</div>";

                //解析文本中附件
                IEnumerable<Attachment> attachmentsFiles = attachments.Where(n => n.MediaType != MediaType.Image);
                foreach (var attachment in attachmentsFiles)
                {
                    bbTags.Add(AddBBTag(htmlTemplate, attachment));
                }

                body = HtmlUtility.BBCodeToHtml(body, bbTags);
            }

            body = new EmotionService().EmoticonTransforms(body);
            body = DIContainer.Resolve<ParsedMediaService>().ResolveBodyForHtmlDetail(body, ParsedMediaTagGenerate);

            return body;
        }

        #region private method

        /// <summary>
        /// 生成at用户标签
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="displayName">显示名</param>
        private string AtUserTagGenerate(string userName, string displayName)
        {
            return string.Format("<a href=\"{1}\" target=\"_blank\" title=\"{0}\">@{0}</a> ", displayName, SiteUrls.Instance().SpaceHome(userName));
        }

        /// <summary>
        /// 添加BBTag实体
        /// </summary>
        /// <param name="htmlTemplate">html模板</param>
        /// <param name="attachment">带替换附件</param>
        /// <returns>BBTag实体</returns>
        private BBTag AddBBTag(string htmlTemplate, Attachment attachment)
        {
            PointCategory pointCategory = new PointService().GetPointCategory(PointCategoryKeys.Instance().TradePoints());
            string categoryName = pointCategory != null ? pointCategory.CategoryName : "金币";

            BBAttribute bbAttribute = new BBAttribute("attachTemplate", "",
                                                      n =>
                                                      {
                                                          return string.Format(htmlTemplate,
                                                                               attachment.FriendlyFileName,
                                                                               attachment.FriendlyFileLength,
                                                                               attachment.Price > 0 ? "，<em>需要" + attachment.Price + categoryName + "</em>" : "",
                                                                               attachment.DownloadCount,
                                                                               attachment.AttachmentId);
                                                      },
                                                      HtmlEncodingMode.UnsafeDontEncode);

            return new BBTag("attach:" + attachment.AttachmentId, "${attachTemplate}", "", false, BBTagClosingStyle.LeafElementWithoutContent, null, bbAttribute);
        }

        /// <summary>
        /// 生成多媒体内容标签
        /// </summary>
        /// <param name="shrotUrl">短网址</param>
        /// <param name="parsedMedia">多媒体连接实体</param>
        private string ParsedMediaTagGenerate(string shrotUrl, ParsedMedia parsedMedia)
        {
            if (parsedMedia == null)
                return string.Empty;

            if (parsedMedia.MediaType == MediaType.Audio)
            {
                string musicHtml = "<p><a href=\"{0}\" ntype=\"mediaPlay\">{1}<span class=\"tn-icon tn-icon-music tn-icon-inline\"></span></a><br />"
                                   + "<a  href=\"{0}\" ntype=\"mediaPlay\" class=\"tn-button tn-corner-all tn-button-default tn-button-text-icon-primary\">"
                                   + "<span class=\"tn-icon tn-icon-triangle-right\"></span><span class=\"tn-button-text\">音乐播放</span></a></p>";
                return string.Format(musicHtml, SiteUrls.Instance()._MusicDetail(parsedMedia.Alias), shrotUrl);
            }
            else if (parsedMedia.MediaType == MediaType.Video)
            {
                string videoHtml = "<p><a  href=\"{0}\" ntype=\"mediaPlay\">{1}<span class=\"tn-icon tn-icon-movie tn-icon-inline\"></span></a><br />"
                                    + "<a ntype=\"mediaPlay\" href=\"{0}\"><img src=\"{2}\"></a></p>";
                return string.Format(videoHtml, SiteUrls.Instance()._VideoDetail(parsedMedia.Alias), shrotUrl, parsedMedia.ThumbnailUrl);
            }

            return string.Empty;
        }

        #endregion

        #region 新增
        /// <summary>
        /// 手机端日志正文解析
        /// </summary>
        /// <param name="body"></param>
        /// <param name="tenantTypeId"></param>
        /// <param name="associateId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string ProcessForMobile(string body, string tenantTypeId, long associateId, long userId)
        {
            //解析at用户
            AtUserService atUserService = new AtUserService(TenantTypeIds.Instance().BlogThread());
            body = atUserService.ResolveBodyForDetail(body, associateId, userId, AtUserTagGenerate);

            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            IEnumerable<Attachment> attachments = attachmentService.GetsByAssociateId(associateId);
            if (attachments != null && attachments.Count() > 0)
            {
                IList<BBTag> bbTags = new List<BBTag>();
                string htmlTemplate = "<a href=\"{1}\" target=\"_blank\" menu=\"#attachement-artdialog-{2}\">{0}</a>";

                //解析文本中附件
                IEnumerable<Attachment> attachmentsFiles = attachments.Where(n => n.MediaType != MediaType.Image);
                foreach (var attachment in attachmentsFiles)
                {
                    bbTags.Add(AddBBTagForMobile(htmlTemplate, attachment));
                }

                body = HtmlUtility.BBCodeToHtml(body, bbTags);
            }

            body = new EmotionService().EmoticonTransforms(body);
            body = DIContainer.Resolve<ParsedMediaService>().ResolveBodyForHtmlDetail(body, ParsedMediaTagGenerateForMobile);

            return body;
        }
        /// <summary>
        /// 添加BBTag实体
        /// </summary>
        /// <param name="htmlTemplate">html模板</param>
        /// <param name="attachment">带替换附件</param>
        /// <returns></returns>
        private BBTag AddBBTagForMobile(string htmlTemplate, Attachment attachment)
        {

            BBAttribute bbAttribute = new BBAttribute("attachTemplate", "",
                                                      n =>
                                                      {
                                                          return string.Format(htmlTemplate,
                                                                               attachment.FriendlyFileName,
                                                                               SiteUrls.FullUrl(SiteUrls.Instance().AttachmentUrl(attachment.AttachmentId,TenantTypeIds.Instance().BlogThread())),
                                                                               attachment.FriendlyFileLength);
                                                      },
                                                      HtmlEncodingMode.UnsafeDontEncode);

            return new BBTag("attach:" + attachment.AttachmentId, "${attachTemplate}", "", false, BBTagClosingStyle.LeafElementWithoutContent, null, bbAttribute);
        }

        /// <summary>
        /// 生成多媒体内容标签
        /// </summary>
        /// <param name="shrotUrl">短网址</param>
        /// <param name="parsedMedia">多媒体连接实体</param>
        private string ParsedMediaTagGenerateForMobile(string shrotUrl, ParsedMedia parsedMedia)
        {
            if (parsedMedia == null)
                return string.Empty;

            if (parsedMedia.MediaType == MediaType.Audio)
            {
                string musicHtml = "<p><a href=\"{1}\" ntype='mediaPlay'>点击播放音乐</a></p>";
                ShortUrlEntity shortUrlEntity = new ShortUrlService().Get(parsedMedia.Alias);
                return string.Format(musicHtml, SiteUrls.FullUrl(SiteUrls.Instance()._MusicDetail(parsedMedia.Alias)), shortUrlEntity == null ? null : shortUrlEntity.Url);
            }
            else if (parsedMedia.MediaType == MediaType.Video)
            {
                string videoHtml = "<p><a  href=\"{1}\" ntype=\"mediaPlay\">{1}<img src=\"{2}\"></a></p>";
                ShortUrlEntity shortUrlEntity = new ShortUrlService().Get(parsedMedia.Alias);
                return string.Format(videoHtml, SiteUrls.FullUrl(SiteUrls.Instance()._VideoDetail(parsedMedia.Alias)), shortUrlEntity == null ? null : shortUrlEntity.Url, parsedMedia.ThumbnailUrl);
            }

            return string.Empty;
        } 
        #endregion

    }
}