using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Codecentric
{
    internal class Program
    {
        private static readonly HttpClient _client = new HttpClient();
        public static List<FinalMember> finalMembers = new List<FinalMember>();
        static async Task Main(string[] args)
        {
            //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
            Console.WriteLine("In Main");
            await GetMembers();
            //await GetReposForMember("Test");

            Console.WriteLine("Endee");
            Console.WriteLine(finalMembers);
            /*foreach (var members in finalMembers)
            {
                Console.WriteLine(members);
            }*/
            
            Console.WriteLine("Drücken Sie eine beliebige Taste, um das Programm zu beenden...");
            Console.ReadKey();
        }

        static async Task GetMembers()
        {
            Console.WriteLine("In GetMembers");
            string url = "https://api.github.com/orgs/codecentric/members";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "token " + "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");
                client.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    
                     List<MemberInformation> members = JsonConvert.DeserializeObject<List<MemberInformation>>(json);
                    foreach (MemberInformation member in members)
                    {
                        //await Repo[] GetReposForMember(member.login);
                        Repo[] repos = await GetReposForMember(member.login);
                        //await GetLanguages(member.login);

                        FinalMember finalMember = new FinalMember
                        {
                            member = member,
                            repo = repos,
                            languages = new string[] { "Java", "C#" }
                            //languages = await GetLanguages()
                        };
                        //Console.WriteLine("----------");
                        //Console.WriteLine(finalMember);
                        finalMembers.Add(finalMember);
                    }
                    //Console.WriteLine(json);
                }
                else
                {
                    Console.WriteLine("Failed to get members. Status code: " + response.StatusCode);
                }
            }
        }

        static async Task<Repo[]> GetReposForMember(string username)
        {
            //username = "danielbayerlein";
             Console.WriteLine("In GetReposForMember");
            string url = $"https://api.github.com/users/{username}/repos";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine($"Repos for {username}:");
                    Console.WriteLine(json);
                    List<Repo> repos = JsonConvert.DeserializeObject<List<Repo>>(json);
                    return repos.ToArray();
                }
                else
                {
                    Console.WriteLine($"Failed to fetch repositories for {username}. Status code: {response.StatusCode}");
                    return null;
                }
            }


            
        }
        /*static async Task GetLanguages(string username, string name)
        {
            string url = $"https://api.github.com/repos/{username}/{name}";
        }*/

    }
}
