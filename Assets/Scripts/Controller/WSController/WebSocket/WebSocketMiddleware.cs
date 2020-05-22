using System;
using System.Text;
using NetworkSocket;
using NetworkSocket.Plugs;
using NetworkSocket.WebSocket;

namespace AMG
{
    public class WebSocketMiddleware : WebSocketMiddlewareBase
    {
        protected sealed override void OnBinary(IContenxt context, FrameRequest frame)
        {
            var text = Encoding.UTF8.GetString(frame.Content);
            var ip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
            if (Globle.IPMessage.ContainsKey(ip))
            {
                Globle.IPMessage[ip] = text;
            }
            else
            {
                Globle.IPMessage.Add(ip, text);
            }
            //var log = string.Format("时间:{0} 用户:{1} 信息:{2}", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString(), text);
            //Debug.Log(log);
            //Globle.DataLog = Globle.DataLog + log;
        }
    }

    public class CustomWSPlug : PlugBase
    {
        protected sealed override void OnConnected(object sender, IContenxt context)
        {
            var log = string.Format("[XDP]时间:{0} 用户:{1} 连接", DateTime.Now.ToString("mm:ss"), context.Session.ToString());
            //Debug.Log(log);
            Globle.AddDataLog(log);
        }

        protected sealed override void OnDisconnected(object sender, IContenxt context)
        {
            var log = string.Format("[XDP]时间:{0} 用户:{1} 断开连接", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString());
            //Debug.Log(log);
            Globle.AddDataLog(log);
            Globle.IPMessage.Remove(((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString());
        }

        protected sealed override void OnException(object sender, Exception exception)
        {
            var log = string.Format("[XDP]时间:{0} 发生错误：{1} {2}", DateTime.Now.ToString("mm:ss"), exception.Message, exception.StackTrace);
            //Debug.Log(log);
            Globle.AddDataLog(log);
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