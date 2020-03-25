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
                //_ServerSocket.Shutdown(SocketShutdown.Both)
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
                //定义网络终节点（封装IP和端口）
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8040);
                //实例化套接字
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //服务端绑定地址
                _ServerSocket.Bind(endPoint);
                //开始监听
                _ServerSocket.Listen(1);
                //监听的最大长度
                Debug.Log("启动监听{0}成功" + _ServerSocket.LocalEndPoint.ToString());
                Loom.RunAsync(
                () =>
                {
                    _thClientMsg = new Thread(ClientMsg);
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

        /// <summary>
        /// 服务器端和客户端通信的后天线程
        /// </summary>
        /// <param name="?"></param>
        public void ClientMsg()
        {
            Socket socketMsg = _ServerSocket.Accept();
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
                        socketMsg.Shutdown(SocketShutdown.Both);
                        socketMsg.Close();
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

        public class FaceXDJson
        {
            public float mouthOpenY { get; set; }
            public float eyeROpen { get; set; }
            public float eyeLOpen { get; set; }
            public float eyeX { get; set; }
            public float eyeY { get; set; }
            public float headYaw { get; set; }
            public float headPitch { get; set; }
            public float headRoll { get; set; }
            public float bodyAngleX { get; set; }
            public float bodyAngleY { get; set; }
            public float bodyAngleZ { get; set; }
            public float eyeBrowAngleL { get; set; }
            public float eyeBrowAngleR { get; set; }
            public float mouthForm { get; set; }
            public float eyeBrowYR { get; set; }
            public float eyeBrowYL { get; set; }
            [JsonIgnore]
            public blendShapes blendShapes { get; set; }
        }

        public class blendShapes
        {

        }

        public static T JsonDeSerializerObj<T>(string strJson)
        {
            T t = JsonConvert.DeserializeObject<T>(strJson);
            return t;
        }

        public void doJsonPrase(string input)
        {
            Debug.Log("解析 JSON ...." + input);

            var jsonResult = JsonConvert.DeserializeObject<FaceXDJson>(input);
            _D2LiveModelController.paramMouthOpenYValue = jsonResult.mouthOpenY;
            _D2LiveModelController.ParamEyeBallXValue = jsonResult.eyeX;
            _D2LiveModelController.ParamEyeBallYValue = jsonResult.eyeY;
            _D2LiveModelController.paramAngleXValue = jsonResult.headYaw;
            _D2LiveModelController.paramAngleYValue = jsonResult.headPitch;
            _D2LiveModelController.paramAngleZValue = jsonResult.headRoll;
            _D2LiveModelController.ParamBodyAngleXValue = jsonResult.bodyAngleX;
            _D2LiveModelController.ParamBodyAngleYValue = jsonResult.bodyAngleY;
            _D2LiveModelController.ParamBodyAngleZValue = jsonResult.bodyAngleZ;
            _D2LiveModelController.paramBrowAngleLValue = jsonResult.eyeBrowAngleL;
            _D2LiveModelController.paramBrowAngleRValue = jsonResult.eyeBrowAngleR;
            _D2LiveModelController.paramMouthFormValue = jsonResult.mouthForm;
            _D2LiveModelController.paramBrowRYValue = jsonResult.eyeBrowYR;
            _D2LiveModelController.paramBrowLYValue = jsonResult.eyeBrowYL;
            _D2LiveModelController.paramEyeROpenValue = jsonResult.eyeROpen;
            _D2LiveModelController.paramEyeLOpenValue = jsonResult.eyeLOpen;
        }
    }
}