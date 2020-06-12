using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using MaterialUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class ModelPanelController : MonoBehaviour
    {
        [SerializeField] private Toggle ModelLostResetToggle;
        [SerializeField] private Slider ModelShowLevelSlider;
        [SerializeField] private SelectionBoxConfig ModelLostResetChooseDropdown;
        [SerializeField] private SelectionBoxConfig ModelLostResetActionDropdown;
        [SerializeField] private Button ModelConfigSaveButton;
        [SerializeField] private Button ModelConfigLoadButton;
        [SerializeField] private SaveController SaveController;
        [SerializeField] private SettingPanelController SettingPanelController;


        private void Start()
        {
            ModelShowLevelSlider.onValueChanged.AddListener((float value) => { OnModelShowLevelSliderValueChanged(value); });
            ModelLostResetToggle.onValueChanged.AddListener((bool value) => { OnModelLostResetToggleValueChanged(value); });
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
                    ModelLostResetToggle.isOn  = controller.LostReset;
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

                    /*foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                    {
                        foreach (KeyValuePair<string, ShortcutClass> kkvp in ShortcutController.ShortcutDict[kvp.Key])
                        {
                            if (kkvp.Value.Model == model)
                            {
                                if (kkvp.Value.Type == 0)
                                {
                                    //aniDict.Add(kkvp.Value.AnimationClip, kkvp.Value);
                                }
                                else if (kkvp.Value.Type == 1)
                                {
                                    //aniDict.Add(kkvp.Value.Parameter, kkvp.Value);
                                }
                            }
                        }
                    }*/


                    SaveController.SaveUserData(controller.ModelPath, controller.GetModelSettings(), controller.GetModelOtherSettings(), controller.GetModelLocationSettings(), null);
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
                        SetValueFromModel();
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
    }
}
