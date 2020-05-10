﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static AMG.AMGUtils;

namespace AMG
{
    public class AMGSaveController : MonoBehaviour
    {
        public SaveArrayInfo LoadUserData(string path)
        {
            try
            {
                var savePath = path + "AMGSettings.json";

                StreamReader sr = new StreamReader(savePath, Encoding.Default);
                String line = sr.ReadToEnd();
                SaveArrayInfo arrayInfo = JsonConvert.DeserializeObject<SaveArrayInfo>(line);
                Globle.AddDataLog("[Main]文件读取成功");
                return arrayInfo;
            }
            catch (Exception err)
            {
                Globle.AddDataLog("[Main]读取文件时发生错误：" + err.Message);
                return null;
            }
        }

        public void SaveUserData(string path, Dictionary<string, string> userdict, Dictionary<string, Dictionary<string, string>> shortcutPair)
        {
            try
            {
                SaveArrayInfo arrayInfo = new SaveArrayInfo
                {
                    APPVersion = Globle.APPVersion,
                    ModelDict = userdict,
                    ShortcutPair = shortcutPair
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;
                var indentFile = JsonConvert.SerializeObject(arrayInfo, settings);
                var savePath = path + "AMGSettings.json";
                Globle.AddDataLog("[Main]文件保存到：" + savePath);
                if (!File.Exists(savePath))
                {
                    System.IO.File.Create(savePath).Dispose(); ;
                }
                StreamWriter sw = new StreamWriter(savePath);
                sw.WriteLine(indentFile); 
                sw.Close();
            }
            catch (Exception err)
            {
                Globle.AddDataLog("[Main]保存文件时发生错误：" + err.Message);
            }
        }
    }
}