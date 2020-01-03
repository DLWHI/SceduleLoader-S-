using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using SceduleLoader.Core;
using System.Threading.Tasks;

namespace SceduleLoader.SceduleMephi
{
    class Worker<T> where T : class
    {
        private IParser<T> parser;//our parser
        private ISets sets;//url sets

        public IParser<T> Parser
        {
            get
            {
                return parser;
            }
            set
            {
                parser = value;
            }
        }
        public ISets Sets
        {
            get
            {
                return sets;
            }
            set
            {
                sets = value;
            }
        }
        public string ErrorMes { get; set; }

        public Worker(IParser<T> parser)
        {
            Parser = parser;
        }
        public Worker(IParser<T> parser, ISets sets): this(parser)
        {
            Sets = sets;
        }

        public async Task<T> Work(ILoader loader)
        {
            string source = await loader.GetPage();//gettin page
            TreatErrors(source);//error check
            HtmlParser domParser = new HtmlParser();//anglesharp domparser
            IHtmlDocument doc = await domParser.ParseDocumentAsync(source);//parsin url so anglesharp can read it
            var result  = Parser.Parse(doc);//parsin url so WE can read it
            return result;
        }

        private void TreatErrors(string source)//if we encounter error, make error mes look like something
        {
            if (source == null || source == "No group or type selected")
                ErrorMes = "Нет такой группы";
            else if (source == "400")
                ErrorMes = "Кривой запрос, но это уже другая история.(400 Bad Request)";
            else if (source == "500")
                ErrorMes = "Home.Mephi лежит и ничего не делает лучше чем вы все вместе взятые(500 Internal Server Error)";
            else if (source == "503")
                ErrorMes = "Home.Mephi закрыт на технические работы. Пожалуйста подождите(ЧЕТЫРЕЖДЫБЛЯДСКАЯ ЯРОСТЬ)(500 Internal Server Error)";
            else if (source == "THEN MAKE IT GOOD(502 Bad Gateway)")
                ErrorMes = "Я все вижу, вырубай впн(502 Bad Gateway)";
            else if (source.Length == 3)
                ErrorMes = "Вот теперь хуй пойми что происходит, скажи мне " + source;
            else
                ErrorMes = "Ошибок нет збс";
        }
    }
}
