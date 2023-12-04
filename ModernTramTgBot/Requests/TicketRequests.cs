using ModernTramTgBot.Models;
using Newtonsoft.Json;
using System.Net;

namespace ModernTramTgBot.Requests
{
    internal class TicketRequests
    {
        internal static string GetAllTickets()
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Ticket/GetAllTickets";

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                List<Ticket> tickets = JsonConvert.DeserializeObject<List<Ticket>>(response);

                if (tickets != null && tickets.Any())
                {
                    foreach (var ticket in tickets)
                    {
                        answer += $"Ticket ID: {ticket.TicketId}\n" +
                                  $"Purchase Date: {ticket.PurchaseDateTime}\n" +
                                  $"Expiry Date: {ticket.ExpiryDateTime}\n" +
                                  $"Price: {ticket.Price}\n" +
                                  $"Tg ID: {ticket.TgID}\n\n\n";
                    }
                }
                else
                {
                    answer = "Нвчого не знайдено";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get All Tickets Error: {ex.Message}");
                answer = $"Помилка: {ex.Message}";
            }

            return answer;
        }

        internal static string GetAllTicketsForPessengare(int id)
        {
            string answer = string.Empty;
            string url = "https://localhost:7195/api/Ticket/GetAllTicketsForPessengare/" + id;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                List<Ticket> viewResponses = JsonConvert.DeserializeObject<List<Ticket>>(response);

                if (viewResponses != null && viewResponses.Any())
                {
                    foreach (var viewResponse in viewResponses)
                    {
                        answer += $"Номер білету: {viewResponse.TicketId}\n" +
                                  $"Дата покупки: {viewResponse.PurchaseDateTime}\n" +
                                  $"Дата закінчення: {viewResponse.ExpiryDateTime}\n" +
                                  $"Ціна: {viewResponse.Price}₴\n\n\n";
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
            }

            return answer;
        }

        internal static void BuyTicket(long tgId, decimal price, string time)
        {
            Ticket ticket = new Ticket();
            ticket.TgID = Int32.Parse(tgId.ToString()); 
            ticket.PurchaseDateTime = DateTime.Now;
            ticket.ExpiryDateTime = DateTime.Now.AddHours(Double.Parse(time));
            ticket.Price = price;

            string url = "https://localhost:7195/api/Ticket/PostTicket";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST"; // Устанавливаем метод POST
                httpWebRequest.ContentType = "application/json"; // Устанавливаем тип контента JSON

                // Преобразуем объект User в JSON-строку и отправляем в тело запроса
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string ticketJson = JsonConvert.SerializeObject(ticket);
                    streamWriter.Write(ticketJson);
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

        internal static string DeleteTicket(int ticketId)
        {
            string url = $"https://localhost:7195/api/Ticket/Delete/" + ticketId;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE";
                httpWebRequest.ContentType = "application/json";

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("Ticket deleted successfully.");
                    return "Додано";
                }
                else
                {
                    Console.WriteLine($"Failed to delete ticket. Status code: {httpWebResponse.StatusCode}");
                    return $"Помилка додавання. Status code: {httpWebResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Ticket Error: {ex.Message}");
                return $"Помилка: {ex.Message}";
            }
        }

        internal static string IsValidTicket(int id)
        {
            string url = "https://localhost:7195/api/Ticket/HasValidTicket/" + id;
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

                bool viewResponses = JsonConvert.DeserializeObject<bool>(response);
                if (viewResponses == true) 
                {
                    answer = "Пасажир має активний квиток";
                }
                else
                {
                    answer = "Пасажир заєць";
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"View Error: {ex.Message}");
                answer = $"Помилка:{ex.Message}";
            }

            return answer;
        }


    }
}

