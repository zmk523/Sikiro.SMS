﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sikiro.SMSService.Interfaces;

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

        public bool SendSMS(List<string> phones, string content, string signName, string templateCode = "")
        {
            return SendSMS(string.Join(";", phones), content, signName, templateCode);
        }

        public abstract bool SendSMS(string phone, string content, string signName, string templateCode = "");
    }
}
