using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TwitchLib.Communication.Interfaces;

namespace BudyBot
{
    internal class Commands
    {
        public static string Quote(DatabaseConnector db, string cmdUser, string command, string channel, long timestamp)
        {
            Regex rg = new Regex("@[\\w]{4,25}");
            if (!rg.IsMatch(command))
            {
                Task<(string, long)> msg = db.GetMessage();
                Task.WaitAll(msg);

                var dt = DateTimeOffset.FromUnixTimeMilliseconds(msg.Result.Item2);
                if (msg.Result.Item1 == "")
                    return "No quoted messages found.";
                return $"'{msg.Result.Item1}' - random, {dt:dd-MM-yyyy}";
            }
            string[] split = command.Split(" ", 3);

            // prunes the "@" before the user, and any other additional text
            string quotedUser = split[0].Substring(split[0].Contains("@") ? 1 : 0);
            if (split.Length == 1 || split[1] == string.Empty)
            {
                Task<(string, long)> c = db.GetMessage(quotedUser);
                Task.WaitAll(c);
                if (c.Result.Item1 == "")
                {
                    return $"No quotes found for {quotedUser}";
                }
                return $"'{c.Result.Item1}' - {quotedUser}, {c.Result.Item2:dd-MM-yyyy}";
            }
            else
            {
                Task<(int, int, string, int)> t = db.SelectUser(quotedUser);
                Task.WaitAll(t);
                if (t.Result.Item1 < 1)
                {
                    return $"'{quotedUser}' not found in database.";
                }
                Task.WaitAll(db.AddMessage(quotedUser, split[1], timestamp));
            }

            return "Unable to execute command. Please try again!";
        }

        public static string Slay(DatabaseConnector db)
        {
            // Grab slay count from database
            int count = db.GetCounter(2).Result;
            Task.WaitAll(db.IncrementCounter(2));
            return $"budy has danced well for once! budywuSmile budy has danced well {count} times budywuSlay";

        }

        public static string Score(DatabaseConnector db)
        {
            // Grab 13k count from database
            int count = db.GetCounter(1).Result;
            Task.WaitAll(db.IncrementCounter(1));
            return $"budy got another 13k! budywuSmile Thats {count} so far budywuHot";
        }

        public static string Hair(DatabaseConnector db)
        {
            // This is a fucking stupid command
            // But its on my NightBot and got used like 3 times so whatever
            Task.WaitAll(db.IncrementCounter(3));
            return $"hairography king 🗣🔥🔥🔥🔥🔥🔥";
        }

        public static string Emotes()
        {
            return "catJAM Clap dittoDumper dittoQuake KEKW LETSGO Pog pepeDS peepoSHAKE PeepoS peepoHey OMEGALUL POGGERS POGGING ratJAM";
        }

        public static string JDPlus()
        {
            return "https://justdance.fandom.com/wiki/Just_Dance%2B";
        }

        public static string CurrentChallenge()
        {
            return "No challenge currently!";
        }

        public static string Avatar()
        {
            return "main avatar: Morado uploaded by DeviantDerpy; https://vrchat.com/home/avatar/avtr_a6ea63ec-d39f-46ac-a5de-766e03151543";
        }

        public static string BanList()
        {
            return "https://bit.ly/3fMLae8";
        }
    }
}
