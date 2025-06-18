using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResApiIntegration.Config
{
    public class ReqResOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int CacheExpirationSeconds { get; set; } = 60; // default 60 seconds
    }
}
