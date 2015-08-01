using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HipchatApiV2;
using HipchatApiV2.Enums;
using Newtonsoft.Json;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using CSChatworkAPI;

namespace RedmineNotification
{
    internal class Program
    {
        static Settings _settings;
        public static string GetMessage(IList<Issue> list, string retCode, string title)
        {
            var memdic = new Dictionary<string, List<Issue>>();
            foreach (var l in list)
            {
                var name = "未定";
                if (l.AssignedTo != null)
                {
                    name = l.AssignedTo.Name;
                }
                if (memdic.ContainsKey(name) == false)
                {
                    memdic.Add(name, new List<Issue>());
                }
                memdic[name].Add(l);
            }
            var message = string.Empty;
            if (list.Count != 0)
            {
                message = title + " : " + list.Count + "枚 " + retCode + retCode;
                foreach (var l in memdic)
                {
                    message += l.Key + retCode;
                    message = l.Value.Aggregate(message, (current, r) => current + ("    #" + r.Id + " 『" + r.Subject +"』" + "(" + r.Status.Name +")" + " " + _settings.GetRedmineIssueUrl(r.Id) + retCode));
                    message += retCode;
                }
            }
            return message + retCode;
        }

        public static IList<Issue> GetLostList(IList<Issue> oldlist, IList<Issue> newlist)
        {
            return (from n in newlist let state = oldlist.Any(o => n.Id == o.Id) where state == false select n).ToList();
        }

        public static IList<Issue> GetChangeSets(IList<Issue> oldlist, IList<Issue> newlist)
        {
            return oldlist.Where(o => newlist.Any(n => o.Id == n.Id && o.UpdatedOn == n.UpdatedOn) == false).ToList();
        }

        /// <summary>
        ///     メイン処理
        /// </summary>
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("XMLファイルが指定されていません。");
                return 1;
            }
            IList<Issue> cacheList = new List<Issue>();
            IList<Issue> resultList = new List<Issue>();
            IList<Issue> changetList = new List<Issue>();
            const string cacheFileName = "cache.json";
            var serializer = new XmlSerializer(typeof(Settings));
            try
            {
                using (var sr = new StreamReader(args[0]))
                {
                    _settings = (Settings)serializer.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
            try
            {
                using (var sr = new StreamReader(cacheFileName))
                {
                    var json = sr.ReadToEnd();
                    cacheList = JsonConvert.DeserializeObject<IList<Issue>>(json);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("キャッシュファイルが読み込めませんでした。");
            }
            var manager = new RedmineManager(_settings.GetRedmineProjectUrl(), _settings.RedmineApiKey);
            var parameters = new NameValueCollection { { "query_id", _settings.RedmineQueryId } };
            try
            {
                changetList = GetChangeSets(manager.GetObjectList<Issue>(parameters), cacheList);
                foreach (var s in GetLostList(manager.GetObjectList<Issue>(parameters), cacheList))
                {
                    s.Status.Name = "完了";
                    changetList.Add(s);
                }
                resultList = GetChangeSets(manager.GetObjectList<Issue>(parameters), changetList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
            try
            {
                var json = JsonConvert.SerializeObject(manager.GetObjectList<Issue>(parameters));
                using (var sw = new StreamWriter(cacheFileName))
                {
                    sw.WriteLine(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
            if (changetList.Count > 0)
            {
                if (string.IsNullOrEmpty(_settings.ChatworkApiKey) == false)
                {
                    var client = new ChatworkClient(_settings.ChatworkApiKey);
                    var roomid = Convert.ToInt32(_settings.ChatworkApiRoomId);
                    var meessage = "[info][title]" + _settings.ChatworkTitle + " 合計:" +
                                   (changetList.Count + resultList.Count) + "枚 " + _settings.GetRedmineQueryUrl() +
                                   "[/title]" +
                                   GetMessage(changetList, "\n", "変更チケット") +
                                   GetMessage(resultList, "\n", "残りチケット") + "[/info]";
                    var sentMessage = client.SendMessage(roomid, meessage);
                    Console.WriteLine(sentMessage);
                }
                if (string.IsNullOrEmpty(_settings.HipchatAuthToken) == false)
                {
                    var client = new HipchatClient(_settings.HipchatAuthToken);
                    var request = new HipchatApiV2.Requests.SendRoomNotificationRequest()
                    {
                        Message = " 合計:" + (changetList.Count + resultList.Count) + "枚 " + _settings.GetRedmineQueryUrl() + "<BR>" + GetMessage(changetList, "<BR>", "変更チケット") + GetMessage(resultList, "<BR>", "残りチケット"),
                        MessageFormat = HipchatMessageFormat.Html,
                        Notify = true,
                        Color = RoomColors.Purple
                    };
                    client.SendNotification(Convert.ToInt32(_settings.HipchatRoomId), request);
                }
            }
            else
            {
                Console.WriteLine("キャッシュと最新版との変化がありません");
            }
            return 0;
        }
    }
}
