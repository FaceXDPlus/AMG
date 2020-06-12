using NetworkSocket.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AMG
{
    public class AWebSocketClient : WebSocketClient
    {
        public AWebSocketClient(Uri address)
            : base(address)
        {
        }

        protected override void OnBinary(FrameRequest frame)
        {
            try
            {
                var text = Encoding.UTF8.GetString(frame.Content);
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                var ipMessage = jsonResult["ipMessage"];
                foreach (JProperty jp in ipMessage)
                {
                    var ip = ("P2P") + " : " + jp.Name;
                    var request = jp.Value.ToString();
                    var jsonResultMessage = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jp.Value.ToString());
                    if (Globle.WSClients.ContainsKey(ip))
                    {
                        Globle.WSClients[ip].message = request;
                        Globle.WSClients[ip].result = jsonResultMessage;
                        Globle.WSClients[ip].lastUpdated = DateTime.Now.Date;
                    }
                    else
                    {
                        var WSC = new WSClientClass();
                        WSC.ip = ip;
                        WSC.message = request;
                        WSC.uuid = jsonResultMessage["uuid"].ToString();
                        WSC.result = jsonResultMessage;
                        WSC.lastUpdated = DateTime.Now.Date;
                        WSC.isRemote = true;
                        Globle.WSClients.Add(ip, WSC);
                        Globle.WSClientsChanged = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientException", ex.Message, ex.StackTrace));
            }
        }

    }
}

