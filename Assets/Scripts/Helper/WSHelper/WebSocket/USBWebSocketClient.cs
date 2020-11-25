using NetworkSocket.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using UnityEngine;

namespace AMG
{
    public class USBWebSocketClient : WebSocketClient
    {
        public USBWebSocketClient(Uri address)
            : base(address)
        {
        }

        protected override void OnBinary(FrameRequest frame)
        {
            try
            {
                var text = Encoding.UTF8.GetString(frame.Content);
                //var ip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
                //var ip = context.Session.RemoteEndPoint.ToString();
                try
                {
                    var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                    var uuid = "USB: " + jsonResult["uuid"].ToString();
                    if (Globle.WSClients.ContainsKey(uuid))
                    {
                        Globle.WSClients[uuid].message = text;
                        Globle.WSClients[uuid].result = jsonResult;
                        Globle.WSClients[uuid].lastUpdated = DateTime.Now.Date;
                    }
                    else
                    {
                        var WSC = new WSClientClass();
                        WSC.ip = "USB";
                        WSC.message = text;
                        WSC.uuid = jsonResult["uuid"].ToString();
                        WSC.result = jsonResult;
                        WSC.lastUpdated = DateTime.Now.Date;
                        WSC.isRemote = false;
                        WSC.isUSB = true;
                        Globle.WSClients.Add(uuid, WSC);
                        Globle.WSClientsChanged = true;
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientException", ex.Message, ex.StackTrace));
            }
        }

    }
}
