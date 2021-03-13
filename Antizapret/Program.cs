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

            var domains = response.Content.Replace("*.", "").Split("\n").Select(e => e.Split(".")).ToList();
            var resultList = new List<string>();

            for (int i = 0; i < domains.Count; i++)
            {
                var array = domains[i];

                foreach (var s in domains[i])
                {
                    if (Regex.IsMatch(s, @"\p{IsCyrillic}"))
                    {
                        domains[i] = null;
                        continue;
                    }
                }

                var str = new string[2];
                if (array.Length <= 1)
                {
                    continue;
                }
                if (array.Count() > 2)
                {
                    str[0] = array[array.Length - 2];
                    str[1] = array[array.Length - 1];
                }
                else
                {
                    str[0] = array[0];
                    str[1] = array[1];
                }

                if (str[1] == "ru" || str[1] == "com" || str[1] == "org" || str[1] == "net")
                {
                    resultList.Add($"add list=blocklist address={str[0]}.{str[1]}");
                }
            }

            resultList = resultList.Distinct().ToList();

            string result = string.Join("\n", resultList);

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
