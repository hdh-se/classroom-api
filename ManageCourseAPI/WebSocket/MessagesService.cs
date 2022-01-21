using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ManageCourseAPI.WebSocket
{
    public class MessagesService : WebSocketBehavior
    {
        public int id;
        private static Dictionary<int, MessagesService> Connections = new Dictionary<int, MessagesService>();

        protected override void OnMessage(MessageEventArgs e)
        {
            Message m = JsonConvert.DeserializeObject<Message>(e.Data);
            if (m.channel == "JOIN")
            {
                lock (Connections)
                {
                    Connections[m.sender] = this;
                }

                Send(new Message()
                {
                    channel = "SUCCESS",
                    data = "CONNECTED"
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

        public static void AddComment<T>(int receiver, T response)
        {
            SendResponse(receiver, response, "ADD_COMMENT");
        }

        public static void UpdateComment<T>(int receiver, T response)
        {
            SendResponse(receiver, response, "UPDATE_COMMENT");
        }


        public static void DeleteComment<T>(int receiver, T response)
        {
            SendResponse(receiver, response, "DELETE_COMMENT");
        }

        private static void SendResponse<T>(int receiver, T response, string channel)
        {
            try
            {
                lock (Connections)
                {
                    if (Connections.ContainsKey(receiver))
                    {
                        Connections[receiver].Send((new Message()
                        {
                            channel = channel,
                            data = response,
                            receiver = receiver,
                        }.SerializeObject()));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SendApproval<T>(int receiver, T res)
        {
            SendResponse(receiver, res, "APPROVAL");
        }
    }
}