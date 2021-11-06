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
            JArray vacancies = GetHHVacancies();

            var averageCSharpSalary = vacancies.Select(v => GetSalaryForVacancy(v["salary"])).Average();
            
            Console.WriteLine($"Средняя зарплата C#-разработчика в Пермском крае составляет {averageCSharpSalary} рублей");
        }

        private static JArray GetHHVacancies()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            int currentPageNum = 0;
            string initialUrl = $"https://api.hh.ru/vacancies?text=c%23&area=72&only_with_salary=true&per_page=10&page={currentPageNum}";

            var initialResponse = client.GetAsync(initialUrl).Result;
            var initialResponseBody = initialResponse.Content.ReadAsStringAsync().Result;

            JObject json = JObject.Parse(initialResponseBody);
            JArray vacancies = (JArray)json["items"];
            int pageCount = (int)json["pages"];

            currentPageNum++;
            while (currentPageNum < pageCount)
            {
                string urlForPage = $"https://api.hh.ru/vacancies?text=c%23&area=72&only_with_salary=true&per_page=10&page={currentPageNum}";
                var responseForPage = client.GetAsync(urlForPage).Result;
                var responseForPageBody = responseForPage.Content.ReadAsStringAsync().Result;
                JObject jsonForPage = JObject.Parse(responseForPageBody);
                JArray vacanciesForPage = (JArray)jsonForPage["items"];

                foreach (var vacancy in vacanciesForPage)
                {
                    vacancies.Add(vacancy);
                }

                currentPageNum++;
            }

            return vacancies;
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
