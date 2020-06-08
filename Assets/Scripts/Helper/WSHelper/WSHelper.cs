using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private NetworkSocket.TcpListener listener;
        private WebSocketClient P2Pclient;
        public bool P2PClientStatus = false;

        public void SocketStart()
        {
            try
            {
                listener = new NetworkSocket.TcpListener();
                listener.Use<WebSocketMiddleware>();
                listener.UsePlug<WebSocketPlug>();
                listener.Start(8041);
                Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPServerStart"));

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
            Globle.AddDataLog("XDP", Globle.LangController.GetLang("LOG.XDPServerStop"));
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
                var WSClients = ObjectCopier.Clone(Globle.WSClients);
                var ipMessage = new Dictionary<string, string>();
                foreach (KeyValuePair<string, WSClientClass> kvp in WSClients)
                {
                    if (!ipMessage.ContainsKey(kvp.Value.ip) && kvp.Value.isRemote == false)
                    {
                        ipMessage.Add(kvp.Value.ip, kvp.Value.message);
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
            var RemoteIPMessage = ObjectCopier.Clone(Globle.WSClients);
            foreach (KeyValuePair<string, WSClientClass> kvp in RemoteIPMessage)
            {
                if (kvp.Value.isRemote == true)
                {
                    Globle.WSClients.Remove(kvp.Key);
                }
            }
            Globle.WSClientsChanged = true;
        }

    }
}