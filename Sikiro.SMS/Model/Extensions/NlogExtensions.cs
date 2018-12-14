using System;
using System.Collections.Generic;
using System.Text;

namespace Sikiro.SMS.Api.Model.Extensions
{
    public class NlogExtensions
    {
        public string TraceIdentifier { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int StatusCode { get; set; }
        public string Result { get; set; }
        public string Levels { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string FromBody { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Exception { get; set; }
        public string Time { get; set; }
    }
}
