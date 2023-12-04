using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace ModernTramTgBot.Requests
{
    internal class IncidentRequests
    {
        static Dictionary<int,string> incidents = new Dictionary<int,string>();
        internal static string GetAllTodayIncident()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Incident/GetAllTodayIncident";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Incident> viewResponses = JsonConvert.DeserializeObject<List<Incident>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Час аварії : {viewResponse.IncDateTime}\n" +
                                  $"Опис : {viewResponse.IncDescription}\n" +
                                  $"Статус поломки: {viewResponse.IncStatus}\n" +
                                  $"Номер трамваю: {viewResponse.TramID}\n\n\n";
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

        internal static string GetAllIncident()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Incident/GetAllIncidents";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Incident> viewResponses = JsonConvert.DeserializeObject<List<Incident>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"ID : {viewResponse.ID}\n" +
                                  $"Час аварії : {viewResponse.IncDateTime}\n" +
                                  $"Опис : {viewResponse.IncDescription}\n" +
                                  $"Статус поломки: {viewResponse.IncStatus}\n" +
                                  $"Номер трамваю: {viewResponse.TramID}\n\n\n";
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
                answer = $"View Error: {ex.Message}";
            }

            return answer;
        }

        internal static string AddIncident(DateTime incDateTime, string incDescription, string incStatus, int tramId)
        {
            Incident incident = new Incident
            {
                ID = 0,
                IncDateTime = incDateTime,
                IncDescription = incDescription,
                IncStatus = incStatus,
                TramID = tramId
            };

            string url = "https://localhost:7195/api/Incident/AddIncident";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string incidentJson = JsonConvert.SerializeObject(incident);
                    streamWriter.Write(incidentJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    return "Аварія додана";
                }
                else
                {
                    return $"Невдалося додати аварію. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add Incident Error: {ex.Message}");
                return $"Помилка додавання: {ex.Message}";
            }
        }

        internal static string DeleteIncident(int id)
        {
            string url = $"https://localhost:7195/api/Incident/RemoveIncident/{id}";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE";
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Incident deleted successfully.");
                    return "Аварію видалено";
                }
                else
                {
                    Console.WriteLine($"Failed to delete incident. Status code: {httpWebResponse.StatusCode}");
                    return $"Невдалося видалити аварію. Код помилки: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Incident Error: {ex.Message}");
                return $"Помилка видалення: {ex.Message}";
            }
        }
        internal static void UpdateIncident(int id, DateTime incDateTime, string incDescription, string incStatus, int tramId)
        {
            Incident incident = new Incident
            {
                ID = id,
                IncDateTime = incDateTime,
                IncDescription = incDescription,
                IncStatus = incStatus,
                TramID = tramId
            };

            string url = $"https://localhost:7195/api/incident/UpdateIncident";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "PUT";
                httpWebRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string incidentJson = JsonConvert.SerializeObject(incident);
                    streamWriter.Write(incidentJson);
                    streamWriter.Flush();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Аварію оновленно успішно.");
                }
                else
                {
                    Console.WriteLine($"Невдалося оновити аварію. Status code: {httpWebResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка оновлення: {ex.Message}");
            }
        }

        internal static string NoticeIncident()
        {

            string url = "https://localhost:7195/api/Incident/GetAllTodayIncident";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
            string response;
            using (StreamReader Streamreader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = Streamreader.ReadToEnd();
            }
            List<Incident> Incidents = JsonConvert.DeserializeObject<List<Incident>>(response);
            bool inIdList = false;
            bool inStatus = false;
            string NoticeIncident = null;
            foreach (var incid in Incidents)
            {
                inIdList = false; 
                inStatus = false;
                foreach (var inc in incidents)
                {
                    if (inc.Key == incid.ID)
                    {
                        inIdList = true;
                        if (inc.Value != incid.IncStatus)
                        {
                            inStatus = true;
                        }
                    }
                }
                if (inIdList != true)
                {
                    NoticeIncident += $"🔴Аварія🔴\n" +
                                  $"Номер трамваю: {incid.TramID}\n" +
                                  $"Опис : {incid.IncDescription}\n" +
                                  $"Статус поломки: {incid.IncStatus}\n" +
                                  $"Час аварії : {incid.IncDateTime}\n\n\n";
                    incidents.Add(incid.ID,incid.IncStatus) ;
                }
                else if (inStatus == true)
                {
                    if (incid.IncStatus == "Закінченно")
                    {
                        NoticeIncident += $"🟢Аварія🟢\n" +
                                  $"Номер трамваю: {incid.TramID}\n" +
                                  $"Опис : {incid.IncDescription}\n" +
                                  $"Статус поломки: {incid.IncStatus}\n" +
                                  $"Час аварії : {incid.IncDateTime}\n\n\n";
                    }
                    else
                    {
                        NoticeIncident += $"🟡Аварія🟡\n" +
                                  $"Номер трамваю: {incid.TramID}\n" +
                                  $"Опис : {incid.IncDescription}\n" +
                                  $"Статус поломки: {incid.IncStatus}\n" +
                                  $"Час аварії : {incid.IncDateTime}\n\n\n";
                    }
                    incidents[incid.ID] = incid.IncStatus;
                }
            }
            return NoticeIncident;

        }

    }
}
