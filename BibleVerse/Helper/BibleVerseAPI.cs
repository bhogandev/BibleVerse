using System;
using System.Net.Http;
namespace BibleVerse.Helper
{
    public class BibleVerseAPI
    {
        public HttpClient Initial()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44307/api/");
            return client;
        }
    }
}
