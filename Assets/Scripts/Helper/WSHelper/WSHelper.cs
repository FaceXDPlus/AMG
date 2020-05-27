using NetworkSocket.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class WSHelper : MonoBehaviour
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
                listener.UsePlug<WebSocketPlug>();
                listener.Start(8040);
                Globle.AddDataLog("XDP", "服务器已经启动");

            }
            catch (Exception ex)
            {
                SocketSwitch.isOn = false;
                //Globle.AddDataLog("[XDP]捕捉服务器发生错误 " + ex.Message + " : " + ex.StackTrace);
                Globle.AddDataLog("XDP", "捕捉服务器发生错误 " + ex.Message);
            }
        }

        public void SocketStop()
        {
            listener.Dispose();
            Globle.AddDataLog("XDP", "服务器已经关闭");
        }

        public async void P2PClientStart(string uritext)
        {
            try
            {
                Globle.AddDataLog("WSC", "客户连接启动中");
                var uri = new Uri(uritext);
                P2Pclient = new AWebSocketClient(uri);

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
                    Globle.AddDataLog("WSC", "客户连接已经启动");
                }
                else
                {
                    P2PClientSwitch.isOn = false;
                    Globle.AddDataLog("WSC", "客户连接启动失败");
                }
            }
            catch (Exception ex)
            {
                P2PClientSwitch.isOn = false;
                //Globle.AddDataLog("[WSC]客户连接发生错误 " + ex.Message + " : " + ex.StackTrace);
                Globle.AddDataLog("WSC", "客户连接发生错误 " + ex.Message);
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
                Globle.AddDataLog("WSC", "发生错误 " + ex.Message);
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
            Globle.AddDataLog("WSC", "客户连接已经关闭");
            //var RemoteIPMessage = ObjectCopier.Clone(Globle.RemoteIPMessage);
            /*foreach (KeyValuePair<string, string> kvp in RemoteIPMessage)
            {
                if (kvp.Key.IndexOf("回调") > 0)
                {
                    Globle.RemoteIPMessage.Remove(kvp.Key);
                }
            }
            Globle.globleIPChanged = true;*/
        }

    }
}