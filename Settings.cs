using System;
using System.Xml.Serialization;

namespace RedmineNotification
{
    [XmlRoot("Settings")]
    public class Settings
    {
        /// <summary>
        ///     RedmineのURL
        /// </summary>
        [XmlElement("redmine-url")]
        public String RedmineUrl { get; set; }
        /// <summary>
        ///     RedmineのURL
        /// </summary>
        [XmlElement("redmine-project")]
        public String RedmineProjectName { get; set; }

        /// <summary>
        ///     RedmineのAPIKEY
        /// </summary>
        [XmlElement("redmine-apikey")]
        public String RedmineApiKey { get; set; }
        /// <summary>
        ///     RedmineのAPIKEY
        /// </summary>
        [XmlElement("redmine-query-id")]
        public String RedmineQueryId { get; set; }

        /// <summary>
        ///     Chatwork投稿時のタイトル
        /// </summary>
        [XmlElement("chatwork-title")]
        public String ChatworkTitle { get; set; }

        /// <summary>
        ///     ChatworkのAPIKEY
        /// </summary>
        [XmlElement("chatwork-apikey")]
        public String ChatworkApiKey { get; set; }

        /// <summary>
        ///     ChatworkのルームID
        /// </summary>
        [XmlElement("chatwork-roomid")]
        public String ChatworkApiRoomId { get; set; }

        /// <summary>
        ///     ChatworkのAPIKEY
        /// </summary>
        [XmlElement("hipchat-authtoken")]
        public String HipchatAuthToken { get; set; }
        /// <summary>
        ///     ChatworkのAPIKEY
        /// </summary>
        [XmlElement("hipchat-roomid")]
        public String HipchatRoomId { get; set; }

        private string GetUrl(string url)
        {
            if (url.Length != 0 && url[url.Length - 1] != '/')
            {
                url += '/';
            }
            return url;
        }
        /// <summary>
        ///     プロジェクトのURLを取得
        /// </summary>
        public string GetRedmineProjectUrl()
        {
            return GetUrl(RedmineUrl) + "projects/" + RedmineProjectName;
        }
        /// <summary>
        ///     プロジェクトのクエリーのURLを取得
        /// </summary>
        public string GetRedmineQueryUrl()
        {
            return GetUrl(GetRedmineProjectUrl()) + "issues?query_id=" + RedmineQueryId;
        }
    /// <summary>
        ///     指定IssueのURLを取得
        /// </summary>
        public string GetRedmineIssueUrl(int issueid)
        {
            return GetUrl(RedmineUrl) + "issues/" + issueid;
        }
    }
}