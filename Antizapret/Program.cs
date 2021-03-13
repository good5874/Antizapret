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

            var domains = response.Content.Replace("*.", "").Split("\n").ToList();

            for (int i = 0; i < domains.Count; i++)
            {
                if(Regex.IsMatch(domains[i], @"\p{IsCyrillic}"))
                {
                    var c = domains[i];
                    domains[i] = "";
                    continue;
                }
                var domain = domains[i].Split(".");
                var str = new string [2];
                if(domain.Length <= 1)
                {
                    continue;
                }
                if (domain.Count() > 2)
                {
                    str[0] = domain[domain.Length - 2];
                    str[1] = domain[domain.Length - 1];
                }
                else
                {
                    str[0] = domain[0];
                    str[1] = domain[1];
                }

                if (str[1] == "ru" || str[1] == "com" || str[1] == "org" || str[1] == "net")
                {
                    domains[i] = $"add list=blocklist address={str[0]}.{str[1]}";
                }
                else
                {
                    domains[i] = "";
                }
            }

            domains.RemoveAll(e => e == "");

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
