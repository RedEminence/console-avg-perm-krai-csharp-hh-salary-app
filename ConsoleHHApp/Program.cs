using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ConsoleAvgPermKraiCSharpHHSalaryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var baseUrl = "https://api.hh.ru";
            var vacanciesEndpointWithParams = "/vacancies?text=c%23&area=72&only_with_salary=true&per_page=100";
            var fullUrl = $"{baseUrl}{vacanciesEndpointWithParams}";

            var response = client.GetAsync(fullUrl).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;

            var json = JObject.Parse(responseBody);
            var vacancies = (JArray)json["items"];

            var averageCSharpSalary = vacancies.Select(v => GetSalaryForVacancy(v["salary"])).Average();
            
            Console.WriteLine($"Средняя зарплата C#-разработчика в Пермском крае составляет {averageCSharpSalary} рублей");
        }

        private static float GetSalaryForVacancy(JToken vacancySalaryJson)
        {
            if (vacancySalaryJson["from"].Type == JTokenType.Null)
            {
                return (float)vacancySalaryJson["to"];
            }

            if (vacancySalaryJson["to"].Type == JTokenType.Null)
            {
                return (float)vacancySalaryJson["from"];
            }

            return ((float)vacancySalaryJson["to"] + (float)vacancySalaryJson["from"]) / 2;
        }
    }
}
