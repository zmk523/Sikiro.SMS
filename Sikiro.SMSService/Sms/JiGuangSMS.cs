using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Jiguang.JPush;
using Jiguang.JPush.Model;
using Sikiro.SMSService.Model;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    /// 极光推送
    /// </summary>
    public class JiGuangSMS : BaseSMS
    {
        private static string AppKey = string.Empty;
        private static string AppSecret = string.Empty;
        private JPushClient client;
        //private Dictionary<string, string> headers = new Dictionary<string, string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public JiGuangSMS(IConfiguration configuration) : base(configuration)
        {
            var config = configuration.Get<SmsConfig>();
            AppKey = config.Sms.JiGuangSMS.AppKey;
            AppSecret = config.Sms.JiGuangSMS.AppSecret;
            Url = config.Sms.JiGuangSMS.Url;
            MaxCount = config.Sms.JiGuangSMS.MaxCount;

            client = new JPushClient(AppKey, AppSecret);
            //
            //headers.Add("Authorization", $"Basic {EncryptHelper.Base64Code(string.Concat(Account, ":", Password))}");
        }

        public override Tuple<bool, string> SendSMS(string phone, string content, string signName, string templateCode = "", string _params = "", string token = "")
        {
            try
            {
                //string jparams = JsonConvert.SerializeObject(_params);
                var cidHttpResponse = client.GetCIdList(1, "push");
                var cidModel = JsonConvert.DeserializeObject<JiguangCIdList>(cidHttpResponse.Content);
                _params = _params.Replace("8888888888", cidModel.cidlist[0]);
                //var pushModel = JsonConvert.DeserializeObject<PushPayload>(_params);
                var httpResponse = client.SendPushAsync(_params).Result;
                if (!httpResponse.Content.Contains("sendno"))
                {
                    Console.WriteLine($"JiGuangSMS：{httpResponse.Content}  {DateTime.Now}{Environment.NewLine}");
                    return new Tuple<bool, string>(false, httpResponse.Content);
                }
                var result = JsonToDictionary(httpResponse.Content);
                //{"sendno":"0","msg_id":"54043197107897451"}              
                Console.WriteLine($"JiGuangSMS：{httpResponse.Content}  {DateTime.Now}{Environment.NewLine}");
                return new Tuple<bool, string>(Convert.ToInt32(result["sendno"].ToString()) == 0 ? true : false, httpResponse.Content);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }

        public Dictionary<string, object> JsonToDictionary(string jsonString)
        {
            Dictionary<string, object> ht = new Dictionary<string, object>();
            object json = JsonConvert.DeserializeObject(jsonString);
            //返回的结果一定是一个json对象
            Newtonsoft.Json.Linq.JObject jsonObject = json as Newtonsoft.Json.Linq.JObject;
            if (jsonObject == null)
            {
                return ht;
            }
            foreach (Newtonsoft.Json.Linq.JProperty jProperty in jsonObject.Properties())
            {
                Newtonsoft.Json.Linq.JToken jToken = jProperty.Value;
                string value = "";
                if (jToken != null)
                {
                    value = jToken.ToString();
                }
                ht.Add(jProperty.Name, value);
            }
            return ht;
        }
    }
}

