using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class WSHelper : MonoBehaviour
    {
        [SerializeField] private Toggle SocketSwitch;
        [SerializeField] private Toggle P2PClientSwitch;
        [SerializeField] private Toggle USBClientSwitch;
        [SerializeField] private Toggle DanmakuClientSwitch;

        private NetworkSocket.TcpListener listener;
        private WebSocketClient P2Pclient;
        private WebSocketClient USBclient;
        private WebSocketClient Danmakuclient;
        public bool P2PClientStatus = false;
        public bool USBClientStatus = false;
        public bool DanmakuClientStatus = false;

        public void SocketStart()
        {
            try
            {
                listener = new NetworkSocket.TcpListener();
                listener.Use<WebSocketMiddleware>();
                listener.UsePlug<WebSocketPlug>();
                listener.Start(8040);
                Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPServerStarted"));

            }
            catch (Exception ex)
            {
                SocketSwitch.isOn = false;
                Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPServerException", ex.Message, ex.StackTrace));
            }
        }

        public void SocketStop()
        {
            listener.Dispose();
            Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPServerStopped"));
        }

        public class ArrayInfo
        {
            public string hostName { get; set; }
            public ArrayList keyboardAttached { get; set; }
            public Dictionary<string, string> ipMessage { get; set; }
        }

        private void FixedUpdate()
        {
            if (P2PClientStatus == true && P2PClientSwitch.isOn == true)
            {
                var ipMessage = new Dictionary<string, string>();
                foreach (KeyValuePair<string, WSClientClass> kvp in Globle.WSClients)
                {
                    if (!ipMessage.ContainsKey(kvp.Key) && kvp.Value.isRemote == false)
                    {
                        ipMessage.Add(kvp.Key, kvp.Value.message);
                    }
                }
                ArrayInfo arrayInfo = new ArrayInfo
                {
                    hostName = Globle.GetComputerName(),
                    keyboardAttached = new ArrayList(),
                    ipMessage = ipMessage
                };
                byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(arrayInfo));
                P2PClientSendBinary(byteArray);
            }
        }

        public async void P2PClientStart(string uritext)
        {
            try
            {
                Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientStarting"));
                var uri = new Uri(uritext);
                P2Pclient = new AWebSocketClient(uri);

                var addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException(
                        Globle.LangController.GetLang("LOG.WSCClientIPFailed"),
                        ""
                    );
                }
                await P2Pclient.ConnectAsync(addresses[0], uri.Port);
                if (P2Pclient.IsConnected)
                {
                    P2PClientStatus = true;
                    Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientStarted"));
                }
                else
                {
                    P2PClientSwitch.isOn = false;
                    Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientStartedFailed"));
                }
            }
            catch (Exception ex)
            {
                P2PClientSwitch.isOn = false;
                Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientException", ex.Message));
            }
        }

        public void P2PClientSendBinary(byte[] binary)
        {
            try
            {
                if (P2Pclient.IsConnected && P2PClientStatus == true)
                {
                    P2Pclient.SendBinary(binary);
                }
            }
            catch (Exception ex)
            {
                P2PClientSwitch.isOn = false;
                P2PClientStatus = false;
                Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientException", ex.Message));
            }
        }

        public void P2PClientStop()
        {
            if (P2Pclient != null)
            {
                P2PClientStatus = false;
                P2Pclient.Close();
                P2Pclient.Dispose();
            }
            Globle.AddDataLog("WSC", Globle.LangController.GetLang("LOG.WSCClientStopped"));
            var readyToRemove = new ArrayList();
            foreach (KeyValuePair<string, WSClientClass> kvp in Globle.WSClients)
            {
                if (kvp.Value.isRemote == true)
                {
                    readyToRemove.Add(kvp.Key);
                }
            }
            foreach (string aa in readyToRemove)
            {
                Globle.WSClients.Remove(aa);
            }
            Globle.WSClientsChanged = true;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        public async void DanmakuClientStart()
        {
            try
            {
                Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.DanmakuClientStarting"));

                var uri = new Uri("wss://danmaku.loli.ren/chat");
                Danmakuclient = new DanmakuWebsocketClient(uri, CheckValidationResult);

                var addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException(
                        Globle.LangController.GetLang("LOG.DanmakuClientIPFailed"),
                        ""
                    );
                }
                await Danmakuclient.ConnectAsync(addresses[0], uri.Port);
                if (Danmakuclient.IsConnected)
                {
                    DanmakuClientStatus = true;
                    Globle.AddDataLog("Danmaku", Globle.LangController.GetLang("LOG.USBClientStarted"));
                    Danmakuclient.SendText("{\"cmd\":1,\"data\":{ \"roomId\":35119946,\"version\":\"0.2.2\", \"config\":{ \"autoTranslate\":false}}}");
                }
                else
                {
                    DanmakuClientSwitch.isOn = false;
                    Globle.AddDataLog("Danmaku", Globle.LangController.GetLang("LOG.DanmakuClientStartedFailed"));
                }
            }
            catch (Exception ex)
            {
                DanmakuClientSwitch.isOn = false;
                Globle.AddDataLog("Danmaku", Globle.LangController.GetLang("LOG.DanmakuClientException", ex.Message));
            }
        }

        public void DanmakuClientStop()
        {
            if (Danmakuclient != null)
            {
                DanmakuClientStatus = false;
                Danmakuclient.Close();
                Danmakuclient.Dispose();
            }
            Globle.AddDataLog("Danmaku", Globle.LangController.GetLang("LOG.DanmakuClientStopped"));
        }

        public async void USBClientStart()
        {
            try
            {
                Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientStarting"));

                var uri = new Uri("ws://127.0.0.1:22546");
                USBclient = new USBWebSocketClient(uri);

                var addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException(
                        Globle.LangController.GetLang("LOG.USBClientIPFailed"),
                        ""
                    );
                }
                await USBclient.ConnectAsync(addresses[0], uri.Port);
                if (USBclient.IsConnected)
                {
                    USBClientStatus = true;
                    Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientStarted"));
                }
                else
                {
                    USBClientSwitch.isOn = false;
                    Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientStartedFailed"));
                }
            }
            catch (Exception ex)
            {
                USBClientSwitch.isOn = false;
                Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientException", ex.Message));
            }
        }

        public void USBClientStop()
        {
            if (USBclient != null)
            {
                USBClientStatus = false;
                USBclient.Close();
                USBclient.Dispose();
            }
            Globle.AddDataLog("USB", Globle.LangController.GetLang("LOG.USBClientStopped"));
            var readyToRemove = new ArrayList();
            foreach (KeyValuePair<string, WSClientClass> kvp in Globle.WSClients)
            {
                if (kvp.Value.isUSB == true)
                {
                    readyToRemove.Add(kvp.Key);
                }
            }
            foreach (string aa in readyToRemove)
            {
                Globle.WSClients.Remove(aa);
            }
            Globle.WSClientsChanged = true;
        }


    }
}