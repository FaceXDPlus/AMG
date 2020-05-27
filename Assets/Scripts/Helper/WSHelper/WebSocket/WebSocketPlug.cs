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
            var log = string.Format("[XDP]时间:{0} 用户:{1} 连接", DateTime.Now.ToString("mm:ss"), context.Session.ToString());
            //Debug.Log(log);
            //Globle.AddDataLog(log);
        }

        protected sealed override void OnDisconnected(object sender, IContenxt context)
        {
            var log = string.Format("[XDP]时间:{0} 用户:{1} 断开连接", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString());
            //Debug.Log(log);
            //Globle.AddDataLog(log);
            //Globle.IPMessage.Remove(((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString());
        }

        protected sealed override void OnException(object sender, Exception exception)
        {
            var log = string.Format("[XDP]时间:{0} 发生错误：{1} {2}", DateTime.Now.ToString("mm:ss"), exception.Message, exception.StackTrace);
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

