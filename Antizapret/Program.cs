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

            var tmpList = new List<string[]>();

            for (int i = 0; i < domains.Count; i++)
            {

                foreach (var s in domains[i])
                {
                    if (Regex.IsMatch(s, @"\p{IsCyrillic}"))
                    {
                        domains[i] = null;
                        break;
                    }
                }

                var array = domains[i];

                var str = new string[2];
                if (array == null || array.Length <= 1)
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
                    tmpList.Add(str);
                }
            }

            var resultList = new List<string[]>();

            var c = tmpList.Distinct().ToList();

            var duplicates = tmpList
             .GroupBy(r => new { S1 = r[0], S2 = r[1] })
             .Where(g => g.Count() > 1)
             .ToList();

            var notDuplicates = tmpList
             .GroupBy(r => new { S1 = r[0], S2 = r[1] })
             .Where(g => g.Count() == 1)
             .ToList();

            resultList = notDuplicates.Concat(duplicates).ToList().Select(e => new string[] { e.Key.S1, e.Key.S2 }).ToList();

            var res = resultList.Select(e => $"add list=blocklist address={e[0]}.{e[1]}");

            string result = string.Join("\n", res);

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
