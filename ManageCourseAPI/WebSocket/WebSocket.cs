using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ManageCourseAPI.WebSocket
{
    public class WebSocket:IWebSocket
    {
        private WebSocketServer wsksv;

        public void Run()
        {
            this.wsksv = new WebSocketServer("ws://localhost");
            wsksv.AddWebSocketService<MessagesService>("/Messages");
            wsksv.AddWebSocketService<NotificationsService>("/Notifications");
            wsksv.Start();
        }

        public void Stop()
        {
            wsksv.Stop();
        }

        public void SendComment<T>(int reciver, T response)
        {
            MessagesService.AddComment(reciver, response);
        }

        public void SendNotification<T>(int reciver, T response)
        {
            NotificationsService.SendNotification(reciver, response);
        }
    }
}