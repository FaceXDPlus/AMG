using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.UI;
using D2LiveManager.Live2DCubism3;
using Newtonsoft.Json.Linq;

namespace D2Live
{
    class D2LiveSocketManager : MonoBehaviour
    {
        private Socket _ServerSocket;                       //服务器监听套接字
        private bool _IsListionContect;                     //是否在监听
        public Text _statusText;
        public Toggle _socketSwitch;
        private Thread _thClientMsg;
        public D2LiveModelController _D2LiveModelController;
        public void StopServer()
        {
            if (_ServerSocket != null)
            {
                _IsListionContect = false;
                _ServerSocket.Shutdown(SocketShutdown.Both);
                _ServerSocket.Close();
                if (_thClientMsg != null)
                {
                    _thClientMsg.Abort();
                }
            }
        }
        public D2LiveSocketManager(Text statusText, Toggle socketSwitch, D2LiveModelController D2LiveModelController)
        {
            _D2LiveModelController = D2LiveModelController;
            _socketSwitch = socketSwitch;
            _statusText = statusText;
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8040);
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _ServerSocket.Bind(endPoint);
                _ServerSocket.Listen(1);
                Debug.Log("启动监听{0}成功" + _ServerSocket.LocalEndPoint.ToString());
                Loom.RunAsync(
                () =>
                {
                    _thClientMsg = new Thread(BuildSever);
                    _IsListionContect = true;
                    _thClientMsg.Start();
                    Debug.Log("服务器已经启动...");
                });
            }
            catch (Exception ex)
            {
                Debug.Log("发生错误" + ex.Message);
                Loom.QueueOnMainThread((param) =>
                {
                    _statusText.text = ex.Message;
                    _socketSwitch.isOn = false;
                }, null);
            }
        }

        private void BuildSever()
        {
            while (_IsListionContect)
            {
                Socket socket = _ServerSocket.Accept();
                Debug.Log("连接到" + socket.RemoteEndPoint);
                ClientMsg(socket);
            }
        }
        /// <summary>
        /// 服务器端和客户端通信的后天线程
        /// </summary>
        /// <param name="?"></param>
        public void ClientMsg(Socket socketMsg)
        {
            while (_IsListionContect)
            {
                Debug.Log("连接到" + socketMsg.RemoteEndPoint);
                Loom.QueueOnMainThread((param) =>
                {
                    _statusText.text = "已连接";
                }, null);
                if (!socketMsg.Connected)
                {
                    Loom.QueueOnMainThread((param) =>
                    {
                        _statusText.text = "等待连接";
                    }, null);
                    socketMsg.Shutdown(SocketShutdown.Both);
                    socketMsg.Close();
                    break;
                }
                try
                {
                    byte[] msyArray = new byte[0124 * 0124];
                    int receiveNumber = socketMsg.Receive(msyArray);
                    Debug.Log("接收客户端" + socketMsg.RemoteEndPoint.ToString() + "消息， 长度为" + receiveNumber);
                    if (receiveNumber == 0)
                    {
                        Loom.QueueOnMainThread((param) =>
                        {
                            _statusText.text = "等待连接";
                        }, null);
                        Debug.Log("接收失败 关闭连接");
                        //socketMsg.Shutdown(SocketShutdown.Both);
                        //socketMsg.Close();
                        break;
                    }
                    string strMsg = Encoding.UTF8.GetString(msyArray, 0, receiveNumber);
                    GetStringJson(strMsg);
                }
                catch (Exception ex)
                {
                    Debug.Log("错误：" + ex.Message);
                    /*Debug.Log("接收失败 关闭连接" + ex.Message);
                    Debug.Log("\n");
                    Loom.QueueOnMainThread((param) =>
                    {
                        _statusText.text = "等待连接";
                    }, null);
                    socketMsg.Shutdown(SocketShutdown.Both);
                    socketMsg.Close();
                    break;*/
                    break;
                }
            }
        }

        public void GetStringJson(string tmpData)
        {
            List<string> outputList = new List<string>();

            int idxStart = tmpData.IndexOf("{");
            int idxEnd = 0;
            while (tmpData.Contains("}"))
            {
                idxEnd = tmpData.IndexOf("}", idxEnd) + 1;
                Console.WriteLine("{}=>" + idxStart.ToString() + "--" + idxEnd.ToString());
                if (idxStart >= idxEnd)
                {
                    continue;// 找下一个 "}"
                }

                var sJSON = tmpData.Substring(idxStart, idxEnd);

                doJsonPrase(sJSON);

                tmpData = tmpData.Substring(idxEnd); //剩余未解析部分
                idxEnd = 0; //复位

                if (tmpData.Contains("{") && tmpData.Contains("}") && (tmpData.Length > 2))
                {
                    GetStringJson(tmpData);
                    break;
                }
            }
        }

        public void doJsonPrase(string input)
        {
            Debug.Log("解析 JSON ...." + input);

            var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
            _D2LiveModelController.paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
            _D2LiveModelController.ParamEyeBallXValue   = float.Parse(jsonResult["eyeX"].ToString());
            _D2LiveModelController.ParamEyeBallYValue   = float.Parse(jsonResult["eyeY"].ToString());
            _D2LiveModelController.paramAngleXValue     = float.Parse(jsonResult["headYaw"].ToString());
            _D2LiveModelController.paramAngleYValue     = float.Parse(jsonResult["headPitch"].ToString());
            _D2LiveModelController.paramAngleZValue     = float.Parse(jsonResult["headRoll"].ToString());
            _D2LiveModelController.ParamBodyAngleXValue = float.Parse(jsonResult["bodyAngleX"].ToString());
            _D2LiveModelController.ParamBodyAngleYValue = float.Parse(jsonResult["bodyAngleY"].ToString());
            _D2LiveModelController.ParamBodyAngleZValue = float.Parse(jsonResult["bodyAngleZ"].ToString());
            _D2LiveModelController.paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
            _D2LiveModelController.paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
            _D2LiveModelController.paramMouthFormValue  = float.Parse(jsonResult["mouthForm"].ToString());
            _D2LiveModelController.paramBrowRYValue     = float.Parse(jsonResult["eyeBrowYR"].ToString());
            _D2LiveModelController.paramBrowLYValue     = float.Parse(jsonResult["eyeBrowYL"].ToString());
            _D2LiveModelController.paramEyeROpenValue   = float.Parse(jsonResult["eyeROpen"].ToString());
            _D2LiveModelController.paramEyeLOpenValue   = float.Parse(jsonResult["eyeLOpen"].ToString());
        }
    }
}