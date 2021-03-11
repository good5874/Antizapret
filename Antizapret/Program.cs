using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Antizapret
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://api.antizapret.info");
            var request = new RestRequest("group.php");
            request.AddParameter("data", "domain");
            request.AddParameter("type", "json");

            var response = client.Get(request);
            var content = response.Content;
            var domains = content.Replace("*.","").Replace("\n", "\nadd list=blocklist address=");
            domains = Regex.Replace(domains, @"[а-яА-ЯёЁ]", "");
            var resultDomains = domains.Replace("\n", "\nadd list=blocklist address=");

            string writePath = @"file.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(resultDomains);
                }
                Console.WriteLine("Запись выполнена");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
