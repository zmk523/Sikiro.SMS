using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Net.SDK.Core;
using Aliyun.Net.SDK.Core.Exceptions;
using Aliyun.Net.SDK.Core.Http;
using Aliyun.Net.SDK.Core.Profile;
using Microsoft.Extensions.Configuration;
using Sikiro.SMSService.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    /// 阿里大鱼
    /// </summary>
    public class AliyunSMS : BaseSMS
    {
        private string EndpointName;
        private string RegionId;
        private string Product;
        private string Domain;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public AliyunSMS(IConfiguration configuration) : base(configuration)
        {
            var config = configuration.Get<SmsConfig>();
            EndpointName = config.Sms.AliyunSMS.EndpointName;
            RegionId = config.Sms.AliyunSMS.RegionId;
            Product = config.Sms.AliyunSMS.Product;
            Domain = config.Sms.AliyunSMS.Domain;
            Account = config.Sms.AliyunSMS.AccessKeyId;
            Password = config.Sms.AliyunSMS.AccessKeySecret;
            MaxCount = config.Sms.AliyunSMS.MaxCount;
        }

        public override bool SendSMS(string phone, string content, string signName, string templateCode = "", string _params = "")
        {
            phone = phone.Replace(";", ",");
            IClientProfile profile = DefaultProfile.GetProfile(RegionId, Account, Password);
            //初始化ascClient,暂时不支持多region（请勿修改）
            DefaultProfile.AddEndpoint(EndpointName, RegionId, Product, Domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            string msg = "";
            int repCode = 0;
            try
            {
                SendSmsResponse response;
                switch (templateCode)
                {
                    case "SMS_116785169"://身份验证验证码
                    case "SMS_116785167"://登录确认验证码
                    case "SMS_116785166"://登录异常验证码
                    case "SMS_116785165"://用户注册验证码
                    case "SMS_116785164"://修改密码验证码
                    case "SMS_116785163"://信息变更验证码
                        response = SendSMS(acsClient, request, phone, content, signName, templateCode);
                        break;
                    case "SMS_116785168"://短信测试
                        response = SendSMS2(acsClient, request, phone, content, signName, templateCode);
                        break;
                    default:
                        return false;
                }
                if (response.Code.ToUpper() == "OK")
                {
                    repCode = 1;
                }
                else
                {
                    repCode = 0;
                }
                msg = response.Message;
            }
            catch (ServerException e)
            {
                msg = e.Message;
            }
            catch (ClientException e)
            {
                msg = e.Message;
            }
            Console.WriteLine($"AliyunSMS：{msg}  {DateTime.Now}{Environment.NewLine}");
            return repCode > 0 ? true : false;
        }

        private SendSmsResponse SendSMS(IAcsClient acsClient, SendSmsRequest request, string phone, string content, string signName, string templateCode)
        {
            //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
            request.PhoneNumbers = phone;
            //必填:短信签名-可在短信控制台中找到
            request.SignName = signName;
            //必填:短信模板-可在短信控制台中找到
            request.TemplateCode = templateCode;
            //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
            request.TemplateParam = $"{{\"code\":\"{content}\"}}";
            //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
            request.OutId = "yourOutId";
            //请求失败这里会抛ClientException异常
            return acsClient.GetAcsResponse(request);

        }

        private SendSmsResponse SendSMS2(IAcsClient acsClient, SendSmsRequest request, string phone, string content, string signName, string templateCode)
        {
            //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
            request.PhoneNumbers = phone;
            //必填:短信签名-可在短信控制台中找到
            request.SignName = signName;
            //必填:短信模板-可在短信控制台中找到
            request.TemplateCode = templateCode;
            //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
            request.TemplateParam = $"{{\"customer\":\"{content}\"}}";
            //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
            request.OutId = "yourOutId";
            //请求失败这里会抛ClientException异常
            return acsClient.GetAcsResponse(request);

        }
    }
}
