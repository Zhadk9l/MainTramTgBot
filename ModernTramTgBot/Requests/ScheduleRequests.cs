using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class ScheduleRequests
    {

        internal static string GetAllSchedule()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Schedule/GetAllSchedule";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<ScheduleWithRouteAndOperator> viewResponses = JsonConvert.DeserializeObject<List<ScheduleWithRouteAndOperator>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Номер трамваю: #{viewResponse.TramID}\n" +
                                  $"Дні: {viewResponse.Weekdays}\n" +
                                  $"Час старту: {viewResponse.DepartureTime}\n" +
                                  $"Назва маршруту: {viewResponse.RouteName}\n" +
                                  $"Довжина маршруту: {viewResponse.RoutLength}\n" +
                                  $"Ім'я оператора: {viewResponse.OperatorName}\n" +
                                  $"Тг оператора: {viewResponse.OperatorTgName}\n\n\n";
                    }
                }
                else
                {
                    answer = "Нічого не знайдено";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"View Error: {ex.Message}");
                answer = $"Помилка: {ex.Message}";
            }

            return answer;
        }

        internal static string GetlScheduleByOperId(int id)
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Schedule/GetScheduleByOperator/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<ScheduleWithRouteAndOperator> viewResponses = JsonConvert.DeserializeObject<List<ScheduleWithRouteAndOperator>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Номер трамваю: #{viewResponse.TramID}\n" +
                                  $"Дні: {viewResponse.Weekdays}\n" +
                                  $"Час старту: {viewResponse.DepartureTime}\n" +
                                  $"Назва маршруту: {viewResponse.RouteName}\n" +
                                  $"Час маршруту: {viewResponse.Duration}\n" +
                                  $"Довжина маршруту: {viewResponse.RoutLength}\n" +
                                  $"Розклад:\n";
                        foreach (var stop in viewResponse.Stops)
                        {
                            answer += $" - Зупинка: {stop.Name} Час: {stop.Time}\n";
                        }
                    }
                }
                else
                {
                    answer = "Нічого не знайдено";
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при выполнении HTTP-запроса
                Console.WriteLine($"View Error: {ex.Message}");
                answer = $"Помилка: {ex.Message}";
            }

            return answer;
        }

    }
}
