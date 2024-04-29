using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace Codecentric
{
    internal class Program
    {
        public static List<FinalMember> _finalMembers = new List<FinalMember>();

        static async Task Main(string[] args)
        {
            await GetMembers();
            SaveFinalMembersToJson("finalMembers.json", _finalMembers);
            Console.ReadKey();
        }

        static async Task GetMembers()
        {
            string url = "https://api.github.com/orgs/codecentric/members";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                client.DefaultRequestHeaders.Add("Authorization", "token " + "Your authorization token");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    
                    List<MemberInformation> members = JsonConvert.DeserializeObject<List<MemberInformation>>(json);

                    foreach (MemberInformation member in members)
                    {
                        List<string> memberLanguages = new List<string>();
                        Repository[] repos = await GetReposForMember(member.login);

                        if (repos != null)
                        {
                            memberLanguages.Clear();
                            foreach (Repository repo in repos)
                            {
                                if (!string.IsNullOrEmpty(repo.language) && !memberLanguages.Contains(repo.language))
                                {
                                    memberLanguages.Add(repo.language);
                                }
                            }
                        }

                        FinalMember finalMember = new FinalMember
                        {
                            Member = member,
                            Repo = repos,
                            Languages = memberLanguages
                        };
                        _finalMembers.Add(finalMember);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to get members. Status code: " + response.StatusCode);
                }
            }
        }

        static async Task<Repository[]> GetReposForMember(string username)
        {
            string url = $"https://api.github.com/users/{username}/repos";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<Repository> repos = JsonConvert.DeserializeObject<List<Repository>>(json);
                    return repos.ToArray();
                }
                else
                {
                    Console.WriteLine($"Failed to get repositories for {username}. Status code: {response.StatusCode}");
                    return null;
                }
            }
        }
        static void SaveFinalMembersToJson(string filePath, List<FinalMember> finalMembers)
        {
            string newJson = JsonConvert.SerializeObject(finalMembers, Formatting.Indented);

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);

                if (newJson != existingJson)
                {
                    File.WriteAllText(filePath, newJson);
                    Console.WriteLine("Json file has been updated.");
                }
                else
                {
                    Console.WriteLine("Content of the Json file has not changed.");
                }
            }
            else
            {
                File.WriteAllText(filePath, newJson);
                Console.WriteLine("Json file was created..");
            }
        }

    }
}
