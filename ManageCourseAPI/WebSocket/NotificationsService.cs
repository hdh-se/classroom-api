using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCourse.Core.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace ManageCourseAPI.WebSocket
{
    public class NotificationsService : WebSocketBehavior
    {
        private static Dictionary<int, NotificationsService> Connections =
            new Dictionary<int, NotificationsService>();

        public int id;

        protected override void OnMessage(MessageEventArgs e)
        {
            Message m = JsonConvert.DeserializeObject<Message>(e.Data);
            if (m.channel == "JOIN")
            {
                this.id = m.sender;
                lock (Connections)
                {
                    Connections[id] = this;
                }

                Send(new Message()
                {
                    channel = "CONNECTED",

                    data = "SUCCESS"
                }.SerializeObject());
            }
            else
            {
                lock (Connections)
                {
                    if (Connections.ContainsKey(m.receiver))
                    {
                        Connections[m.receiver].Send(e.Data);
                        Send(e.Data);
                    }
                    else
                    {
                        Send(new Message()
                        {
                            channel = "ERROR",
                            data = "Người dùng đã offline"
                        }.SerializeObject());
                    }
                }
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            lock (Connections)
            {
                Connections.Remove(id);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine("Error " + e.Message);
            lock (Connections)
            {
                Connections.Remove(id);
            }
        }

        public static void SendNotification<T>(int reciver, T response)
        {
            lock (Connections)
            {
                if (Connections.ContainsKey(reciver))
                {
                    Connections[reciver].Send(new Message()
                    {
                        channel = "NOTIFICATION",
                        data = response,
                        receiver = reciver,
                    }.SerializeObject());
                }
            }
        }

        public static void SendNotification(ICollection<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                NotificationsService.SendNotification(notification.UserId, notification);
            }
        }

        public static void SendNotification(Notification notification)
        {
            NotificationsService.SendNotification(notification.UserId, notification);
        }
    }
}