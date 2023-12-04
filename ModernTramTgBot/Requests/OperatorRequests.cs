using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class OperatorRequests
    {

        internal static string GetAllOperators()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Operator/GetAllOpearator";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<TramOperator> viewResponses = JsonConvert.DeserializeObject<List<TramOperator>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"TgID = {viewResponse.OpTgID}\n" +
                                  $"Name = {viewResponse.Name}\n" +
                                  $"TgName = {viewResponse.TgName}\n" +
                                  $"OperatorPasswor = {viewResponse.OperatorPasswor}\n\n\n";
                    }
                }
                else
                {
                    answer = "Нічого не знайденно";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            return answer;
        }

        internal static string AddOperator(int tgId, string name, string tgName, string password)
        {
            TramOperator tramOperator = new TramOperator();
            tramOperator.OpTgID = tgId;
            tramOperator.Name = name;
            tramOperator.TgName = tgName;
            tramOperator.OperatorPasswor = password;

            string url = "https://localhost:7195/api/Operator/PostOper";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string operJson = JsonConvert.SerializeObject(tramOperator);
                    streamWriter.Write(operJson);
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

                return $"Помилка додавання : {ex.Message}";
            }

        }
        internal static string DeleteOperator(int id)
        {
            string url = "https://localhost:7195/api/Operator/Delete/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE"; // Set the method to DELETE
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("TramOperator deleted successfully.");
                    return "Видалено";
                }
                else
                {
                    Console.WriteLine($"Failed to delete oper. Status code: {httpWebResponse.StatusCode}");
                    return $"Помилка видалення. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string UpdateOperator(int tgId, string name, string tgName, string password)
        {
            TramOperator tramOperator = new TramOperator();
            tramOperator.OpTgID = tgId;
            tramOperator.Name = name;
            tramOperator.TgName = tgName;
            tramOperator.OperatorPasswor = password;

            string url = $"https://localhost:7195/api/Operator/UpdateOper";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string operJson = JsonConvert.SerializeObject(tramOperator);
                    streamWriter.Write(operJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("TramOperator updated successfully.");
                    return "Оновелнно";
                }
                else
                {
                    Console.WriteLine($"Failed to update tram. Status code: {httpWebResponse.StatusCode}");
                    return $"Помилка оновлення. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string IsOperator(int id)
        {
            string url = "https://localhost:7195/api/Operator/IsOper/" + id;
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
                return $"Помилка: {ex.Message}";
            }

            return answer;
        }


    }
}
