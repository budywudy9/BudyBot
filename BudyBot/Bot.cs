using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Configuration;

namespace BudyBot
{
    class Bot
    {
        TwitchClient client;
        DatabaseConnector db;
        ConfigurationBuilder builder;
        IConfiguration configuration; 

        public Bot()
        {
            builder = new ConfigurationBuilder();
            configuration = builder.AddUserSecrets<Bot>().Build();
            ConnectionCredentials credentials = new ConnectionCredentials(configuration.GetSection("twitch")["username"], configuration.GetSection("twitch")["access_token"]);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "channel");

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();

            client.JoinChannel(configuration.GetSection("twitch")["username"]);

            db = new DatabaseConnector();

        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Connected");
            client.SendMessage(e.Channel, "Connected");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!db.UserExists(e.ChatMessage.Username).Result)
            {
                client.SendMessage(e.ChatMessage.Channel, $"Welcome to the hangout, @{e.ChatMessage.Username}!");
                Task.WaitAll(db.AddUser(e.ChatMessage.Username));
            }
            string msg = e.ChatMessage.Message;

            Task.WaitAll(db.IncrementMessages(e.ChatMessage.Username, Int32.Parse(e.ChatMessage.UserId), e.ChatMessage.Message,Int64.Parse(e.ChatMessage.TmiSentTs)));
            if (e.ChatMessage.Message.Contains("!quote"))
                client.SendMessage(e.ChatMessage.Channel, Commands.Quote(db, e.ChatMessage.Username, e.ChatMessage.Message.Substring(6), e.ChatMessage.Channel, e.ChatMessage.TmiSentTs));
            else if (msg.Contains("!slay"))
                Slay();
            else if (msg.Contains("!13k"))
                Score();
            else if (msg.Contains("!hairflip"))
                Hair();
            if (msg.Contains("hi im denis"))
                client.SendMessage(e.ChatMessage.Channel, "didnt fucking ask");

        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        private void Slay()
        {
            return;
        }

        private void Score()
        {
            return;
        }

        private void Hair()
        {
            return;
        }

        
    }
}
