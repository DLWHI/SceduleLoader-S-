using AngleSharp.Html.Dom;
using System.Collections.Generic;

namespace SceduleLoader.Core
{
    interface IParser<T>
    {
        //use in case of expanding
        T Parse(IHtmlDocument doc);
    }
}
