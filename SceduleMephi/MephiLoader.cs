using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using SceduleLoader.SceduleMephi;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;

namespace SceduleLoader.Core
{
    //site loader
    class MephiLoader : ILoader
    {
        public HttpClient client { get; }
        public string baseUrl { get; }
        public string type { get; }
        public string group { get; set; }

        public MephiLoader(MephiSets set)
        {
            client = new HttpClient();
            baseUrl = set.BaseUrl;
            type = set.Sced;
            group = set.GroupId;//url is built on based url + group id that i don't know how to trace and which type of schedule(1 day/week) we want
            //all this info should be listed in ISets interface(or MephiLoader class they're the same and i don't think that ISets will be used elsewhere
        }

        public async Task<string> GetPage()
        {
            string source = null;
            string url = await GetUrl(group);
            HttpResponseMessage response;
            if (url != null)
                response = await client.GetAsync(url);
            else
                response = null;
            if (response != null && response.StatusCode == HttpStatusCode.OK)//sum code chekin, 404 will never appear due home.mephi's native 404 catchers but i left it cuz i'm lazy
                source = await response.Content.ReadAsStringAsync();
            return source;
        }

        private async Task<string> GetUrl(string info)
        {
            var response = await client.GetAsync(baseUrl + "/study_groups");
            string term = "\\";
            if (info.Length > 0 && info[0] == 'Б')
                term = "Бакалавриат";
            else if (info.Length > 0 && (info[0] == 'С'))
                term = "Специалитет";
            else if (info.Length > 0 && info[0] == 'М')
                term = "Магистратура";
            else if (info.Length > 0 && info.Length > 0 && (info[0] == 'А'))
                term = "Аспирантура";
            else if (info.Length > 0 && info[0] == 'И')
                term = "ПФ";
            string source = null;
            if (response != null && response.StatusCode == HttpStatusCode.OK)//sum code chekin
                source = await response.Content.ReadAsStringAsync();
            HtmlParser domParser = new HtmlParser();
            IHtmlDocument doc = await domParser.ParseDocumentAsync(source);
            var anchors = doc.QuerySelectorAll("a");
            foreach (var a in anchors)
            {
                if (a.TextContent == term + "\n")
                {
                    response = await client.GetAsync(baseUrl + a.GetAttribute("href"));
                    if (response != null && response.StatusCode == HttpStatusCode.OK)//sum code chekin
                        source = await response.Content.ReadAsStringAsync();
                    doc = await domParser.ParseDocumentAsync(source);
                    var anchorsGroup = doc.QuerySelectorAll("a");
                    foreach (var ag in anchorsGroup)
                    {
                        if (ag.TextContent == group + "\n")
                            return baseUrl + "/study_groups/" + ag.GetAttribute("href").Split('/')[2] + "/" + type;
                    }
                }
            }
            return null;
        }
    }
}
