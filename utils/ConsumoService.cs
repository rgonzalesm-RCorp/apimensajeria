using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

public class ApiService
{ 
    public async Task<string> HttpUltraMsg(string ruta, Method http, string instanceId, string token, List<ParamsBodyRequest> objBody)
    {
        string path = AppSettings.GetSetting("pathUltramsg");  //"https://api.ultramsg.com/";
        string url = $"{path}{instanceId}/{ruta}";
        var client = new RestClient(url);

        var request = new RestRequest(url, http);

        request.AddHeader("content-type", "application/x-www-form-urlencoded");
        request.AddParameter("token", token);
        foreach (var item in objBody)
        {
            request.AddParameter(item.clave, item.valor);
        }
        RestResponse response = await client.ExecuteAsync(request);
        var output = response.Content ?? string.Empty;
        Console.WriteLine(output);
        return output;
    }
}
