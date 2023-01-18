using System;
using System.Text.Json;

namespace SainsburysStores
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient HttpClient = new HttpClient();
            using StreamWriter file = new("Stores.txt");
            
            bool end = false;
            int offset = 0;
            int limit = 50;

            while(!end)
            {
                String url = "https://stores.sainsburys.co.uk/api/v1/stores/?store_type=main%2Clocal%2Cpfs&fields=contact%2Etelephone%2Ccontact.address1%2Ccontact.address2%2Ccontact.city%2Ccontact.post_code%2Ccode&offset=" + offset + "&limit=" + limit + "&api_client_id=slfe";
                using (JsonDocument jsonDocument = JsonDocument.Parse(await HttpClient.GetStringAsync(url)))
                {
                    JsonElement root = jsonDocument.RootElement;
                    JsonElement results = root.GetProperty("results");

                    if(results.GetArrayLength() != limit) end = true;

                    foreach(JsonElement store in results.EnumerateArray())
                    {
                        var code = store.GetProperty("code").GetString();
                        var address1 = store.GetProperty("contact").GetProperty("address1").GetString();
                        var address2 = store.GetProperty("contact").GetProperty("address2").GetString();
                        var city = store.GetProperty("contact").GetProperty("city").GetString();
                        var post_code = store.GetProperty("contact").GetProperty("post_code").GetString();
                        var telephone = store.GetProperty("contact").GetProperty("telephone").GetString();

                        await file.WriteLineAsync(code + "; " + address1 + "; " + address2 + "; " + city + "; " + post_code + "; " + telephone);
                    }
                }
                offset += limit;
                Task.Delay(50).Wait();
            }
            
            Console.WriteLine("Stores.txt created");
        }
    }
}
