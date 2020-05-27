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
                //Globle.AddDataLog(text);
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                var ipMessage = jsonResult["ipMessage"];
                foreach (JProperty jp in ipMessage)
                {
                    var ip = ("回调") + " : " + jp.Name;
                    var request = jp.Value.ToString();
                    //Globle.AddDataLog(ip);
                    /*if (Globle.RemoteIPMessage.ContainsKey(ip))
                    {
                        Globle.RemoteIPMessage[ip] = request;
                    }
                    else
                    {
                        Globle.globleIPChanged = true;
                        Globle.RemoteIPMessage.Add(ip, request);
                    }*/
                }
            }
            catch (Exception ex)
            {
                var log = "客户连接发生错误 " + ex.Message + ":" + ex.StackTrace;
                Globle.AddDataLog("WSC", log);
            }
        }

    }
}

