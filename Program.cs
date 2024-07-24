using System;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Newtonsoft.Json;
using Telegram.Bot.Requests;

public class EvoFarmNotificator
{
    private static string filePath = Environment.CurrentDirectory + "/farm.txt";
    private static string lastContent = "";

    private static string tgbot_id = "";
    private static string user_id = "";

    private static TelegramBotClient tgbot;

    public static void Main()
    {
        if (!LoadCfg())
        {
            Console.Write("The configuration file could not be found. Insert Telegram bot ID here: ");
            tgbot_id = Console.ReadLine();
            Console.Write("Great! Now paste your User ID: ");
            user_id = Console.ReadLine();
            SaveCfg();
        }

        tgbot = new TelegramBotClient(tgbot_id);

        Timer timer = new Timer(CheckFile, null, 0, 10000);

        Console.WriteLine("Notificator started.");
        Console.WriteLine("Commands: exit, test_mes");

        while (true)
        {
            string input = Console.ReadLine();//.Trim();

            if (input == "exit")
                Environment.Exit(0);
            else if (input == "test_mes")
                tgbot.SendTextMessageAsync(user_id, "Test message.");
            else
                Console.WriteLine("Unknown command.");
        }
    }

    private static void CheckFile(object state)
    {
        if (File.Exists(filePath))
        {
            string currentContent = File.ReadAllText(filePath);
            if (currentContent != lastContent)
            {
                Console.WriteLine("New content detected in the file. Sent to Telegram.");
                tgbot.SendTextMessageAsync(user_id, $"New content detected in the file:\n \n{currentContent.Substring(lastContent.Length)}");
                lastContent = currentContent;
            }
        }
    }

    private static bool LoadCfg()
    {
        var path = Environment.CurrentDirectory + "/botcfg.json";
        if (File.Exists(path))
        {
            string[] str = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(path));
            tgbot_id = str[0];
            user_id = str[1];
            return true;
        }
        return false;
    }

    private static void SaveCfg()
    {
        string[] stringArray = { null, null };
        stringArray[0] = tgbot_id;
        stringArray[1] = user_id;
        using (StreamWriter file = File.CreateText(Environment.CurrentDirectory + "/botcfg.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, stringArray);
        }
    }
}