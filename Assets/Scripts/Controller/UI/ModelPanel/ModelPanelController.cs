using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using MaterialUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ModelPanelController : MonoBehaviour
    {
        [SerializeField] private Toggle ModelLostResetToggle;
        [SerializeField] private Toggle ModelLostResetEyeToggle;
        [SerializeField] private Toggle ModelMotionLoopToggle;
        [SerializeField] private Slider ModelShowLevelSlider;
        [SerializeField] private SelectionBoxConfig ModelLostResetChooseDropdown;
        [SerializeField] private SelectionBoxConfig ModelLostResetActionDropdown;
        [SerializeField] private Button ModelConfigSaveButton;
        [SerializeField] private Button ModelConfigLoadButton;
        [SerializeField] private SaveController SaveController;
        [SerializeField] private SettingPanelController SettingPanelController;


        [SerializeField] private GameObject ShortcutClassObject;
        [SerializeField] private GameObject ShortcutClassObjectParent;
        [SerializeField] private ShortcutController ShortcutController;


        private void Start()
        {
            ModelShowLevelSlider.onValueChanged.AddListener((float value) => { OnModelShowLevelSliderValueChanged(value); });
            ModelLostResetToggle.onValueChanged.AddListener((bool value) => { OnModelLostResetToggleValueChanged(value); });
            ModelLostResetEyeToggle.onValueChanged.AddListener((bool value) => { OnModelLostResetEyeToggleValueChanged(value); });
            ModelMotionLoopToggle.onValueChanged.AddListener((bool value) => { OnModelMotionLoopToggleValueChanged(value); });
            ModelLostResetChooseDropdown.ItemPicked += OnModelLostResetChooseDropdownSelected;
            ModelLostResetActionDropdown.ItemPicked += OnModelLostResetActionDropdownSelected;
            ModelConfigSaveButton.onClick.AddListener(() => { OnModelConfigSaveButtonClick(); });
            ModelConfigLoadButton.onClick.AddListener(() => { OnModelConfigLoadButtonClick(); });
        }

        public void SetValueFromModel()
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    ModelLostResetToggle.isOn = controller.LostReset;
                    ModelLostResetEyeToggle.isOn = controller.LostResetEye;
                    ModelMotionLoopToggle.isOn = controller.LostResetMotionLoop;
                    ModelLostResetChooseDropdown.selectedText.text = ModelLostResetChooseDropdown.listItems[controller.LostResetAction];
                    ModelLostResetActionDropdown.listItems = new string[controller.animationClips.Count];
                    var i = 0;
                    foreach (var name in controller.animationClips)
                    {
                        ModelLostResetActionDropdown.listItems[i] = name.ToString();
                        i++;
                    }
                    ModelLostResetActionDropdown.RefreshList();
                    ModelLostResetActionDropdown.selectedText.text = controller.LostResetMotion;
                    ModelShowLevelSlider.value = model.gameObject.GetComponent<CubismRenderController>().SortingOrder;
                }
            }
        }

        public void OnModelLostResetChooseDropdownSelected(int id)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    controller.LostResetAction = id;
                }
            }
        }

        public void OnModelLostResetActionDropdownSelected(int id)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    controller.LostResetMotion = ModelLostResetActionDropdown.selectedText.text;
                }
            }
        }

        public void OnModelConfigSaveButtonClick()
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    //处理快捷键
                    var aniDict = new Dictionary<string, Dictionary<string, string>>();

                    foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                    {
                        foreach (KeyValuePair<string, ShortcutClass> kkvp in ShortcutController.ShortcutDict[kvp.Key])
                        {
                            if (kkvp.Value.Model == model)
                            {
                                var ddict = new Dictionary<string, string>();
                                ddict.Add("AnimationClip", kkvp.Value.AnimationClip);
                                ddict.Add("Parameter", kkvp.Value.Parameter);
                                ddict.Add("UUID", kkvp.Value.UUID);
                                ddict.Add("isPressedText", kkvp.Value.isPressedText);
                                ddict.Add("MType", kkvp.Value.MType.ToString());
                                ddict.Add("Type", kkvp.Value.Type.ToString());
                                ddict.Add("IsInvert", kkvp.Value.IsInvert.ToString());
                                ddict.Add("IsLock", kkvp.Value.IsLock.ToString());
                                ddict.Add("IsLoop", kkvp.Value.IsLoop.ToString());
                                ddict.Add("Duration", kkvp.Value.fps.ToString());
                                var KeyboardPressedString = string.Join(",", kkvp.Value.isCPressed.ToArray());
                                ddict.Add("isCPressed", KeyboardPressedString);
                                aniDict.Add(kkvp.Value.UUID, ddict);
                            }
                        }
                    }

                    SaveController.SaveUserData(controller.ModelPath, controller.ConnectionUUID, controller.GetModelSettings(), controller.GetModelOtherSettings(), controller.GetModelLocationSettings(), aniDict);
                }
            }
        }

        public void OnModelConfigLoadButtonClick()
        {
            try
            {
                var model = SettingPanelController.GetCubismModelSelected();
                if (model != null)
                {
                    if (model.GetComponent<Live2DModelController>() != null)
                    {
                        var controller = model.GetComponent<Live2DModelController>();
                        var data = SaveController.LoadUserData(controller.ModelPath);
                        controller.SetModelSettings(data.ModelAlign);
                        controller.SetModelOtherSettings(data.ModelOtherSettings);
                        controller.SetModelLocationSettings(data.ModelLocationSettings);
                        if (data.LastDUID != null)
                        {
                            controller.ConnectionUUID = data.LastDUID;
                        }
                        SetValueToCubismShortcut(data.ShortcutPair, model);
                        SetValueFromModel();
                        SettingPanelController.ResetModelAdvancedPanel();
                        SettingPanelController.ResetShortcutPanel();
                    }
                }
            }
            catch (Exception err)
            {
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelConfigLoadException", err.Message));
            }
        }

        public void OnModelLostResetToggleValueChanged(bool value)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    controller.LostReset = value;
                }
            }
        }

        public void OnModelLostResetEyeToggleValueChanged(bool value)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    controller.LostResetEye = value;
                }
            }
        }

        public void OnModelMotionLoopToggleValueChanged(bool value)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var controller = model.GetComponent<Live2DModelController>();
                    controller.LostResetMotionLoop = value;
                }
            }
        }

        public void OnModelShowLevelSliderValueChanged(float value)
        {
            var model = SettingPanelController.GetCubismModelSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    model.gameObject.GetComponent<CubismRenderController>().SortingOrder = (int)value;
                }
            }
        }

        public void SetValueToCubismShortcut(Dictionary<string, Dictionary<string, string>> aniDict, CubismModel model)
        {
            if (model.GetComponent<Live2DModelController>() != null)
            {
                var controller = model.GetComponent<Live2DModelController>();
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in aniDict)
                {
                    var ani = aniDict[kvp.Key];
                    var isPressed = ani["isCPressed"].Split(',').ToList();

                    if (model.Parameters.FindById(ani["Parameter"]) != null || controller.animationClips.Contains(ani["AnimationClip"]))
                    {
                        var item = Instantiate(ShortcutClassObject);
                        item.transform.SetParent(ShortcutClassObjectParent.transform, false);
                        var sclass = item.GetComponent<ShortcutClass>();
                        sclass.Model = model;
                        sclass.AnimationClip = ani["AnimationClip"];
                        sclass.Parameter = ani["Parameter"];
                        sclass.UUID = ani["UUID"];
                        sclass.isCPressed = isPressed;
                        sclass.isPressedText = ani["isPressedText"];
                        sclass.MType = int.Parse(ani["MType"]);
                        sclass.Type = int.Parse(ani["Type"]);
                        sclass.IsInvert = bool.Parse(ani["IsInvert"]);
                        sclass.IsLock = bool.Parse(ani["IsLock"]);
                        sclass.IsLoop = bool.Parse(ani["IsLock"]);
                        sclass.fps  = float.Parse(ani["Duration"]);
                        ShortcutController.SetShortcutClass(isPressed, sclass, ani["UUID"]);
                        item.SetActive(true);
                    }
                }
            }
        }
    }
}
