using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            //var content = "/ip firewall address-list\n" + response.Content;
            //var domains = content.Replace("*.","");
            //domains = Regex.Replace(domains, @"[а-яА-ЯёЁ]", "");
            //var resultDomains =  domains.Replace("\n", "\nadd list=blocklist address=");

            var domains = response.Content.Replace("*.","").Split("\n").ToList();

            for (int i =0;i< domains.Count;i++)
            {
                domains[i] = Regex.Replace(domains[i], @"[а-яА-ЯёЁ]", "");
                if(domains[i] == "")
                {
                    domains.RemoveAt(i);
                }
                else
                {
                    domains[i] = $"add list=blocklist address={domains[i]}";
                }
            }

            domains = domains.Distinct().ToList();

            string result = string.Join("\n", domains);

            string writePath = @"file.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(result);
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
