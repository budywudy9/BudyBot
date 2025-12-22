using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TwitchLib.Communication.Interfaces;

namespace BudyBot
{
    internal class Commands
    {
        public static string Quote(DatabaseConnector db, string cmdUser, string command, string channel, string timestamp)
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
    }
}
