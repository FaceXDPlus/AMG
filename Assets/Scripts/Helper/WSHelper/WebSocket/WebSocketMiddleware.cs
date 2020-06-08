using System;
using System.Text;
using NetworkSocket;
using NetworkSocket.Plugs;
using NetworkSocket.WebSocket;
using UnityEngine;

namespace AMG
{
    public class WebSocketMiddleware : WebSocketMiddlewareBase
    {
        protected sealed override void OnBinary(IContenxt context, FrameRequest frame)
        {
            var text = Encoding.UTF8.GetString(frame.Content);
            var ip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
            if (Globle.WSClients.ContainsKey(ip))
            {
                Globle.WSClients[ip].message = text;
                Globle.WSClients[ip].lastUpdated = DateTime.Now.Date;
            }
            else
            {
                var WSC = new WSClientClass();
                WSC.ip = ip;
                WSC.message = text;
                WSC.lastUpdated = DateTime.Now.Date;
                WSC.isRemote = true;
                Globle.WSClients.Add(ip, WSC);
                Globle.WSClientsChanged = true;
            }
        }
    }
}