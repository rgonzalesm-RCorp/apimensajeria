using System;
using System.Threading.Tasks;
using RestSharp;

public class WhatsAppApi
{
    public static async Task SendMessage(string Body)
    {
        string instance;
        string token;
        instance = AppSettings.GetSetting("WhatsApp:instance");
        token = AppSettings.GetSetting("WhatsApp:token");

        var url = $"https://api.ultramsg.com/{instance}/messages/chat";
        var client = new RestClient(url);

        var request = new RestRequest(url, Method.Post);
        request.AddHeader("content-type", "application/x-www-form-urlencoded");
        request.AddParameter("token", token);
        request.AddParameter("to", "+59170972055");
        request.AddParameter("body", Body);


        RestResponse response = await client.ExecuteAsync(request);
        var output = response.Content;
        Console.WriteLine(output);
    }
}