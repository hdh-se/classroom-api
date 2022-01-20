using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCourseAPI.Model.Response;

namespace ManageCourseAPI.WebSocket
{
    public interface IWebSocket
    {
        void Run();
        void Stop();

        void SendComment<T>(int reciver, T response);
        void SendNotification<T>(int reciver, T response);
    }
}