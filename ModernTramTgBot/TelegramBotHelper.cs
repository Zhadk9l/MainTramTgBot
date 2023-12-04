using ModernTramTgBot.Requests;
using ModernTramTgBot.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModernTramTgBot
{
    public class TelegramBotHelper
    {
        private const string Text_Schedule = "Розклад";
        private const string Text_Tickets = "Квитки";
        private const string Text_Tickets_GetAll = "Перелік квитків";
        private const string Text_Tickets_Buy = "Купити";
        private const string Text_Tickets_Buy_OneH = "1 година";
        private const string Text_Tickets_Buy_ThreeH = "3 години";
        private const string Text_AddTickets = "Додати білет";
        private const string Text_DeleteTickets = "Видалити білет";
        private const string Text_Incident = "Аварії";
        private const string Text_PostIncident = "Нова аварія";
        private const string Text_UpdateIncident = "Перезаписати аварію";
        private const string Text_DeleteIncident= "Видалити аварію";
        private const string Text_Trams = "Трамваї";
        private const string Text_PostTrams = "Новий трамвай";
        //private const string Text_DeleteTrams = "Видалити трамвай";
        private const string Text_UpdateTrams = "Перезаписати трамвай";
        private const string Text_Back = "Назад";
        private const string Text_Next = "-->";
        private const string Text_Prev = "<--";

        private const string Text_Logs = "Журнал";
        private const string Text_Logs_Add = "Додати запис";
        private const string Text_Logs_Del = "Видалити запис";
        private const string Text_Logs_Update = "Перезаписати запис";

        private const string Text_TramOperators = "Оператор";
        private const string Text_MyTramConditions = "Мої трамваї";
        private const string Text_MyRoutes = "Мої маршрути";
        private const string Text_IsPeassengerHaveTicket = "Перевірка білету";

        private string Check = null;



        private const string Text_Admin = "Адмін";
        private const string Text_Admin_Passenger = "Пасажири";
        private const string Text_Admin_GetAll = "Перелік";
        private const string Text_PostOperator = "Новий оператор";
        private const string Text_UpdateOperator = "Перезаписати оператора";
        private const string Text_Admin_TechnicalStaff = "Технічний персонал";
        private const string Text_PostStaff = "Новий персонал";
        private const string Text_UpdateStaff = "Перезаписати персонал";


        private string token;
        Telegram.Bot.TelegramBotClient _client;
        private Dictionary<long,ModernState> _clientStates = new Dictionary<long, ModernState>();

        public TelegramBotHelper(string token)
        {
            this.token = token;
        }

        internal void GetUpdates()
        {
            _client = new Telegram.Bot.TelegramBotClient(token);
            var me = _client.GetMeAsync().Result;
            if(me != null && !string.IsNullOrEmpty(me.Username))
            {
                int offset = 0;
                while (true)
                {
                    try
                    {
                        var updates = _client.GetUpdatesAsync(offset).Result;
                        string Notice = IncidentRequests.NoticeIncident();
                        if (Notice != null)
                        {
                            List<long> ids = PassengerRequests.GetAllPassengerID();
                            foreach (long id in ids)
                            {
                                _client.SendTextMessageAsync(id, Notice);
                            }
                        }
                        if (updates != null && updates.Count() > 0) 
                        {
                            foreach (var update in updates)
                            {
                                processUpdate(update);
                                offset = update.Id + 1;
                            }
                        }
                    }
                    catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                    Thread.Sleep(1000);
                }
            }
        }

        private void processUpdate(Update update)
        {
            switch(update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    UserInfo(update.Message);
                    var text = update.Message.Text;
                    var state = _clientStates.ContainsKey(update.Message.Chat.Id) ? _clientStates[update.Message.Chat.Id] : null;
                    if (state != null)   
                    {
                        switch (state.State)
                        {
                            //ADMIN LEVEL
                            case State.PaswordAdminLev:
                                bool chk = ISAdminPassword(update.Message.Text, update.Message.Chat.Id); //Проверка на пароль и на Админа
                                if (chk == true)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Добрий день админ", replyMarkup: getAdminButtons());
                                    state.State = State.AdminMainLev;
                                }
                                else
                                {
                                    _clientStates[update.Message.Chat.Id] = null;
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Неверно", replyMarkup: GetPessangerButtonsFirstPage());
                                }
                                break;
                            case State.AdminMainLev:
                                switch (text)
                                {
                                    case Text_Admin_Passenger:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню пассажири", replyMarkup: getAdminPassengerButtons());
                                        state.State = State.AdminPassengerLev;
                                        break;
                                    case Text_Trams:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню трамваї", replyMarkup: getAdminTramButtons());
                                        state.State = State.AdminTramLev;
                                        break;
                                    case Text_TramOperators:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню оператори", replyMarkup: getAdminOperatorButtons());
                                        state.State = State.AdminOperatorLev;
                                        break;
                                    case Text_Admin_TechnicalStaff:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню технічний персонал", replyMarkup: getAdminStaffButtons());
                                        state.State = State.AdminStaffLev;
                                        break;
                                    case Text_Incident:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню аварії", replyMarkup: getAdminIncidentButtons());
                                        state.State = State.AdminIncidentLev;
                                        break;
                                    case Text_Tickets:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Меню квитки", replyMarkup: getAdminTicketButtons());// TODO Реализовать Admin меню Tickets
                                        state.State = State.AdminTicketLev;
                                        break;
                                    case Text_Back:
                                        _clientStates[update.Message.Chat.Id] = null;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: GetPessangerButtonsFirstPage());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminPassengerLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, PassengerRequests.GetAllPassenger(), replyMarkup: getAdminPassengerButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminTramLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.GetAllTrams(), replyMarkup: getAdminTramButtons());
                                        break;
                                    case Text_PostTrams:
                                        state.State = State.AdminTramChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати трамвай треба написати в повідомленні /n та через коми id,model,condotion\n приклад /n 1,КТМ-5,Good", replyMarkup: getBackButtons());

                                        break;
                                    case Text_UpdateTrams:
                                        state.State = State.AdminTramChangesLev;
                                        Check = "/u";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати трамвай треба написати в повідомленні /u та через коми id,model,condotion\n приклад /u 1,КТМ-5,Good", replyMarkup: getBackButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminTramChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseTramMessage(text);

                                    int id = result.id;
                                    string model = result.model;
                                    string condition = result.condition;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.AddTram(id, model, condition), replyMarkup: getAdminTramButtons());
                                    state.State = State.AdminTramLev;
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseTramMessage(text);

                                    int id = result.id;
                                    string model = result.model;
                                    string condition = result.condition;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.UpdateTram(id, model, condition), replyMarkup: getAdminTramButtons());
                                    state.State = State.AdminTramLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminTramButtons());
                                    state.State = State.AdminTramLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.AdminOperatorLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, OperatorRequests.GetAllOperators(), replyMarkup: getAdminOperatorButtons());
                                        break;
                                    case Text_PostOperator:
                                        state.State = State.AdminOperatorChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати оператора треба написати в повідомленні /n та через коми id,name,tgname,pass\n приклад /n 1,Danil,@zhad9l,111", replyMarkup: getBackButtons());

                                        break;
                                    case Text_UpdateOperator:
                                        state.State = State.AdminOperatorChangesLev;
                                        Check = "/u";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати оператора треба написати в повідомленні /u та через коми id,name,tgname,pass\n приклад /u 1,Danil,@zhad9l,111", replyMarkup: getBackButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminOperatorChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseOperatorMessage(text);
                                    int id = result.id;
                                    string name = result.name;
                                    string tgname = result.tgname;
                                    string password = result.password;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, OperatorRequests.AddOperator(id, name, tgname, password), replyMarkup: getAdminOperatorButtons());
                                    state.State = State.AdminOperatorLev;
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseOperatorMessage(text);
                                    int id = result.id;
                                    string name = result.name;
                                    string tgname = result.tgname;
                                    string password = result.password;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, OperatorRequests.UpdateOperator(id, name, tgname, password), replyMarkup: getAdminOperatorButtons());
                                    state.State = State.AdminOperatorLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminOperatorButtons());
                                    state.State = State.AdminOperatorLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.AdminStaffLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, StaffRequests.GetAllStaff(), replyMarkup: getAdminStaffButtons());
                                        break;
                                    case Text_PostStaff:
                                        state.State = State.AdminStaffChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати працівника треба написати в повідомленні /n та через коми id,name,position,qualification,pass\n приклад /n 1,Danil,механік,головний,111", replyMarkup: getBackButtons());

                                        break;
                                    case Text_UpdateStaff:
                                        state.State = State.AdminStaffChangesLev;
                                        Check = "/u";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати працівника треба написати в повідомленні /u та через коми id,name,tgname,pass\n приклад /u 1,Danil,механік,головний,111", replyMarkup: getBackButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminStaffChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseStaffMessage(text);
                                    int id = result.id;
                                    string name = result.name;
                                    string position = result.position;
                                    string qualif = result.qualif;
                                    string password = result.password;

                                    _client.SendTextMessageAsync(update.Message.Chat.Id, StaffRequests.AddStaff(id, name, position, qualif, password), replyMarkup: getAdminStaffButtons());
                                    state.State = State.AdminStaffLev;
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseStaffMessage(text);
                                    int id = result.id;
                                    string name = result.name;
                                    string position = result.position;
                                    string qualif = result.qualif;
                                    string password = result.password;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, StaffRequests.UpdateStaff(id, name, position, qualif, password), replyMarkup: getAdminStaffButtons());
                                    state.State = State.AdminStaffLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminStaffButtons());
                                    state.State = State.AdminStaffLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.AdminIncidentLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.GetAllIncident(), replyMarkup: getAdminIncidentButtons());
                                        break;
                                    case Text_PostIncident:
                                        state.State = State.AdminIncidentChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати аварію треба написати в повідомленні /n та через коми description,status,tramId\n приклад /n ДТП,Pending,2", replyMarkup: getBackButtons());

                                        break;
                                    case Text_UpdateIncident:
                                        state.State = State.AdminIncidentChangesLev;
                                        Check = "/u";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати аварію треба написати в повідомленні /u та через коми id,description,status,tramId\n приклад /u 1,ДТП,Pending,2", replyMarkup: getBackButtons());
                                        break;
                                    case Text_DeleteIncident:
                                        state.State = State.AdminIncidentChangesLev;
                                        Check = "/d";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб видалити аварію треба написати в повідомленні /d та id\n приклад /d 1", replyMarkup: getBackButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminIncidentChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseIncidentMessage(text);
                                    DateTime dateTime = DateTime.Now;
                                    string desc = result.IncDescription;
                                    string status = result.IncStatus;
                                    int tramId = result.TramID;

                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.AddIncident(dateTime, desc, status, tramId), replyMarkup: getAdminIncidentButtons());
                                    state.State = State.AdminIncidentLev;
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseIncidentUpdateMessage(text);
                                    int id = result.id;
                                    DateTime dateTime = DateTime.Now;
                                    string desc = result.IncDescription;
                                    string status = result.IncStatus;
                                    int tramId = result.TramID;

                                    IncidentRequests.UpdateIncident(id, dateTime, desc, status, tramId);
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, $"Оновлено", replyMarkup: getAdminIncidentButtons());
                                    state.State = State.AdminIncidentLev;
                                }
                                else if (text.StartsWith("/d ") && Check == "/d")
                                {

                                    string prefixToRemove = "/d ";
                                    string idString = text.Substring(prefixToRemove.Length);

                                    if (int.TryParse(idString, out int id))
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.DeleteIncident(id), replyMarkup: getStaffLogsButtons());
                                        state.State = State.AdminIncidentLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Помилка id формат. ID має бути int");
                                    }
                                    
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminIncidentButtons());
                                    state.State = State.AdminIncidentLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.AdminTicketLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TicketRequests.GetAllTickets(), replyMarkup: getAdminTicketButtons());
                                        break;
                                    case Text_AddTickets:
                                        state.State = State.AdminTicketChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати білет треба написати в повідомленні /n та через коми час_квитка,ціну,idTg\n приклад /n 2,40,684186713", replyMarkup: getBackButtons());
                                        break;
                                  
                                    case Text_DeleteTickets:
                                        state.State = State.AdminTicketChangesLev;
                                        Check = "/d";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб видалити білет треба написати в повідомленні /d та id\n приклад /d 1", replyMarkup: getBackButtons());
                                        break;

                                    case Text_Back:
                                        state.State = State.AdminMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.AdminTicketChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseTicketMessage(text);
                                    decimal price = result.price;
                                    int tgId = result.TgId;
                                    string time = result.time;

                                    TicketRequests.BuyTicket(tgId, price, time);
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, $"Додано", replyMarkup: getAdminTicketButtons());
                                    state.State = State.AdminTicketLev;
                                }
                                else if (text.StartsWith("/d ") && Check == "/d")
                                {

                                    string prefixToRemove = "/d ";

                                    string id = text.Substring(prefixToRemove.Length);
                                    
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, TicketRequests.DeleteTicket(Int32.Parse(id)), replyMarkup: getAdminTicketButtons());
                                    state.State = State.AdminTicketLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getAdminTicketButtons());
                                    state.State = State.AdminTicketLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;

                            //Pesenger TICKET 

                            case State.PassengerTicket:
                                switch (text)
                                {
                                    case Text_Tickets_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TicketRequests.GetAllTicketsForPessengare(Int32.Parse(update.Message.Chat.Id.ToString())), replyMarkup: GetPessangerTicketsButtons());
                                        break;
                                    case Text_Tickets_Buy:
                                        state.State = State.PassengerTicketBuy;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "1 година 20 грн \n3 години 45 грн ", replyMarkup: GetPessangerTicketsBuyButtons());
                                        break;

                                    case Text_Back:
                                        _clientStates[update.Message.Chat.Id] = null;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: GetPessangerButtonsFirstPage());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.PassengerTicketBuy:
                                switch (text)
                                {
                                    case Text_Tickets_Buy_OneH:
                                        TicketRequests.BuyTicket(update.Message.Chat.Id,20,"1");
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, $"Дякуємо за покупку", replyMarkup: GetPessangerTicketsBuyButtons());//TODO реализовать покупку 
                                        break;
                                    case Text_Tickets_Buy_ThreeH:
                                        TicketRequests.BuyTicket(update.Message.Chat.Id, 45, "3");
                                        state.State = State.PassengerTicketBuy;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, $"Дякуємо за покупку", replyMarkup: GetPessangerTicketsBuyButtons());//TODO реализовать покупку 
                                        break;

                                    case Text_Back:
                                        state.State = State.PassengerTicket;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: GetPessangerTicketsButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;

                            //Operator LEVEL

                            case State.PaswordOperatorLev:
                                bool operChk = ISOperatorPassword(update.Message.Text, update.Message.Chat.Id);
                                if (operChk == true)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Доброго дня оператор", replyMarkup: getOperatorsButtons());
                                    state.State = State.OperatorMainLev;
                                }
                                else
                                {
                                    _clientStates[update.Message.Chat.Id] = null;
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Неверно", replyMarkup: GetPessangerButtonsFirstPage());
                                }
                                break;
                            case State.OperatorMainLev:
                                switch (text)
                                {
                                    case Text_MyTramConditions:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.GetAllTramsByOpId(Int32.Parse(update.Message.Chat.Id.ToString())), replyMarkup: getOperatorsButtons());
                                        break;
                                    case Text_MyRoutes:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, ScheduleRequests.GetlScheduleByOperId(Int32.Parse(update.Message.Chat.Id.ToString())), replyMarkup: getOperatorsButtons());
                                        break;
                                    case Text_Incident:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.GetAllTodayIncident(), replyMarkup: getOperatorsButtons());
                                        break;
                                    case Text_PostIncident:
                                        state.State = State.IncidentChangesLev;
                                        Check = "/n";
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати аварію треба написати в повідомленні /n та через коми description,status,tramId\\n приклад /n ДТП,Pending,2", replyMarkup: getBackButtons());
                                        break;
                                    case Text_IsPeassengerHaveTicket:
                                        Check = "/t";
                                        state.State = State.IncidentChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перевірити квиток пасажира треба написати в повідомленні /t та id пасажира\n приклад /t 684186713", replyMarkup: getOperatorsButtons());
                                        break;
                                    case Text_Back:
                                        _clientStates[update.Message.Chat.Id] = null;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: GetPessangerButtonsFirstPage());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.IncidentChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseIncidentMessage(text);
                                    DateTime dateTime = DateTime.Now;
                                    string desc = result.IncDescription;
                                    string status = result.IncStatus;
                                    int tramId = result.TramID;

                                    if(tramId != 0)
                                    {
                                        
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.AddIncident(dateTime, desc, status, tramId), replyMarkup: getOperatorsButtons());
                                        state.State = State.OperatorMainLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, $"Неправвельний формат(текст,текст,число)", replyMarkup: getOperatorsButtons());
                                        state.State = State.OperatorMainLev;
                                    }
                                }
                                else if (text.StartsWith("/t ") && Check == "/t")
                                {
                                    string prefixToRemove = "/t ";
                                    string idString = text.Substring(prefixToRemove.Length);
                                    if (int.TryParse(idString, out int id))
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TicketRequests.IsValidTicket(id), replyMarkup: getOperatorsButtons());
                                        state.State = State.OperatorMainLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Неправвельний ID формат. Будьласка введіть числовий ID.", replyMarkup: getOperatorsButtons());
                                        state.State = State.OperatorMainLev;
                                    }
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getOperatorsButtons());
                                    state.State = State.OperatorMainLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.None: 
                                break;

                            //Staff LEVEL

                            case State.PaswordStaffLev:
                                bool staffChk = ISStaffPassword(update.Message.Text, update.Message.Chat.Id);
                                if (staffChk == true)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Доброго дня працівник", replyMarkup: getStaffButtons());
                                    state.State = State.StaffMainLev;
                                }
                                else
                                {
                                    _clientStates[update.Message.Chat.Id] = null;
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Неверно", replyMarkup: GetPessangerButtonsFirstPage());
                                }
                                break;
                            case State.StaffMainLev:
                                switch (text)
                                {
                                    case Text_Logs:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Logs, replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                        break;
                                    case Text_Incident:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Incident, replyMarkup: getStaffIncidentButtons());
                                        state.State = State.StaffIncLev;
                                        break;
                                    case Text_Trams:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Trams, replyMarkup: getStaffTramsButtons());
                                        state.State = State.StaffTramLev;
                                        break;
                                    case Text_Back:
                                        _clientStates[update.Message.Chat.Id] = null;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: GetPessangerButtonsFirstPage());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.StaffLogsLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, LogsRequests.GetAllLogs(), replyMarkup: getStaffLogsButtons());
                                        break;
                                    case Text_Logs_Add:
                                        Check = "/n";
                                        state.State = State.StaffLogsChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати запис треба написати в повідомленні /n та через коми Опис ремонту,id трамваю\\n приклад /n 1,ДТП,1", replyMarkup: getBackButtons());
                                        
                                        break;
                                    case Text_Logs_Del:
                                        Check = "/d";
                                        state.State = State.StaffLogsChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб видалити запис треба написати в повідомленні /d та через кому id \\n приклад /d 1", replyMarkup: getBackButtons());
                                   
                                        break;
                                    case Text_Logs_Update:
                                        Check = "/u";
                                        state.State = State.StaffLogsChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб оновити запис треба написати в повідомленні /u та через коми id,Опис ремонту,id трамваю\\n приклад /u 1,ДТП,1", replyMarkup: getBackButtons());
                                   
                                        break;
                                    case Text_Back:
                                        state.State = State.StaffMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.StaffLogsChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseLogMessage(text);
                                    int logid = result.id;
                                    int id = Int32.Parse(update.Message.Chat.Id.ToString());
                                    string desc = result.desc;
                                    int tramId = result.tramId;

                                    if (tramId != 0)
                                    {

                                        _client.SendTextMessageAsync(update.Message.Chat.Id, LogsRequests.AddLog(logid, desc, tramId, id), replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, $"Помилка формат(число,текст,число)", replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                    }
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseLogMessage(text);
                                    int logid = result.id;
                                    int id = Int32.Parse(update.Message.Chat.Id.ToString());
                                    string desc = result.desc;
                                    int tramId = result.tramId;
                                    if (tramId != 0)
                                    {

                                        _client.SendTextMessageAsync(update.Message.Chat.Id, LogsRequests.UpdateLogs(logid, desc, tramId, id), replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, $"Неправвельний формат(число,текст,число)", replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                    }
                                }
                                else if (text.StartsWith("/d ") && Check == "/d")
                                {
                                    string prefixToRemove = "/d ";
                                    string idString = text.Substring(prefixToRemove.Length);

                                    if (int.TryParse(idString, out int id))
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, LogsRequests.DeleteLogs(id), replyMarkup: getStaffLogsButtons());
                                        state.State = State.StaffLogsLev;
                                    }
                                    else
                                    {
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Помилка id формат. ID має бути int");
                                    }
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffLogsButtons());
                                    state.State = State.StaffLogsLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.StaffIncLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.GetAllIncident(), replyMarkup: getStaffIncidentButtons());
                                        break;
                                    case Text_UpdateIncident:
                                        Check = "/u";
                                        state.State = State.StaffIncChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати аварію треба написати в повідомленні /u та через коми id,description,status,tramId\n приклад /u 1,ДТП,Pending,2", replyMarkup: getBackButtons());
                                        break;
                                    case Text_Back:
                                        state.State = State.StaffMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.StaffIncChangesLev:
                                
                                if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseIncidentUpdateMessage(text);
                                    int id = result.id;
                                    DateTime dateTime = DateTime.Now;
                                    string desc = result.IncDescription;
                                    string status = result.IncStatus;
                                    int tramId = result.TramID;

                                    IncidentRequests.UpdateIncident(id, dateTime, desc, status, tramId);
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, $"Оновлено", replyMarkup: getStaffIncidentButtons());
                                    state.State = State.StaffIncLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffIncidentButtons());
                                    state.State = State.StaffIncLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                            case State.StaffTramLev:
                                switch (text)
                                {
                                    case Text_Admin_GetAll:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.GetAllTrams(), replyMarkup: getStaffTramsButtons());
                                        break;
                                    case Text_PostTrams:
                                        Check = "/n";
                                        state.State = State.StaffTramsChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб додати трамвай треба написати в повідомленні /n та через коми id,model,condotion\n приклад /n 1,КТМ-5,Good", replyMarkup: getBackButtons());
                                        break;

                                    case Text_UpdateTrams:
                                        Check = "/u";
                                        state.State = State.StaffTramsChangesLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Щоб перезаписати трамвай треба написати в повідомленні /u та через коми id,model,condotion\n приклад /u 1,КТМ-5,Good", replyMarkup: getBackButtons());

                                        break;
                                    case Text_Back:
                                        state.State = State.StaffMainLev;
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffButtons());
                                        break;
                                    default:
                                        _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                        break;
                                }
                                break;
                            case State.StaffTramsChangesLev:
                                if (text.StartsWith("/n ") && Check == "/n")
                                {
                                    var result = ParseTramMessage(text);

                                    int id = result.id;
                                    string model = result.model;
                                    string condition = result.condition;


                                    _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.AddTram(id, model, condition), replyMarkup: getStaffTramsButtons());
                                    state.State = State.StaffTramLev;
                                }
                                else if (text.StartsWith("/u ") && Check == "/u")
                                {
                                    var result = ParseTramMessage(text);

                                    int id = result.id;
                                    string model = result.model;
                                    string condition = result.condition;


                                    _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.UpdateTram(id, model, condition), replyMarkup: getStaffTramsButtons());
                                    state.State = State.StaffTramLev;
                                }
                                else if (text == Text_Back)
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Back, replyMarkup: getStaffTramsButtons());
                                    state.State = State.StaffTramLev;
                                }
                                else
                                {
                                    _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (text)
                    {
                        case "/start":
                            PassengerRequests.PostPassenger(update.Message.Chat.Id, update.Message.Chat.FirstName, update.Message.Chat.Username);
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "Привіт", replyMarkup: GetPessangerButtonsFirstPage());
                            break;
                        case Text_Admin:
                            _clientStates[update.Message.Chat.Id] = new ModernState { State = State.PaswordAdminLev };
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "Введіть пароль:");
                            break;
                        case Text_TramOperators:
                            _clientStates[update.Message.Chat.Id] = new ModernState { State = State.PaswordOperatorLev };
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "Введіть пароль:");
                            break;
                        case Text_Admin_TechnicalStaff:
                            _clientStates[update.Message.Chat.Id] = new ModernState { State = State.PaswordStaffLev };
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "Введіть пароль:");
                            break;
                        case Text_Tickets:
                                _clientStates[update.Message.Chat.Id] = new ModernState { State = State.PassengerTicket };
                                _client.SendTextMessageAsync(update.Message.Chat.Id, Text_Tickets, replyMarkup: GetPessangerTicketsButtons()); //TODO Реализовать меню билетов
                            break;
                        case Text_Schedule:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, ScheduleRequests.GetAllSchedule(), replyMarkup: GetPessangerButtonsFirstPage()); //TODO Реализовать розписания
                            break;
                        case Text_Incident:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, IncidentRequests.GetAllTodayIncident(), replyMarkup: GetPessangerButtonsFirstPage()); //TODO Реализовать аварії
                            break;
                       case Text_Trams:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, TramsRequests.GetAllTramsWithMov(), replyMarkup: GetPessangerButtonsFirstPage()); //TODO Реализовать Trams
                            break;
                       case Text_Next:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "2 сторінка", replyMarkup: GetPessangerButtonsSecondPage());
                            break;
                       case Text_Prev:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "1 сторінка", replyMarkup: GetPessangerButtonsFirstPage());
                            break;
                            default:
                            _client.SendTextMessageAsync(update.Message.Chat.Id, "Wrong");
                            break;
                    }
                    }
                    break;
                default:
                    Console.WriteLine(update.Type + "Not impl");
                    break;
            }
        }

        private IReplyMarkup? getStaffTramsButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_UpdateTrams) },
                new List<KeyboardButton>{ new KeyboardButton(Text_PostTrams) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getStaffIncidentButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_UpdateIncident) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getStaffLogsButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_Logs_Add) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Logs_Update),new KeyboardButton(Text_Logs_Del) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getStaffButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Logs),new KeyboardButton(Text_Incident) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Trams)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private bool ISOperatorPassword(string? text, long id)
        {
            string password = OperatorRequests.IsOperator(Int32.Parse(id.ToString()));

            if (password != null && password == text) { return true; }
            return false;
        }
        private bool ISStaffPassword(string? text, long id)
        {
            string password = StaffRequests.IsStaff(Int32.Parse(id.ToString()));

            if (password != null && password == text) { return true; }
            return false;
        }

        private IReplyMarkup? getOperatorsButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_MyTramConditions),new KeyboardButton(Text_MyRoutes) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Incident),new KeyboardButton(Text_PostIncident) },
                new List<KeyboardButton>{ new KeyboardButton(Text_IsPeassengerHaveTicket) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminTicketButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_AddTickets) },
                new List<KeyboardButton>{ new KeyboardButton(Text_DeleteTickets) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminIncidentButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_PostIncident) },
                new List<KeyboardButton>{ new KeyboardButton(Text_UpdateIncident), new KeyboardButton(Text_DeleteIncident) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminStaffButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_PostStaff) },
                new List<KeyboardButton>{ new KeyboardButton(Text_UpdateStaff) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminOperatorButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_PostOperator) },
                new List<KeyboardButton>{ new KeyboardButton(Text_UpdateOperator)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getBackButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminTramButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll),new KeyboardButton(Text_PostTrams) },
                new List<KeyboardButton>{ new KeyboardButton(Text_UpdateTrams)/*,new KeyboardButton(Text_DeleteTrams) */},
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? GetPessangerTicketsBuyButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Tickets_Buy_OneH), new KeyboardButton(Text_Tickets_Buy_ThreeH)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? GetPessangerTicketsButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Tickets_GetAll), new KeyboardButton(Text_Tickets_Buy)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminPassengerButtons()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_GetAll) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private IReplyMarkup? getAdminButtons() //TODO реализовать Админ меню
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_Passenger), new KeyboardButton(Text_Trams), new KeyboardButton(Text_TramOperators) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin_TechnicalStaff), new KeyboardButton(Text_Incident), new KeyboardButton(Text_Tickets) },
                new List<KeyboardButton>{ new KeyboardButton(Text_Back)}
            })
            {
                ResizeKeyboard = true
            };
        }

        private bool ISAdminPassword(string? text, long id)
        {
            string password = AdminRequests.IsAdmin(Int32.Parse(id.ToString()));

            if (password != null && password == text) { return true; }
            return false;
        }


        private IReplyMarkup GetPessangerButtonsFirstPage() //TODO реализовать меню пасажира
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Tickets), new KeyboardButton(Text_Schedule)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Incident), new KeyboardButton(Text_Trams)},
                new List<KeyboardButton>{ new KeyboardButton(Text_Next)}
            })
            {
                ResizeKeyboard = true
            };
        }
        private IReplyMarkup GetPessangerButtonsSecondPage()
        {
            return new ReplyKeyboardMarkup(new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton(Text_Admin), new KeyboardButton(Text_TramOperators),new KeyboardButton(Text_Admin_TechnicalStaff)  },
                new List<KeyboardButton>{ new KeyboardButton(Text_Prev) }
            })
            {
                ResizeKeyboard = true
            };
        }


        private static void UserInfo(Telegram.Bot.Types.Message message)
        {
            string user = message.Chat.FirstName + " " + message.Chat.Username + " " + message.Date + " " + message.Text + " " + message.Chat.Id;
            Console.WriteLine(user);
        }

        public static (int id, string model, string condition) ParseTramMessage(string message)
        {
            string[] parts = message.Split(' ');

            int id = 0;
            string model = "";
            string condition = "";

            if (parts.Length == 2 && (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u"))
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 3)
                {
                    if (int.TryParse(values[0], out id))
                    {
                        model = values[1];
                        condition = values[2];
                    }
                }
            }

            return (id, model, condition);
        }

        public static (int id, string name, string tgname,string password) ParseOperatorMessage(string message)
        {
            string[] parts = message.Split(' ');

            int id = 0;
            string name = "";
            string tgname = "";
            string password = "";

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 4)
                {
                    if (int.TryParse(values[0], out id))
                    {
                        name = values[1];
                        tgname = values[2];
                        password = values[3];
                    }
                }
            }

            return (id, name, tgname, password);
        }

        public static (int id, string name, string position,string qualif, string password) ParseStaffMessage(string message)
        {
            string[] parts = message.Split(' ');

            int id = 0;
            string name = "";
            string position = "";
            string qualif = "";
            string password = "";

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 5)
                {
                    if (int.TryParse(values[0], out id))
                    {
                        name = values[1];
                        position = values[2];
                        qualif = values[3];
                        password = values[4];
                    }
                }
            }

            return (id, name, position,qualif, password);
        }

        public static (string IncDescription, string IncStatus, int TramID) ParseIncidentMessage(string message)
        {
            string[] parts = message.Split(' ');

            string IncDescription = "";
            string IncStatus = "";
            int TramID = 0;

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 3)
                {
                    if (int.TryParse(values[2], out TramID))
                    {
                        IncDescription = values[0];
                        IncStatus = values[1];
                    }
                }
            }

            return (IncDescription, IncStatus, TramID);
        }
        public static (int id, string IncDescription, string IncStatus, int TramID) ParseIncidentUpdateMessage(string message)
        {
            string[] parts = message.Split(' ');
            int id = 0;
            string IncDescription = "";
            string IncStatus = "";
            int TramID = 0;

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 4)
                {
                    if (int.TryParse(values[0], out id))
                    {
                        IncDescription = values[1];
                        IncStatus = values[2];
                        TramID = Int32.Parse(values[3]);
                    }
                }
            }

            return (id,IncDescription, IncStatus, TramID);
        }

        public static ( string time, decimal price, int TgId) ParseTicketMessage(string message)
        {
            string[] parts = message.Split(' ');

            string time = "";
            decimal price = 0;
            int TgId = 0;

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 3)
                {
                    if (int.TryParse(values[2], out TgId))
                    {
                        time = values[0];
                        price = Decimal.Parse(values[1]);
                    }
                }
            }

            return (time, price, TgId);
        }

        public static (int id, string desc, int tramId) ParseLogMessage(string message)
        {
            string[] parts = message.Split(' ');

            int id = 0;
            string desc = "";
            int tramId = 0;

            if (parts[0] == "/n" || parts[0] == "/d" || parts[0] == "/u")
            {
                string[] values = parts[1].Split(',');

                if (values.Length == 3)
                {
                    if (int.TryParse(values[0], out id))
                    {
                        desc = values[1];
                        tramId = Int32.Parse(values[2]);
                    }
                }
            }

            return (id, desc, tramId);
        }

    }
}
