using System;
using System.Collections.Generic;
using System.Threading;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.EventSub.Websockets.Client;

namespace BudyBot
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            DatabaseConnector db = new DatabaseConnector();

            Console.ReadLine();
           
        }

    }

    
}
