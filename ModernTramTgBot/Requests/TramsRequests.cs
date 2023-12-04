using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.AccessControl;

namespace ModernTramTgBot.Requests
{
    internal class TramsRequests
    {
        internal static string GetAllTramsWithMov()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Tram/GetAllTramsWithTramMove";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<TramWithLastMovement> viewResponses = JsonConvert.DeserializeObject<List<TramWithLastMovement>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Номер трамваю: {viewResponse.Id}\n" +
                                  $"Модель трамваю : {viewResponse.BrandAndModel}\n" +
                                  $"Стан трамваю : {viewResponse.Condition}\n" +
                                  $"Час останнього місц.пол. : {viewResponse.LastMovementDateTime}\n" +
                                  $"Кординати : {viewResponse.LastMovementCoordinates}\n" +
                                  $"Швидкість : {viewResponse.LastMovementSpeed}\n" +
                                  $"Напрямок: {viewResponse.LastMovementDirection}\n\n\n";
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

        internal static string GetAllTrams()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Tram/GetAllTram";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Tram> viewResponses = JsonConvert.DeserializeObject<List<Tram>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"ID = {viewResponse.Id}\n" +
                                  $"BrandAndModel = {viewResponse.BrandAndModel}\n" +
                                  $"Condition = {viewResponse.Condition}\n\n\n";
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
                return $"Помилка: {ex.Message}";
            }

            return answer;
        }

        internal static string GetAllTramsByOpId(int id)
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Tram/GetTramsByOperator/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Tram> viewResponses = JsonConvert.DeserializeObject<List<Tram>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Номер = {viewResponse.Id}\n" +
                                  $"Модель = {viewResponse.BrandAndModel}\n" +
                                  $"Стан = {viewResponse.Condition}\n\n\n";
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
                return $"Помилка: {ex.Message}";
            }

            return answer;
        }

        internal static string AddTram(int Id, string brandAndModel, string condition)
        {
            Tram tram = new Tram();
            tram.Id = Id;
            tram.BrandAndModel = brandAndModel;
            tram.Condition = condition;

            string url = "https://localhost:7195/api/Tram/PostTram";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string tramJson = JsonConvert.SerializeObject(tram);
                    streamWriter.Write(tramJson);
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
                return "Додано";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Post Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }

        }
        internal static void DeleteTram(int id)
        {
            string url = "https://localhost:7195/api/Tram/Delete/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE"; // Set the method to DELETE
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                // Check the response status and handle it as needed
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Tram deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to delete tram. Status code: {httpWebResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
            }
        }

        internal static string UpdateTram(int id, string brandAndModel, string condition)
        {
            Tram tram = new Tram
            {
                Id = id,
                BrandAndModel = brandAndModel,
                Condition = condition
            };

            string url = $"https://localhost:7195/api/Tram/UpdateTram";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string tramJson = JsonConvert.SerializeObject(tram);
                    streamWriter.Write(tramJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Tram updated successfully.");
                    return "Оновленно";
                }
                else
                {
                    Console.WriteLine($"Failed to update tram. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося оновити. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return $"Помлика: {ex.Message}";
            }
        }


    }
}
