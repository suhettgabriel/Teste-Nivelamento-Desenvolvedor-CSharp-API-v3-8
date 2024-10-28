using Newtonsoft.Json.Linq;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals + " goals in " + year);
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;

        totalGoals += await GetGoalsFromApi(team, year, "team1");

        totalGoals += await GetGoalsFromApi(team, year, "team2");

        return totalGoals;
    }

    private static async Task<int> GetGoalsFromApi(string team, int year, string teamPosition)
    {
        int goals = 0;
        int page = 1;
        bool hasMorePages = true;
        HttpClient client = new HttpClient();

        while (hasMorePages)
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamPosition}={team}&page={page}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode) return goals;

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject? jsonObject = JObject.Parse(jsonResponse);

            if (jsonObject == null) return goals;

            int totalPages = (int)(jsonObject["total_pages"] ?? 0);
            foreach (var match in jsonObject["data"] ?? new JArray())
            {
                string? goalString = match[$"{teamPosition}goals"]?.ToString();

                if (int.TryParse(goalString, out int matchGoals))
                {
                    goals += matchGoals;
                }
            }

            page++;
            hasMorePages = page <= totalPages;
        }

        return goals;
    }
}
