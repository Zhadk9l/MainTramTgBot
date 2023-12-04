using Newtonsoft.Json;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class AdminRequests
    {
        internal static string IsAdmin(int id)
        {
            string url = "https://localhost:7195/api/Admin/IsAdmin/" + id;
            string answer = null;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                string viewResponses = JsonConvert.DeserializeObject<string>(response);

                answer = viewResponses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"View Error: {ex.Message}");
                answer = $"Помилка: {ex.Message}";
            }

            return answer;
        }
    }
}
