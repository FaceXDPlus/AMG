//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [InitializeOnLoad]
    public static class MaterialUIDataTool
    {
        private const string dataFileName = "MaterialUIData";

        private static string m_MaterialUiPath;
        public static string materialUiPath
        {
            get
            {
                if (m_MaterialUiPath == null)
                {
                    m_MaterialUiPath = GetMaterialUiPath();
                }
                return m_MaterialUiPath;
            }
        }

        static MaterialUIDataTool()
        {
            if (Application.isPlaying) return;

            Initialize();
        }

        private static void Initialize()
        {
            MaterialUiData data = LoadData();

            data.usedVersions = CheckLatestVersionAndSort(data.usedVersions);

            SaveData(data);
        }

        private static List<string> CheckLatestVersionAndSort(List<string> versionList)
        {
			if (versionList.Count == 0 || !versionList.Contains(MaterialUIVersion.currentVersion))
			{
				versionList.Add(MaterialUIVersion.currentVersion);
			}

            versionList.Sort((s, s1) =>
            {
                int[] sNumbers = s.Split('.').Select(a => int.Parse(a)).ToArray();
                int[] s1Numbers = s1.Split('.').Select(a => int.Parse(a)).ToArray();

                for (int i = 0; i < 3; i++)
                {
                    if (sNumbers[i] > s1Numbers[i]) return 1;
                    if (sNumbers[i] < s1Numbers[i]) return -1;
                }

                return 0;
            });

            return versionList;
        }

        public static MaterialUiData LoadData()
        {
            MaterialUiData data = null;

            if (File.Exists(materialUiPath + dataFileName))
            {
                string jsonText = File.ReadAllText(materialUiPath + dataFileName);

                if (!string.IsNullOrEmpty(jsonText))
                {
                    data = JsonUtility.FromJson<MaterialUiData>(jsonText);
                }
            }

            if (data == null)
            {
                data = new MaterialUiData();
            }

            return data;
        }

        public static void SaveData(MaterialUiData data)
        {
            if (data == null)
            {
                return;
            }

			string jsonText = JsonUtility.ToJson(data);;
            File.WriteAllText(materialUiPath + dataFileName, jsonText);
        }

        private static string GetMaterialUiPath()
        {
            string path = "";
            string[] materialUiDirectories = Directory.GetDirectories(Application.dataPath, "MaterialUI");
            for (int i = 0; i < materialUiDirectories.Length; i++)
            {
                if (Directory.Exists(materialUiDirectories[i] + "/Scripts"))
                {
                    path = materialUiDirectories[i].Replace(@"\", "/");
                }
            }

            return path + "/";
        }

        public class MaterialUiData
        {
            [SerializeField]
            private List<string> m_UsedVersions = new List<string>();
            public List<string> usedVersions
            {
                get { return m_UsedVersions; }
                set { m_UsedVersions = value; }
            }
        }
    }
}