namespace Resurrection.Domain.Constantes
{
    public static class MorganConstantes
    {
        public static readonly List<string> userAgents = new List<string>
        {
            // Chrome - Windows
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36",

            // Chrome - Linux
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (X11; Ubuntu; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",

            // Chrome - Mac
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",

            // Firefox - Windows
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0",
            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:115.0) Gecko/20100101 Firefox/115.0",

            // Firefox - Linux
            "Mozilla/5.0 (X11; Linux x86_64; rv:121.0) Gecko/20100101 Firefox/121.0",
            "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:120.0) Gecko/20100101 Firefox/120.0",

            // Firefox - Mac
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13.5; rv:121.0) Gecko/20100101 Firefox/121.0",

            // Edge
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0",

            // Safari - Mac
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_5) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Safari/605.1.15",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 12_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Safari/605.1.15",

            // Safari - iPhone
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.0 Mobile/15E148 Safari/604.1",

            // Safari - iPad
            "Mozilla/5.0 (iPad; CPU OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",

            // Android - Chrome
            "Mozilla/5.0 (Linux; Android 14; Pixel 8 Pro) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 13; SM-S918B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 12; SM-A525F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Mobile Safari/537.36",

            // Android - Firefox
            "Mozilla/5.0 (Android 14; Mobile; rv:121.0) Gecko/121.0 Firefox/121.0",
            "Mozilla/5.0 (Android 13; Mobile; rv:120.0) Gecko/120.0 Firefox/120.0",

            // Opera
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 OPR/106.0.0.0",
            "Mozilla/5.0 (Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 OPR/105.0.0.0",

            // Brave
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Brave/1.61.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 13_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Brave/1.60.0",

            // Bots / Crawlers (úteis para simulação)
            "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)",
            "Mozilla/5.0 (compatible; Bingbot/2.0; +http://www.bing.com/bingbot.htm)",
            "Mozilla/5.0 (compatible; DuckDuckBot/1.0; +http://duckduckgo.com/duckduckbot.html)",

