using MaterialUI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.Motion;

namespace AMG
{
    public class AMGController : MonoBehaviour
    {
        private SelectionBoxConfig ModelDropdownBox;
        private SelectionBoxConfig ControlModelDropdownBox;
        private SelectionBoxConfig ControlIPDropdownBox;
        private DialogBoxConfig DialogBox;
        private Text DialogBoxTitle;
        private Text DialogBoxText;


        public void setModelDropdownBox(SelectionBoxConfig ModelDropdownBox)
        {
            this.ModelDropdownBox = ModelDropdownBox;
        }

        public void setControlModelDropdownBox(SelectionBoxConfig ControlModelDropdownBox)
        {
            this.ControlModelDropdownBox = ControlModelDropdownBox;
        }

        public void setControlIPDropdownBox(SelectionBoxConfig ControlIPDropdownBox)
        {
            this.ControlIPDropdownBox = ControlIPDropdownBox;
        }
        
        public void setDialogBoxConfig(DialogBoxConfig DialogBox, Text DialogBoxTitle, Text DialogBoxText)
        {
            this.DialogBox      = DialogBox;
            this.DialogBoxTitle = DialogBoxTitle;
            this.DialogBoxText  = DialogBoxText;
        }

        public void InitDropdownBoxs()
        {
            this.ControlModelDropdownBox.listItems = new string[1];
            this.ControlModelDropdownBox.listItems[0] = "无";
            this.ControlModelDropdownBox.RefreshList();

            this.ControlIPDropdownBox.listItems = new string[1];
            this.ControlIPDropdownBox.listItems[0] = "无";
            this.ControlIPDropdownBox.RefreshList();
        }
        
        public void RefreshModelDropdown()
        {
            var returnArray = GetModelsFromAssets();
            var returnCount = returnArray.Count;
            if (returnCount > 0)
            {
                ModelDropdownBox.listItems = new string[returnCount];
                int i = 0;
                while (i < returnCount)
                {
                    ModelDropdownBox.listItems[i] = returnArray[i].ToString();
                    i++;
                }
                ModelDropdownBox.RefreshList();
                ModelDropdownBox.selectedText.text = "请选择模型";
            }
            else
            {
                ModelDropdownBox.selectedText.text = "未找到模型";
            }
        }

        public void RefreshControlDropdown(ArrayList list)
        {
            var listCount = list.Count;
            ControlModelDropdownBox.listItems = new string[listCount];
            int i = 0;
            while (i < listCount)
            {
                var model = (CubismModel)list[i];
                ControlModelDropdownBox.listItems[i] = model.name;
                i++;
            }
            ControlModelDropdownBox.RefreshList();
            ControlModelDropdownBox.currentSelection = -1;
            ControlModelDropdownBox.selectedText.text = "选择控制器";
        }

        public void RefreshControlIPDropdown()
        {
            var IPMessage = Globle.IPMessage;
            var RemoteIPMessage = Globle.RemoteIPMessage;
            var listCount = IPMessage.Count;
            var relistCount = RemoteIPMessage.Count;
            if (listCount == 0 && relistCount == 0)
            {
                ControlIPDropdownBox.selectedText.text = "选择控制IP";
                ControlIPDropdownBox.listItems = new string[1];
                ControlIPDropdownBox.listItems[0] = "无";
                ControlIPDropdownBox.RefreshList();
            }
            else
            {
                int i = 0;
                ControlIPDropdownBox.listItems = new string[listCount + relistCount];
                foreach (KeyValuePair<string, string> kvp in IPMessage)
                {
                    ControlIPDropdownBox.listItems[i] = kvp.Key;
                    i++;
                }
                foreach (KeyValuePair<string, string> kvp in RemoteIPMessage)
                {
                    ControlIPDropdownBox.listItems[i] = kvp.Key;
                    i++;
                }
                ControlIPDropdownBox.RefreshList();
                ControlIPDropdownBox.currentSelection = -1;
                ControlIPDropdownBox.selectedText.text = "选择控制IP";
            }
        }

