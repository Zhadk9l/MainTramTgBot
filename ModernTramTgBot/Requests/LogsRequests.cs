using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class LogsRequests
    {

        internal static string GetAllLogs()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/MaintenanceLog/GetAllLogs";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Logs> viewResponses = JsonConvert.DeserializeObject<List<Logs>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"ID = {viewResponse.ID}\n" +
                                  $"Дата = {viewResponse.ScheduledService}\n" +
                                  $"Опис = {viewResponse.RepairDescription}\n" +
                                  $"Номер трамвая = {viewResponse.TramID}\n" +
                                  $"Майстер = {viewResponse.TechnicalStaff}\n\n\n";
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

        internal static string AddLog(int id, string descript, int tramId, int staff)
        {
            Logs log = new Logs();
            log.ID = 0;
            log.ScheduledService = DateTime.Now;
            log.RepairDescription = descript;
            log.TramID = tramId;
            log.TechnicalStaff = staff;

            string url = "https://localhost:7195/api/MaintenanceLog/PostLogs";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string logJson = JsonConvert.SerializeObject(log);
                    streamWriter.Write(logJson);
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
        internal static string DeleteLogs(int id)
        {
            string url = "https://localhost:7195/api/MaintenanceLog/Delete/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE"; // Set the method to DELETE
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                // Check the response status and handle it as needed
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Log deleted successfully.");
                    return "Запис видалено успішно";
                }
                else
                {
                    Console.WriteLine($"Failed to delete log. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося видалити запис. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string UpdateLogs(int id, string descript, int tramId, int staff)
        {
            Logs log = new Logs();
            log.ID = id;
            log.ScheduledService = DateTime.Now;
            log.RepairDescription = descript;
            log.TramID = tramId;
            log.TechnicalStaff = staff;

            string url = $"https://localhost:7195/api/MaintenanceLog/UpdateLogs";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string logsJson = JsonConvert.SerializeObject(log);
                    streamWriter.Write(logsJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Log updated successfully.");
                    return "Запис оновленно";
                }
                else
                {
                    Console.WriteLine($"Failed to update log. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося оновити запис. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }


    }
}
