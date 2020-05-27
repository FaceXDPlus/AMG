using MaterialUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace AMG
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private CanvasController CanvasController;
        //主面板控制器
        [SerializeField] private MainPanelController MainPanelController;
        
        //DX控制器
        [SerializeField] private DXHelper dxInterface;

        //分辨率
        [SerializeField] private InputField ResolutionRatioX;
        [SerializeField] private InputField ResolutionRatioY;
        [SerializeField] private InputField DXResolutionRatioX;
        [SerializeField] private InputField DXResolutionRatioY;
        [SerializeField] private InputField TargetFrameRate;
        [SerializeField] private UnityEngine.UI.Button ResolutionRatioButton;
        [SerializeField] private UnityEngine.UI.Button DXResolutionRatioButton;
        [SerializeField] private UnityEngine.UI.Button TargetFrameRateButton;

        //模型数量控制
        [SerializeField] private UnityEngine.UI.Button ModelAddButton;
        [SerializeField] private UnityEngine.UI.Button ModelRefreshButton;
        [SerializeField] private SelectionBoxConfig ModelDropdownBox;
        [SerializeField] private GameObject ModelParent;

        //丢失模型动画
        [SerializeField] private GameObject ConnectionLost;  

        private Live2DHelper Live2DHelper;

        void Start()
        {
            ResolutionRatioButton.onClick.AddListener(() => { OnResolutionRatioButtonClick(); });
            DXResolutionRatioButton.onClick.AddListener(() => { OnDXResolutionRatioButtonClick(); });
            TargetFrameRateButton.onClick.AddListener(() => { OnFrameRateButtonClick(); });
            ModelAddButton.onClick.AddListener(() => { OnModelAddButtonClick(); });
            ModelRefreshButton.onClick.AddListener(() => { OnModelRefreshButtonClick(); });
            Live2DHelper = new Live2DHelper();
            RefreshModels();
        }

        private void OnResolutionRatioButtonClick()
        {
            Screen.SetResolution(Convert.ToInt32(ResolutionRatioX.text), Convert.ToInt32(ResolutionRatioY.text), false);
        }

        private void OnDXResolutionRatioButtonClick()
        {
            dxInterface.renderTextureHeight = Convert.ToInt32(DXResolutionRatioY.text);
            dxInterface.renderTextureWidth = Convert.ToInt32(DXResolutionRatioX.text);
            MainPanelController.DXWindowToggle.isOn = false;
        }

        public void OnFrameRateButtonClick()
        {
            Application.targetFrameRate = Convert.ToInt32(TargetFrameRate.text);
        }

        public void OnModelAddButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("AddModel", 0.25f);
        }

        public void OnModelRefreshButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("RefreshModels", 0.25f);
        }

        public void AddModel()
        {
            var model = Live2DHelper.GetModelFromName(ModelDropdownBox.selectedText.text, ModelParent);
            model.GetComponent<Live2dModelController>().MainPanelController = MainPanelController;
            var connectionLost = Instantiate(ConnectionLost);
            connectionLost.transform.SetParent(model.gameObject.transform);
            connectionLost.transform.localPosition = model.gameObject.transform.localPosition;
            connectionLost.GetComponent<PNGListHelper>().Init(); 
            model.GetComponent<Live2dModelController>().ConnectionLost = connectionLost;
        }

        public void RefreshModels()
        {
            var models = Live2DHelper.GetModelsFromAssets();
            var returnCount = models.Count;
            if (returnCount > 0)
            {
                ModelDropdownBox.listItems = new string[returnCount];
                int i = 0;
                while (i < returnCount)
                {
                    ModelDropdownBox.listItems[i] = models[i].ToString();
                    i++;
                }
                ModelDropdownBox.selectedText.text = "请选择模型";
                ModelDropdownBox.RefreshList();
            }
            else
            {
                ModelDropdownBox.selectedText.text = "未找到模型";
            }
        }

    }
}
