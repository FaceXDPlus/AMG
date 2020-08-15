using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.Pose;
using Live2D.Cubism.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AMG
{
    public class Live2DHelper : MonoBehaviour
    {
        public ArrayList GetModelsFromAssets()
        {
            ArrayList returnArray = new ArrayList();
            string streamingAssetsPath = Application.streamingAssetsPath + "../../../Data/live2d";
            DirectoryInfo dir = new DirectoryInfo(streamingAssetsPath);
            foreach (FileInfo file in dir.GetFiles("*.model3.json", SearchOption.AllDirectories))
            {
                returnArray.Add(file.Directory.Name + "/" + file.Name.Substring(0, file.Name.Length - 12));
            }
            return returnArray;
        }

        public CubismModel GetModelFromName(string name, GameObject parent)
        {
            try
            {
                string ModelPath = Application.streamingAssetsPath + "../../../Data/live2d/" + name + ".model3.json";
                //Debug.Log(ModelPath);
                if (System.IO.File.Exists(ModelPath))
                {
                    var ModelFullName = ModelPath;
                    var model3Json = CubismModel3Json.LoadAtPath(ModelFullName, BuiltinLoadAssetAtPath);
                    var motions = model3Json.FileReferences.Motions.Motions;
                    var model = model3Json.ToModel(true);
                    var Scale = 40f;
                    model.gameObject.transform.localScale += new Vector3(Scale, Scale);
                    CubismRenderController cubisumRenderController = model.GetComponent<CubismRenderController>();
                    cubisumRenderController.SortingMode = CubismSortingMode.BackToFrontOrder;

                    //处理动画(Motion)
                    Animation animation = model.gameObject.AddComponent<Animation>();
                    var animationClips = new ArrayList();
                    var modelD = Path.GetDirectoryName(ModelPath);
                    if (motions != null)
                    {
                        for (var i = 0; i < motions.Length; ++i)
                        {
                            for (var j = 0; j < motions[i].Length; ++j)
                            {
                                string MotionPath = modelD + "/" + motions[i][j].File;
                                CubismMotion3Json cubismMotion3Json = CubismMotion3Json.LoadFrom(File.ReadAllText(MotionPath));
                                AnimationClip animationClip = cubismMotion3Json.ToAnimationClip(new AnimationClip
                                {
                                    legacy = true
                                }, false, false, false, null);
                                var motionName = motions[i][j].File.Substring(0, motions[i][j].File.Length - 13);
                                animationClip.name = motionName;
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

                    //+1
                    model.name = model.name + "(" + Globle.ModelNum.ToString() + ")";
                    Globle.ModelNum++; 
                    Globle.ModelList.Add(model.gameObject);

                    var modelController = model.gameObject.AddComponent<Live2DModelController>();
                    modelController.DisplayName = model.name;
                    modelController.ModelPath = modelD;
                    modelController.Animation = animation;
                    modelController.animationClips = animationClips;

                    model.transform.SetParent(parent.transform);
                    model.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                    return model;
                }
                else
                {
                    throw new Exception(Globle.LangController.GetLang("LOG.FileUnisset"));
                }
            }
            catch (Exception err)
            {
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelAddationException", err.Message));
            }
            return null;
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