        public CubismModel AddModelFromDropdown()
        {
            try
            {
                string streamingAssetsPath = Application.streamingAssetsPath + "/models/" + ModelDropdownBox.selectedText.text + "/" + ModelDropdownBox.selectedText.text + ".model3.json";
                Debug.Log(streamingAssetsPath);
                if (System.IO.File.Exists(streamingAssetsPath))
                {
                    Debug.Log("Start loading Live2D");
                    var ModelFullName = streamingAssetsPath;
                    var model3Json = CubismModel3Json.LoadAtPath(ModelFullName, BuiltinLoadAssetAtPath);
                    var motions = model3Json.FileReferences.Motions.Motions;
                    var model = model3Json.ToModel();
                    CubismRenderController cubisumRenderController = model.GetComponent<CubismRenderController>();
                    cubisumRenderController.SortingMode = CubismSortingMode.BackToFrontOrder;

                    /*var fadeMotions = ScriptableObject.CreateInstance<CubismFadeMotionList>();

                    var k = 0;
                    var motionDict = new Dictionary<int, CubismFadeMotionData>();
                    for (var i = 0; i < motions.Length; ++i)
                    {
                        for (var j = 0; j < motions[i].Length; ++j)
                        {
                            string streamingAssetsMotionPath = Application.streamingAssetsPath + "/models/" + ModelDropdownBox.selectedText.text + "/" + motions[i][j].File;
                            CubismMotion3Json cubismMotion3Json = CubismMotion3Json.LoadFrom(File.ReadAllText(streamingAssetsMotionPath));
                            AnimationClip animationClip = cubismMotion3Json.ToAnimationClip(new AnimationClip{
                                legacy = true
                            }, false, false, false, null);
                            animationClip.name = motions[i][j].File;
                            CubismFadeMotionData cubismFadeMotionData = CubismFadeMotionData.CreateInstance(cubismMotion3Json, motions[i][j].File, animationClip.length, false, false);

                            motionDict.Add(k, cubismFadeMotionData);
                            k++;

                            //model.gameObject.GetComponent<CubismMotionController>().PlayAnimation(animationClip, isLoop: false);
                            Debug.Log(motions[i][j].File);
                            Debug.Log(animationClip.name);
                        }
                    }


                    fadeMotions.MotionInstanceIds = new int[k];
                    fadeMotions.CubismFadeMotionObjects = new CubismFadeMotionData[k];
                    foreach (KeyValuePair<int, CubismFadeMotionData> kvp in motionDict)
                    {
                        fadeMotions.CubismFadeMotionObjects[kvp.Key] = kvp.Value;
                    }


                    model.gameObject.AddComponent<CubismMotionController>();
                    CubismFadeController cubismFadeController = model.GetComponent<CubismFadeController>();
                    cubismFadeController.CubismFadeMotionList = fadeMotions;

                    for (var i = 0; i < motions.Length; ++i)
                    {
                        for (var j = 0; j < motions[i].Length; ++j)
                        {
                            string streamingAssetsMotionPath = Application.streamingAssetsPath + "/models/" + ModelDropdownBox.selectedText.text + "/" + motions[i][j].File;
                            CubismMotion3Json cubismMotion3Json = CubismMotion3Json.LoadFrom(File.ReadAllText(streamingAssetsMotionPath));
                            AnimationClip animationClip = cubismMotion3Json.ToAnimationClip(new AnimationClip
                            {
                                legacy = true
                            }, false, false, false, null);
                            animationClip.name = motions[i][j].File;
                            model.gameObject.GetComponent<CubismMotionController>().PlayAnimation(animationClip, isLoop: false);
                            //Debug.Log(motions[i][j].File);
                            Debug.Log(animationClip.name);
                        }
                    }*/


                    return model;
                }
                else
                {
                    throw new Exception("文件不存在");
                }
            }
            catch (Exception err) {
                DialogBoxTitle.text = @"错误";
                DialogBoxText.text = err.Message + "\n" + err.StackTrace;
                DialogBox.Open();
            }
            return null;
        }

        public ArrayList GetModelsFromAssets()
        {
            ArrayList returnArray = new ArrayList();
            string streamingAssetsPath = Application.streamingAssetsPath + "/models";
            DirectoryInfo dir = new DirectoryInfo(streamingAssetsPath);
            foreach (FileInfo file in dir.GetFiles("*.model3.json", SearchOption.AllDirectories))
            {
                returnArray.Add(file.Name.Substring(0, file.Name.Length - 12));
                Debug.Log(file.Name);
            }
            return returnArray;
        }

        public void DoJsonPrase(string input, CubismModel model)
        {
            if (input.Substring(0, 1) == "{" && input.Substring(input.Length - 2, 2) == "}}")
            {
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(input);
                model.GetComponent<AMGModelController>().paramMouthOpenYValue = float.Parse(jsonResult["mouthOpenY"].ToString());
                model.GetComponent<AMGModelController>().ParamEyeBallXValue = float.Parse(jsonResult["eyeX"].ToString());
                model.GetComponent<AMGModelController>().ParamEyeBallYValue = float.Parse(jsonResult["eyeY"].ToString());
                model.GetComponent<AMGModelController>().paramAngleXValue = float.Parse(jsonResult["headYaw"].ToString());
                model.GetComponent<AMGModelController>().paramAngleYValue = float.Parse(jsonResult["headPitch"].ToString());
                model.GetComponent<AMGModelController>().paramAngleZValue = float.Parse(jsonResult["headRoll"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleXValue = float.Parse(jsonResult["bodyAngleX"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleYValue = float.Parse(jsonResult["bodyAngleY"].ToString());
                //model.GetComponent<AMGModelController>().ParamBodyAngleZValue = float.Parse(jsonResult["bodyAngleZ"].ToString());
                model.GetComponent<AMGModelController>().paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
                model.GetComponent<AMGModelController>().paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
                model.GetComponent<AMGModelController>().paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
                model.GetComponent<AMGModelController>().paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
                model.GetComponent<AMGModelController>().paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
                model.GetComponent<AMGModelController>().paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
                model.GetComponent<AMGModelController>().paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());
            }
        }

        public CubismModel GetModelFromDropdown(ArrayList ModelList)
        {
            if (ControlModelDropdownBox.selectedText.text != "选择控制器")
            {
                var modelname = ControlModelDropdownBox.selectedText.text;
                foreach (CubismModel Model in ModelList)
                {
                    if (Model.name == modelname)
                    {
                        return Model;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public static object BuiltinLoadAssetAtPath(Type assetType, string absolutePath)
        {
            if (assetType == typeof(byte[]))
            {
                return File.ReadAllBytes(absolutePath);
            }
            if (assetType == typeof(string))
            {
                return File.ReadAllText(absolutePath);
            }
            if (assetType == typeof(Texture2D))
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes(absolutePath));
                return texture2D;
            }
            throw new NotSupportedException();
        }
    }
}
