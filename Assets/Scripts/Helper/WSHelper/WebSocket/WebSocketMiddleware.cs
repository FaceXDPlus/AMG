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
            //var ip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
            var ip = context.Session.RemoteEndPoint.ToString();
            //Debug.Log(text);
            try {
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                var uuid = jsonResult["uuid"].ToString();
                if (Globle.WSClients.ContainsKey(uuid))
                {
                    Globle.WSClients[uuid].message = text;
                    Globle.WSClients[uuid].result = jsonResult;
                    Globle.WSClients[uuid].lastUpdated = DateTime.Now.Date;
                }
                else
                {
                    var WSC = new WSClientClass();
                    WSC.ip = ip;
                    WSC.message = text;
                    WSC.uuid = jsonResult["uuid"].ToString();
                    WSC.result = jsonResult;
                    WSC.lastUpdated = DateTime.Now.Date;
                    WSC.isRemote = false;
                    Globle.WSClients.Add(uuid, WSC);
                    Globle.WSClientsChanged = true;
                }
            }
            catch { }
        }
    }
}