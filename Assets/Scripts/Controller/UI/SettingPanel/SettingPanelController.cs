using Live2D.Cubism.Core;
using MaterialUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //其他面板对象
        [SerializeField] private ModelPanelController ModelPanelController;
        [SerializeField] private ModelAdvancedController ModelAdvancedController;
        [SerializeField] private ShortcutPanelController ShortcutPanelController;

        //DX控制器
        [SerializeField] private DXHelper dxInterface;

        //模型数量控制
        [SerializeField] private UnityEngine.UI.Button ModelAddButton;
        [SerializeField] private UnityEngine.UI.Button ModelRefreshButton;
        [SerializeField] private UnityEngine.UI.Button ModelRemoveButton;
        [SerializeField] private SelectionBoxConfig ModelDropdownBox;
        [SerializeField] private SelectionBoxConfig ModelSelectionDropdownBox;
        [SerializeField] private SelectionBoxConfig ModelIPDropdownBox;
        [SerializeField] private GameObject ModelParent;
        [SerializeField] private GameObject MouseObject;


        //VRM模型数量控制
        [SerializeField] private UnityEngine.UI.Button VRMModelAddButton;
        [SerializeField] private UnityEngine.UI.Button VRMModelRefreshButton;
        [SerializeField] private SelectionBoxConfig VRMModelDropdownBox;
        [SerializeField] private GameObject VRMModelParent;

        //P2P连接
        [SerializeField] private InputField P2PClientField;
        [SerializeField] private UnityEngine.UI.Toggle P2PClientToggle;
        [SerializeField] private WSHelper WebSocketHelper;
        [SerializeField] private Text P2PClientName;
     
        //丢失模型动画
        [SerializeField] private GameObject ConnectionLost;

        //其他控制器
        [SerializeField] private Live2DHelper Live2DHelper;
        [SerializeField] private VRMHelper VRMHelper;
        [SerializeField] private LangController LangController;
        [SerializeField] private ShortcutController ShortcutController;

        //分辨率
        [SerializeField] public InputField ResolutionRatioX;
        [SerializeField] public InputField ResolutionRatioY;
        [SerializeField] private UnityEngine.UI.Button ResolutionRatioButton;

        void Start()
        {
            P2PClientName.text = Globle.GetComputerName();
            ModelAddButton.onClick.AddListener(() => { OnModelAddButtonClick(); });
            ModelRefreshButton.onClick.AddListener(() => { OnModelRefreshButtonClick(); });
            //VRM
            VRMModelAddButton.onClick.AddListener(() => { OnVRMModelAddButtonClick(); });
            VRMModelRefreshButton.onClick.AddListener(() => { OnVRMModelRefreshButtonClick(); });

            ModelRemoveButton.onClick.AddListener(() => { OnModelRemoveButtonClick(); });
            ModelSelectionDropdownBox.ItemPicked += OnModelSelectionDropdownBoxSelected;
            ModelIPDropdownBox.ItemPicked += OnModelIPDropdownBoxSelected;
            P2PClientToggle.onValueChanged.AddListener((bool isOn) => { OnP2PClientToggleClick(isOn); });
            ResolutionRatioButton.onClick.AddListener(() => { OnResolutionRatioButtonPressed(); });
            var none = new string[1];
            none[0] = "/";
            ModelSelectionDropdownBox.listItems = none;
            ModelIPDropdownBox.listItems = none;
            RefreshModels();
        }

        void Update()
        {
            if (Globle.WSClientsChanged)
            {
                Globle.WSClientsChanged = false;
                OnRefreshModelIPDropdownBoxDropdown();
            }
        }

        #region Model

        public string GetModelSelected()
        {
            return ModelSelectionDropdownBox.selectedText.text;
        }

        public GameObject GetModelObjectSelected()
        {
            if (ModelSelectionDropdownBox.selectedText.text != "/")
            {
                foreach (GameObject model in Globle.ModelList)
                {
                    if (ModelSelectionDropdownBox.selectedText.text == model.name)
                    {
                        return model;
                    }
                }
            }
            return null;
        }

        public void OnModelSelectionDropdownBoxSelected(int id)
        {
            if (id != 0) {
                Globle.AddDataLog("Model", LangController.GetLang("LOG.SelectModel", ModelSelectionDropdownBox.selectedText.text));
                ModelPanelController.SetValueFromModel();
                var model = GetModelObjectSelected();
                if (model != null)
                {
                    if (model.GetComponent<Live2DModelController>() != null)
                    {
                        ModelIPDropdownBox.selectedText.text = model.GetComponent<Live2DModelController>().ConnectionUUID;
                    }
                    else if (model.GetComponent<VRMModelController>() != null)
                    {
                        ModelIPDropdownBox.selectedText.text = model.GetComponent<VRMModelController>().ConnectionUUID;
                    }
                }
            }
            else
            {
                ModelIPDropdownBox.selectedText.text = "/";
            }
            ResetModelAdvancedPanel();
            ResetShortcutPanel();
        }

        public void OnModelIPDropdownBoxSelected(int id)
        {
            var model = GetModelObjectSelected();
            if (model != null)
            {
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    model.GetComponent<Live2DModelController>().ConnectionUUID = ModelIPDropdownBox.selectedText.text;
                }
                else if (model.GetComponent<VRMModelController>() != null)
                {
                    model.GetComponent<VRMModelController>().ConnectionUUID = ModelIPDropdownBox.selectedText.text;
                }
                Globle.AddDataLog("Model", LangController.GetLang("LOG.SetModelIP", ModelSelectionDropdownBox.selectedText.text, ModelIPDropdownBox.selectedText.text));
            }
        }

        public void OnModelAddButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("AddModel", 0.25f);
        }

        public void OnVRMModelAddButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("AddVRMModel", 0.25f);
        }

        public void OnModelRefreshButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("RefreshModels", 0.25f);
        }

        public void OnVRMModelRefreshButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("RefreshModels", 0.25f);
        }

        public void OnModelRemoveButtonClick()
        {
            CanvasController.CloseAllDropdown();
            Invoke("RemoveModel", 0.25f);
        }

        public void AddModel()
        {
            var model = Live2DHelper.GetModelFromName(ModelDropdownBox.selectedText.text, ModelParent);
            if (model != null)
            {
                var controller = model.GetComponent<Live2DModelController>();
                controller.SettingPanelController = this;
                var connectionLost = Instantiate(ConnectionLost);
                connectionLost.transform.SetParent(model.gameObject.transform);
                connectionLost.transform.localPosition = model.gameObject.transform.localPosition;
                connectionLost.GetComponent<PNGListHelper>().Init();
                controller.ConnectionLost = connectionLost;
                controller.MouseObject = MouseObject;
                ResetModelSelectionDropdown();
                ModelIPDropdownBox.selectedText.text = "/"; 
                ModelIPDropdownBox.currentSelection = -1; 
                Invoke("ReloadModelSettings", 0.7f);
            }
        }

        public void AddVRMModel()
        {
            VRMHelper.GetModelFromName(VRMModelDropdownBox.selectedText.text, VRMModelParent, MouseObject);
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
                ModelDropdownBox.selectedText.text = "/";
                ModelDropdownBox.RefreshList();
            }
            else
            {
                ModelDropdownBox.selectedText.text = "/";
            }
            //处理VRM
            var vrmmodels = VRMHelper.GetModelsFromAssets();
            var vrmreturnCount = vrmmodels.Count;
            if (vrmreturnCount > 0)
            {
                VRMModelDropdownBox.listItems = new string[vrmreturnCount];
                int i = 0;
                while (i < vrmreturnCount)
                {
                    VRMModelDropdownBox.listItems[i] = vrmmodels[i].ToString();
                    i++;
                }
                VRMModelDropdownBox.selectedText.text = "/";
                VRMModelDropdownBox.RefreshList();
            }
            else
            {
                VRMModelDropdownBox.selectedText.text = "/";
            }
        }

        public void RemoveModel()
        {
            var model = GetModelObjectSelected();
            if (model != null)
            {
                ShortcutController.RemoveShortcutClassByModel(model);
                Globle.ModelList.Remove(model);
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    UnityEngine.Object.Destroy(model.gameObject.GetComponent<Live2DModelController>().ConnectionLost);
                    UnityEngine.Object.Destroy(model.gameObject.GetComponent<Live2DModelController>());
                }
                UnityEngine.Object.Destroy(model.gameObject);
                ResetModelSelectionDropdown();
                ModelIPDropdownBox.selectedText.text = "/";
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
                ModelAdvancedController.OnDisable();
                return;
            }
        }

        public void ResetModelSelectionDropdown()
        {
            var list = Globle.ModelList;
            var listCount = list.Count + 1;
            ModelSelectionDropdownBox.listItems = new string[listCount];
            ModelSelectionDropdownBox.listItems[0] = "/";
            int i = 0;
            while (i < list.Count)
            {
                var model = (GameObject)list[i];
                ModelSelectionDropdownBox.listItems[i + 1] = model.name;
                i++;
            }
            ModelSelectionDropdownBox.RefreshList();
            ModelSelectionDropdownBox.currentSelection = -1;
            ModelSelectionDropdownBox.Select(i);
        }

        public void ResetModelAdvancedPanel()
        {
            ModelAdvancedController.OnDisable();
            ModelAdvancedController.gameObject.SetActive(false);
            MainPanelController.ModelAdvancedToggle.isOn = false;
        }

        public void ResetShortcutPanel()
        {
            ShortcutPanelController.OnDisable();
            ShortcutPanelController.gameObject.SetActive(false);
            MainPanelController.ShortcutToggle.isOn = false;
        }

        public void ReloadModelSettings()
        {
            ModelPanelController.OnModelConfigLoadButtonClick();
        }

        #endregion

        #region IP

        public void OnRefreshModelIPDropdownBoxDropdown()
        {
            CanvasController.CloseAllDropdown();
            Invoke("RefreshModelIPDropdownBoxDropdown", 0.25f);
        }

        public void RefreshModelIPDropdownBoxDropdown()
        {
            if (Globle.WSClients.Count > 0)
            {
                var listCount = Globle.WSClients.Count + 1;
                ModelIPDropdownBox.listItems = new string[listCount];
                ModelIPDropdownBox.listItems[0] = "/";
                var i = 1;
                foreach (KeyValuePair<string, WSClientClass> kvp in Globle.WSClients)
                {
                    ModelIPDropdownBox.listItems[i] = kvp.Key;
                    i++;
                }
            }
            else
            {
                var none = new string[1];
                none[0] = "/";
                ModelIPDropdownBox.listItems = none;
            }
            ModelIPDropdownBox.RefreshList();
            ModelIPDropdownBox.currentSelection = -1;
            Globle.AddDataLog("IP", LangController.GetLang("LOG.RefreshIPList"));
        }

        #endregion
    
        public void OnP2PClientToggleClick(bool isOn)
        {
            if (isOn && P2PClientField.text != "")
            {
                WebSocketHelper.P2PClientStart(P2PClientField.text);

            }
            else
            {
                if (WebSocketHelper.P2PClientStatus == true)
                {
                    WebSocketHelper.P2PClientStop();
                }
            }
        }

        public void OnResolutionRatioButtonPressed()
        {
            if (ResolutionRatioX.text != "" && ResolutionRatioY.text != "")
            {
                CanvasController.MainStorage.ResolutionRatioX = Convert.ToInt32(ResolutionRatioX.text);
                CanvasController.MainStorage.ResolutionRatioY = Convert.ToInt32(ResolutionRatioY.text);
                //Screen.SetResolution(Convert.ToInt32(ResolutionRatioX.text), Convert.ToInt32(ResolutionRatioY.text), false);
                CanvasController.GetComponent<DXHelper>().renderTextureHeight = Convert.ToInt32(ResolutionRatioY.text);
                CanvasController.GetComponent<DXHelper>().renderTextureWidth = Convert.ToInt32(ResolutionRatioX.text);
                CanvasController.Save();
            }
        }
    }
}
