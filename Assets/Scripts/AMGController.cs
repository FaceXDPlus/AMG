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
using Live2D.Cubism.Framework.Pose;

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
                string streamingAssetsPaths = Application.streamingAssetsPath + "/models/" + ModelDropdownBox.selectedText.text + "/";
                string streamingAssetsPath = Application.streamingAssetsPath + "/models/" + ModelDropdownBox.selectedText.text + "/" + ModelDropdownBox.selectedText.text + ".model3.json";
                Debug.Log(streamingAssetsPath);
                if (System.IO.File.Exists(streamingAssetsPath))
                {
                    Debug.Log("Start loading Live2D");
                    var ModelFullName = streamingAssetsPath;
                    var model3Json = CubismModel3Json.LoadAtPath(ModelFullName, BuiltinLoadAssetAtPath);
                    var motions = model3Json.FileReferences.Motions.Motions;
                    var model = model3Json.ToModel(true);
                    var Scale = 5f;
                    model.gameObject.transform.localScale += new Vector3(Scale, Scale);
                    CubismRenderController cubisumRenderController = model.GetComponent<CubismRenderController>();
                    cubisumRenderController.SortingMode = CubismSortingMode.BackToFrontOrder;

                    //处理动画
                    Animation animation = model.gameObject.AddComponent<Animation>();
                    var animationClips = new ArrayList();
                    if (motions != null)
                    {
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
                                var name = motions[i][j].File.Substring(0, motions[i][j].File.Length - 13);
                                animationClip.name = name;
                                animation.AddClip(animationClip, animationClip.name);
                                animationClips.Add(animationClip.name);
                                //animation.Blend(animationClip.name);
                            }
                        }
                    }

                    //处理姿势
                    var pose3Json = model3Json.Pose3Json;
                    if (pose3Json != null)
                    {
                        var groups = pose3Json.Groups;
                        var parts = model.Parts;
                        for (var groupIndex = 0; groupIndex < groups.Length; ++groupIndex)
                        {
                            var group = groups[groupIndex];
                            if (group == null)
                            {
                                continue;
                            }

                            for (var partIndex = 0; partIndex < group.Length; ++partIndex)
                            {
                                var part = parts.FindById(group[partIndex].Id);

                                if (part == null)
                                {
                                    continue;
                                }

                                var posePart = part.gameObject.GetComponent<CubismPosePart>();

                                if (posePart == null)
                                {
                                    posePart = part.gameObject.AddComponent<CubismPosePart>();
                                }

                                posePart.GroupIndex = groupIndex;
                                posePart.PartIndex = partIndex;
                                posePart.Link = group[partIndex].Link;
                            }
                        }
                        model.GetComponent<CubismPoseController>().Refresh();
                    }


                    model.name = model.name + "(" + Globle.ModelNum.ToString() + ")";
                    Globle.ModelNum++;
                    var modelController = model.gameObject.AddComponent<AMGModelController>();
                    modelController.DisplayName = model.name;
                    modelController.ModelPath = streamingAssetsPaths;
                    modelController.Animation = animation;
                    modelController.animationClips = animationClips;
                    return model;
                }
                else
                {
                    throw new Exception("文件不存在");
                }
            }
            catch (Exception err) {
                //Globle.AddDataLog("[Main]添加模型发生错误" + err.Message + " : " + err.StackTrace);
                DialogBoxTitle.text = @"错误";
                //DialogBoxText.text = err.Message + "\n" + err.StackTrace;
                DialogBoxText.text = err.Message;
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
            model.GetComponent<AMGModelController>().paramBrowLFormValue = float.Parse(jsonResult["eyeBrowLForm"].ToString());
            model.GetComponent<AMGModelController>().paramBrowRFormValue = float.Parse(jsonResult["eyeBrowRForm"].ToString());
            model.GetComponent<AMGModelController>().paramBrowAngleLValue = float.Parse(jsonResult["eyeBrowAngleL"].ToString());
            model.GetComponent<AMGModelController>().paramBrowAngleRValue = float.Parse(jsonResult["eyeBrowAngleR"].ToString());
            model.GetComponent<AMGModelController>().paramMouthFormValue = float.Parse(jsonResult["mouthForm"].ToString());
            model.GetComponent<AMGModelController>().paramBrowRYValue = float.Parse(jsonResult["eyeBrowYR"].ToString());
            model.GetComponent<AMGModelController>().paramBrowLYValue = float.Parse(jsonResult["eyeBrowYL"].ToString());
            model.GetComponent<AMGModelController>().paramEyeROpenValue = float.Parse(jsonResult["eyeROpen"].ToString());
            model.GetComponent<AMGModelController>().paramEyeLOpenValue = float.Parse(jsonResult["eyeLOpen"].ToString());
            if (jsonResult.Property("paramAngleXAlignValue") != null)
            {
                model.GetComponent<AMGModelController>().paramAngleXAlignValue = float.Parse(jsonResult["paramAngleXAlignValue"].ToString());
                model.GetComponent<AMGModelController>().paramAngleYAlignValue = float.Parse(jsonResult["paramAngleYAlignValue"].ToString());
                model.GetComponent<AMGModelController>().paramAngleZAlignValue = float.Parse(jsonResult["paramAngleZAlignValue"].ToString());
                model.GetComponent<AMGModelController>().paramEyeBallXAlignValue = float.Parse(jsonResult["paramEyeBallXAlignValue"].ToString());
                model.GetComponent<AMGModelController>().paramEyeBallYAlignValue = float.Parse(jsonResult["paramEyeBallYAlignValue"].ToString());
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
