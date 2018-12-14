using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sikiro.SMSService.Interfaces;
using System;

namespace Sikiro.SMSService.Sms
{
    public abstract class BaseSMS : IService
    {
        protected readonly IConfiguration Configuration;
        public int MaxCount { get; set; }

        protected BaseSMS(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected string Account { get; set; }

        protected string Password { get; set; }

        protected string Url { get; set; }

        protected string UserId { get; set; }

        public Tuple<bool,string> SendSMS(List<string> phones, string content, string signName, string templateCode = "", string _params = "", string token = "")
        {
            return SendSMS(string.Join(";", phones), content, signName, templateCode, _params, token);
        }

        public abstract Tuple<bool, string> SendSMS(string phone, string content, string signName, string templateCode = "", string _params = "", string token = "");

    }
}
