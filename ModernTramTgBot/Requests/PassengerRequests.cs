using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class PassengerRequests
    {
        internal static void PostPassenger(long id, string name, string tgName)
        {
            Passenger passenger = new Passenger();
            passenger.TgID = Int32.Parse(id.ToString());
            passenger.Name = name;
            passenger.TgName = "@" + tgName;
            string url = "https://localhost:7195/api/Passenger/PostPassenger";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string userJson = JsonConvert.SerializeObject(passenger);
                    streamWriter.Write(userJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;
                using (StreamReader Streamreader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = Streamreader.ReadToEnd();
                }
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Post Error: {ex.Message}");
            }

        }

        internal static string GetAllPassenger()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Passenger/GetAll";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Passenger> viewResponses = JsonConvert.DeserializeObject<List<Passenger>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"ID = {viewResponse.TgID}\n" +
                                  $"Username {viewResponse.Name}\n" +
                                  $"Telegramname {viewResponse.TgName}\n\n\n";
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

        internal static List<long> GetAllPassengerID()
        {
            List<long> answers = new List<long>();
            string url = "https://localhost:7195/api/Passenger/GetAll";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Passenger> viewResponses = JsonConvert.DeserializeObject<List<Passenger>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answers.Add(viewResponse.TgID);
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при выполнении HTTP-запроса
                Console.WriteLine($"View Error: {ex.Message}");
            }

            return answers;
        }

    }
}
