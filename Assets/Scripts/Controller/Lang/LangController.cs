using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AMG
{
    public class LangController : MonoBehaviour
    {
        public string LangName = "zh-CN";
        public bool InitStatus = false;
        private JObject JLangInfo = null;

        public void InitLangFromJson()
        {
            var savePath = Application.streamingAssetsPath + "/lang/" + LangName + ".json";
            if (System.IO.File.Exists(savePath))
            {
                StreamReader sr = new StreamReader(savePath, Encoding.Default);
                String line = sr.ReadToEnd();
                sr.Close();
                var LangInfo = JsonConvert.DeserializeObject(line);
                JLangInfo = LangInfo as Newtonsoft.Json.Linq.JObject;
                Globle.AddDataLog("Lang", JLangInfo["LangInitSuccess"].ToString());
                InitStatus = true;
            }
            else
            {
                Globle.AddDataLog("Lang", "Error: Language File Not Found");
            }
        }

        public string GetLang(string text)
        {
            try
            {
                return JLangInfo[text].ToString();
            }
            catch
            {
                return "Unisset Lang: " + text;
            }
        }
    }
}