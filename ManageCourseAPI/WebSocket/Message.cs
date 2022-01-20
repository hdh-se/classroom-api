using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ManageCourseAPI.WebSocket
{
    [Serializable]
    public class Message
    {
        public int sender;
        public dynamic data;
        public int receiver;
        public string channel;
    }

    public static class ExtensionMessage
    {
        public static IContractResolver Resolver = new CamelCasePropertyNamesContractResolver();

        public static string SerializeObject(this Message message)
        {
            return JsonConvert.SerializeObject(message, new JsonSerializerSettings()
            {
                ContractResolver = Resolver
            });
        }
    }
}