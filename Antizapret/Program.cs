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


            var domains = response.Content.Replace("*.","").Split("\n").ToList();

            for (int i = 0; i < domains.Count; i++)
            {
                var domain = domains[i].Split(".");
                if (domain.Last() == "ru" || domain.Last() == "com" || domain.Last() == "org" || domain.Last() == "net")
                {
                    domains[i] = $"add list=blocklist address={domains[i]}";
                }
                else
                {
                    domains[i]="";
                }
            }

            domains.RemoveAll(e=>e=="");

            domains = domains.Distinct().ToList();

            string result = string.Join("\n", domains);

            string writePath = @"file.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine("/ip firewall address-list\n" + result);
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
