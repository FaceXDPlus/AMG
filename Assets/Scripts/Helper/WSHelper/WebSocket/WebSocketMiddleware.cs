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
            /*if (Globle.IPMessage.ContainsKey(ip))
            {
                Globle.IPMessage[ip] = text;
            }
            else
            {
                Globle.IPMessage.Add(ip, text);
            }*/
            //var log = string.Format("时间:{0} 用户:{1} 信息:{2}", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString(), text);
            //Debug.Log(log);
            //Globle.DataLog = Globle.DataLog + log;
        }
    }
}