            // Legacy / Outros
            "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko",
            "Mozilla/5.0 (X11; Linux i686; rv:109.0) Gecko/20100101 Firefox/109.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36"
        };


        public struct Microsoft
        {
            public const string
            PostLoginMicrosoft = "https://login.live.com/ppsecure/post.srf",
            GetBilling = "https://account.microsoft.com/billing/payments",
            GetPersonalInfo = "https://account.microsoft.com/profile/api/v1/personal-info",
            GetProfile = "https://account.microsoft.com/profile",
            GetAuthorization = "https://account.microsoft.com/auth/acquire-onbehalf-of-token?scopes=pidl",
            GetPaymentInstruments = "https://paymentinstruments.mp.microsoft.com/v6.0/users/me/paymentInstrumentsEx?status=active,removed&language=en-US&partner=northstarweb",
            GetOrders = "https://account.microsoft.com/billing/orders/list",
            GetAuthCompleteSign = "https://account.microsoft.com/auth/complete-client-signin-oauth-silent",
            Connection = "keep-alive",
            Accept = "text/html,application/xhtml+xml",
            AcceptWeb = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
            AcceptJson = "application/json, text/plain, */*",
            AcceptAuthorizationJson = "application/json",
            AcceptLanguage = "en-US,en;q=0.9",
            AcceptEncoding = "gzip, deflate, br, zstd",
            UpgradeInsecureRequests = "1",
            Origin = "https://login.live.com",
            CacheControl = "max-age=0",
            Host = "account.microsoft.com",
            XRequestedWith = "XMLHttpRequest",
            Priority = "u=1, i",
            Xmstest = "undefined",
            Host2 = "login.live.com";

            public struct Headers
            {
                public static readonly Dictionary<string, string> LoginPostSrc = new Dictionary<string, string>
                    {
                        { "Accept", Accept},
                        { "Accept-Language", AcceptLanguage },
                        { "Connection", Connection },
                        { "Upgrade-Insecure-Requests", UpgradeInsecureRequests },
                        //{ "User-Agent", UserAgent }
                    };

                public static readonly Dictionary<string, string> PostLogUrl = new Dictionary<string, string>
                {
                    { "Accept", Accept },
                    { "Accept-Language", AcceptLanguage },
                    { "Cache-Control", CacheControl},
                    { "Origin", Origin },
                    { "Referer", "https://login.live.com/" },
                    { "Upgrade-Insecure-Requests", UpgradeInsecureRequests },
                    //{ "User-Agent", UserAgent}
                };


                public static readonly Dictionary<string, string> PostLogAuth = new Dictionary<string, string>
                {
                    { "Accept", Accept },
                    { "Accept-Language", AcceptLanguage },
                    { "Cache-Control", CacheControl},
                    { "Connection", Connection },
                    { "Origin", Origin },
                    { "Sec-Fetch-Dest", "document" },
                    { "Sec-Fetch-Mode", "navigate" },
                    { "Sec-Fetch-Site", "same-origin" },
                    { "Sec-Fetch-User", "?1" },
                    { "Sec-GPC", "1" },
                    { "Upgrade-Insecure-Requests", UpgradeInsecureRequests },
                    //{ "User-Agent", UserAgent}
                };

                public static readonly Dictionary<string, string> CompleteAuthStage1 = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"Cache-Control", CacheControl},
                    {"Connection", Connection},
                    {"Host", "account.live.com" },
                    { "Referer", Origin},
                    { "upgrade-insecure-requests", UpgradeInsecureRequests},
                    //{ "User-Agent", UserAgent }
                };

                public static readonly Dictionary<string, string> CompleteAuthStage2 = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"Accept-Encoding", AcceptEncoding},
                    {"Accept-Language", AcceptLanguage },
                    {"Cache-Control", CacheControl },
                    {"Connection", Connection},
                    {"host", Host},
                    {"Referer", Origin },
                    {"upgrade-insecure-requests", UpgradeInsecureRequests},
                    //{"user-agent", UserAgent }
                };

                public static readonly Dictionary<string, string> GetDatasAuth = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"accept-encoding", AcceptEncoding},
                    {"accept-language", AcceptLanguage },
                    {"Cache-Control", CacheControl },
                    {"Connection", Connection},
                    {"host", "login.live.com"},
                    {"Referer", Origin },
                    {"upgrade-insecure-requests", UpgradeInsecureRequests},
                    //{"user-agent", UserAgent }
                };

                public static readonly Dictionary<string, string> PostAuthComplete = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"accept-encoding", AcceptEncoding},
                    {"accept-language", AcceptLanguage },
                    {"Cache-Control", CacheControl },
                    {"Connection", Connection},
                    {"host", Host},
                    {"Origin", Origin },
                    {"Referer", $"{Origin}/" },
                    {"upgrade-insecure-requests", UpgradeInsecureRequests },
                    //{"user-agent", UserAgent }
                };

                public static readonly Dictionary<string, string> GetBilling = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"accept-language", AcceptLanguage },
                    {"Cache-Control", CacheControl },
                    {"Connection", Connection},
                    {"Referer", Origin },
                    {"upgrade-insecure-requests", UpgradeInsecureRequests },
                    //{"user-agent", UserAgent }
                };

                public static readonly Dictionary<string, string> GetProfile = new Dictionary<string, string>
                {
                    {"Accept",  AcceptWeb},
                    {"accept-language", AcceptLanguage },
                    {"Cache-Control", CacheControl },
                    {"Connection", Connection},
                    {"upgrade-insecure-requests", UpgradeInsecureRequests },
                    //{"user-agent", UserAgent },
                };

                public static readonly Dictionary<string, string> GetJson = new Dictionary<string, string>
                {
                    {"Accept", AcceptJson },
                    {"Accept-Encoding", AcceptEncoding},
                    {"Accept-Language", AcceptLanguage },
                    {"Connection", Connection},
                    {"host", Host},
                    //{ "User-Agent", UserAgent },
                    { "x-requested-with", XRequestedWith}
                };

                public static readonly Dictionary<string, string> GetJsonBallance = new Dictionary<string, string>
                {
                    {"Accept", "application/json" },
                    {"Accept-Encoding", AcceptEncoding},
                    {"Accept-Language", AcceptLanguage },
                    {"origin",  $"https://{Host}" },
                    {"priority",  Priority },
                    //{ "User-Agent", UserAgent },
                    { "x-ms-test", Xmstest}
                };
            }

            public struct Data
            {
                public static readonly Dictionary<string, string> DataLoginAuth = new Dictionary<string, string>
                {
                    { "LoginOptions", "3" },
                    { "type", "28" },
                    { "ctx", string.Empty },
                    { "hpgrequestid", string.Empty },
                    { "i19", "19130" }
                };
            }

            public struct Params
            {
                public static readonly Dictionary<string, string> ParamsGetBilling = new Dictionary<string, string>
                {
                    { "fref", "home.drawers.payment-options.manage-payment" },
                    { "refd", "account.microsoft.com"}
                };

                public static readonly Dictionary<string, string> ParamsGetOrders = new Dictionary<string, string>
                {
                    {"period", "ThreeMonths" },
                    { "orderTypeFilter", "All"},
                    { "filterChangeCount", "0"},
                    { "isInD365Orders", "true"},
                    { "isPiDetailsRequired", "true"},
                    { "timeZoneOffsetMinutes", "-330"}
                };

            }

        }
    }
}
