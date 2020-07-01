using Live2D.Cubism.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class Globle
    {
        public static string APPName = "AMG-RM";
        public static string APPVersion = "Alpha 0.2";
        public static string APPBuild = "9";

        public static int ModelNum = 1;

        public static ArrayList ModelList = new ArrayList();

        //uuid, WSClient
        public static Dictionary<string, WSClientClass> WSClients = new Dictionary<string, WSClientClass>();

        public static bool WSClientsChanged = false;

        public static string DataLog = "";
        public static LangController LangController = null;

        public static void AddDataLog(string from,string data)
        {
            DataLog = DataLog + "\n[" + from + "] " + data;
        }

        public static string GetComputerName()
        {
            return Environment.GetEnvironmentVariable("computername");
        }
    }

    [Serializable]
    public class WSClientClass
    {
        public string ip = "";
        public string message = "";
        public string uuid = "";
        public DateTime lastUpdated;
        public bool isRemote = false;
        public bool isUSB = false;

        public JObject result;
    }

}
