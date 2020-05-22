using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.Networking;
using NetworkSocket;
using NetworkSocket.Plugs;
using System.Threading.Tasks;
using NetworkSocket.Core;
using NetworkSocket.Fast;
using NetworkSocket.Tasks;
using NetworkSocket.WebSocket;
using System.Linq;
using System.Collections.Generic;
using NetworkSocket.Http;
using Newtonsoft.Json.Linq;

namespace AMG
{
    public class MyWebSocketClient : WebSocketClient
    {
        public MyWebSocketClient(Uri address)
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
                    if (Globle.RemoteIPMessage.ContainsKey(ip))
                    {
                        Globle.RemoteIPMessage[ip] = request;
                    }
                    else
                    {
                        Globle.globleIPChanged = true;
                        Globle.RemoteIPMessage.Add(ip, request);
                    }
                }
            }
            catch (Exception ex)
            {
                var log = "[WSC]客户连接发生错误 " + ex.Message + ":" + ex.StackTrace;
                Globle.AddDataLog(log);
            }
        }

    }

    public class AMGSocketManager : MonoBehaviour
    {
        private Toggle SocketSwitch;
        private Toggle P2PClientSwitch;

        private NetworkSocket.TcpListener listener;
        private WebSocketClient P2Pclient;
        public bool P2PClientStatus = false;


        public void setSocketSwitch(Toggle socketSwitch)
        {
            this.SocketSwitch = socketSwitch;
        }
        
        public void setP2PClientSwitch(Toggle P2PClientSwitch)
        {
            this.P2PClientSwitch = P2PClientSwitch;
        }
        
        public void SocketStart()
        {
            try
            {
                listener = new NetworkSocket.TcpListener();
                listener.Use<WebSocketMiddleware>();
                listener.UsePlug<CustomWSPlug>();
                listener.Start(8040);
                Globle.AddDataLog("[XDP]服务器已经启动");

            }
            catch (Exception ex)
            {
                SocketSwitch.isOn = false;
                //Globle.AddDataLog("[XDP]捕捉服务器发生错误 " + ex.Message + " : " + ex.StackTrace);
                Globle.AddDataLog("[XDP]捕捉服务器发生错误 " + ex.Message);
            }
        }
        
        public void SocketStop()
        {
            listener.Dispose();
            Globle.AddDataLog("[XDP]服务器已经关闭");
        }

        public async void P2PClientStart(string uritext)
        {
            try
            {
                Globle.AddDataLog("[WSC]客户连接启动中");
                var uri = new Uri(uritext);
                P2Pclient = new MyWebSocketClient(uri);

                var addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException(
                        "解析IP失败",
                        ""
                    );
                }
                await P2Pclient.ConnectAsync(addresses[0], uri.Port);
                if (P2Pclient.IsConnected)
                {
                    this.P2PClientStatus = true;
                    Globle.AddDataLog("[WSC]客户连接已经启动");
                }
                else
                {
                    P2PClientSwitch.isOn = false;
                    Globle.AddDataLog("[WSC]客户连接启动失败");
                }
            }
            catch (Exception ex)
            {
                P2PClientSwitch.isOn = false;
                //Globle.AddDataLog("[WSC]客户连接发生错误 " + ex.Message + " : " + ex.StackTrace);
                Globle.AddDataLog("[WSC]客户连接发生错误 " + ex.Message);
            }
        }

        public void P2PClientSendBinary(byte[] binary)
        {
            try
            {
                if (P2Pclient.IsConnected)
                {
                    P2Pclient.SendBinary(binary);
                }
            }
            catch (Exception ex)
            {
                P2PClientSwitch.isOn = false;
                //Globle.AddDataLog("[WSC]发生错误 " + ex.Message + " : " + ex.StackTrace);
                Globle.AddDataLog("[WSC]发生错误 " + ex.Message);
            }
        }

        public void P2PClientStop()
        {
            if (P2Pclient != null)
            {
                this.P2PClientStatus = false;
                P2Pclient.Close();
                P2Pclient.Dispose();
            }
            Globle.AddDataLog("[WSC]客户连接已经关闭");
            var RemoteIPMessage = ObjectCopier.Clone(Globle.RemoteIPMessage);
            foreach (KeyValuePair<string, string> kvp in RemoteIPMessage)
            {
                if (kvp.Key.IndexOf("回调") > 0)
                {
                    Globle.RemoteIPMessage.Remove(kvp.Key);
                }
            }
            Globle.globleIPChanged = true;
        }


    }
}