using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AMG
{
    public class SaveController : MonoBehaviour
    {
        public SaveArrayInfo LoadUserData(string path)
        {
            try
            {
                var savePath = path + "/AMGSettings.json";
                if (System.IO.File.Exists(savePath))
                {
                    StreamReader sr = new StreamReader(savePath, Encoding.Default);
                    String line = sr.ReadToEnd();
                    sr.Close();
                    SaveArrayInfo arrayInfo = JsonConvert.DeserializeObject<SaveArrayInfo>(line);
                    Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigLoadSuccess"));
                    return arrayInfo;
                }
                else
                {
                    Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigUnisset"));
                    return null;
                }
            }
            catch (Exception err)
            {
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigLoadException", err.Message));
                return null;
            }
        }

        public void SaveUserData(string path, Dictionary<string, string> userdict, Dictionary<string, string> otherdict, Dictionary<string, string> locationdict, Dictionary<string, Dictionary<string, string>> shortcutPair)
        {
            try
            {
                SaveArrayInfo arrayInfo = new SaveArrayInfo
                {
                    APPVersion = Globle.APPVersion,
                    ModelAlign = userdict,
                    ModelOtherSettings = otherdict,
                    ModelLocationSettings = locationdict,
                    ShortcutPair = shortcutPair
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;
                var indentFile = JsonConvert.SerializeObject(arrayInfo, settings);
                var savePath = path + "/AMGSettings.json";
                if (!File.Exists(savePath))
                {
                    System.IO.File.Create(savePath).Dispose();
                }
                StreamWriter sw = new StreamWriter(savePath);
                sw.WriteLine(indentFile);
                sw.Close();
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigSaveSuccess", savePath));
            }
            catch (Exception err)
            {
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigSaveException", err.Message));
            }
        }

        public class SaveArrayInfo
        {
            public string APPVersion { get; set; }
            public Dictionary<string, string> ModelAlign { get; set; }
            public Dictionary<string, string> ModelOtherSettings { get; set; }
            public Dictionary<string, string> ModelLocationSettings { get; set; }
            public Dictionary<string, Dictionary<string, string>> ShortcutPair { get; set; }
        }
    }
}