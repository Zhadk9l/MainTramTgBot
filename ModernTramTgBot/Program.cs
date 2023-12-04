using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Timers;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace ModernTramTgBot
{
    class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                TelegramBotHelper hlp = new TelegramBotHelper(token: "6476978943:AAHdmXqui8pdEXAMkOeWdYLhk7CZBTXTmOE");
                hlp.GetUpdates();
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}      
