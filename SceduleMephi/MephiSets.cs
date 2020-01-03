using SceduleLoader.Core;

namespace SceduleLoader.SceduleMephi
{
    class MephiSets: ISets
    {
        public string BaseUrl { get; set; } = "https://home.mephi.ru";
        public string GroupId { get; set; } = "{Id}";
        public string Sced { get; set; } = "{Sced}";//scedule for exams/general/1 day and etc;
    }
}
