using NetworkSocket;
using NetworkSocket.Plugs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class WebSocketPlug : PlugBase
    {
        protected sealed override void OnConnected(object sender, IContenxt context)
        {
            Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPClientConnect", DateTime.Now.ToString("mm:ss"), context.Session.ToString()));
        }

        protected sealed override void OnDisconnected(object sender, IContenxt context)
        {
            //var ip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
            var ip = context.Session.RemoteEndPoint.ToString();
            foreach(var wsclient in Globle.WSClients)
            {
                if (wsclient.Value.ip == ip)
                {
                    Globle.WSClients.Remove(wsclient.Key);
                    break;
                }
            }
            Globle.WSClientsChanged = true;
            Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPClientDisconnect", DateTime.Now.ToString("mm:ss"), context.Session.ToString()));
        }

        protected sealed override void OnException(object sender, Exception exception)
        {
            //var log = string.Format("[XDP]时间:{0} 发生错误：{1} {2}", DateTime.Now.ToString("mm:ss"), exception.Message, exception.StackTrace);
            //Debug.Log(log);
            //Globle.AddDataLog(log);
            //Globle.IPMessage.Remove(context.Session.RemoteEndPoint.ToString());
        }

        /*protected sealed override void OnRequested(object sender, IContenxt context)
        {

            var request = context.StreamReader.ReadString(Encoding.ASCII).Replace("\n", "");
            var log = string.Format("[XDP]时间:{0} 用户:{1} 信息:{2}", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString(), request);
            //Debug.Log(log);
            Globle.AddDataLog(log);
        }*/
    }
}

