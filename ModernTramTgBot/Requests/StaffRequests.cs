using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class StaffRequests
    {

        internal static string GetAllStaff()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Staff/GetAllStaff";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Staff> viewResponses = JsonConvert.DeserializeObject<List<Staff>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"TgID = {viewResponse.ID}\n" +
                                  $"Name = {viewResponse.Name}\n" +
                                  $"TgName = {viewResponse.Qualification}\n" +
                                  $"TgName = {viewResponse.Position}\n" +
                                  $"OperatorPasswor = {viewResponse.TechnicalStaffPassword}\n\n\n";
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

        internal static string AddStaff(int id, string name, string position, string qualification, string password)
        {
            Staff staff = new Staff();
            staff.ID = id;
            staff.Name = name;
            staff.Position = position;
            staff.Qualification = qualification;
            staff.TechnicalStaffPassword = password;

            string url = "https://localhost:7195/api/Staff/PostStaff";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string staffJson = JsonConvert.SerializeObject(staff);
                    streamWriter.Write(staffJson);
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
        internal static string DeleteStaff(int id)
        {
            string url = "https://localhost:7195/api/Staff/Delete/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE"; // Set the method to DELETE
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                // Check the response status and handle it as needed
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Staff deleted successfully.");
                    return "Персонал видалено успішно";
                }
                else
                {
                    Console.WriteLine($"Failed to delete staff. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося видалити персонал. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string UpdateStaff(int id, string name, string position, string qualification, string password)
        {
            Staff staff = new Staff();
            staff.ID = id;
            staff.Name = name;
            staff.Position = position;
            staff.Qualification = qualification;
            staff.TechnicalStaffPassword = password;

            string url = $"https://localhost:7195/api/Staff/UpdateStaff";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string staffJson = JsonConvert.SerializeObject(staff);
                    streamWriter.Write(staffJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Staff updated successfully.");
                    return "Персонал оновленно";
                }
                else
                {
                    Console.WriteLine($"Failed to update staff. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося оновити. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string IsStaff(int id)
        {
            string url = "https://localhost:7195/api/Staff/IsStaff/" + id;
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
