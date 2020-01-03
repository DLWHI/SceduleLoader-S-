using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SceduleLoader.Core
{
    interface ILoader
    {
        string baseUrl { get; }
        Task<string> GetPage();
    }
}
