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

        //P2P连接
        [SerializeField] private InputField P2PClientField;
        [SerializeField] private UnityEngine.UI.Toggle P2PClientToggle;
        [SerializeField] private WSHelper WebSocketHelper;
        [SerializeField] private Text P2PClientName;
     
        //丢失模型动画
        [SerializeField] private GameObject ConnectionLost;

        //其他控制器
        [SerializeField] private Live2DHelper Live2DHelper;
        [SerializeField] private LangController LangController;

        void Start()
        {
            P2PClientName.text = Globle.GetComputerName();
            ModelAddButton.onClick.AddListener(() => { OnModelAddButtonClick(); });
            ModelRefreshButton.onClick.AddListener(() => { OnModelRefreshButtonClick(); });
            ModelRemoveButton.onClick.AddListener(() => { OnModelRemoveButtonClick(); });
            ModelSelectionDropdownBox.ItemPicked += OnModelSelectionDropdownBoxSelected;
            ModelIPDropdownBox.ItemPicked += OnModelIPDropdownBoxSelected;
            P2PClientToggle.onValueChanged.AddListener((bool isOn) => { OnP2PClientToggleClick(isOn); });
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

        public CubismModel GetCubismModelSelected()
        {
            if (ModelSelectionDropdownBox.selectedText.text != "/")
            {
                foreach (CubismModel model in Globle.ModelList)
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
                ModelIPDropdownBox.selectedText.text = GetCubismModelSelected().GetComponent<Live2DModelController>().ConnectionUUID;
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
            var model = GetCubismModelSelected();
            if (model != null)
            {
                model.GetComponent<Live2DModelController>().ConnectionUUID = ModelIPDropdownBox.selectedText.text;
                Globle.AddDataLog("Model", LangController.GetLang("LOG.SetModelIP", ModelSelectionDropdownBox.selectedText.text, ModelIPDropdownBox.selectedText.text));
            }
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
                model.GetComponent<Live2DModelController>().SettingPanelController = this;
                var connectionLost = Instantiate(ConnectionLost);
                connectionLost.transform.SetParent(model.gameObject.transform);
                connectionLost.transform.localPosition = model.gameObject.transform.localPosition;
                connectionLost.GetComponent<PNGListHelper>().Init();
                model.GetComponent<Live2DModelController>().ConnectionLost = connectionLost;
                ResetModelSelectionDropdown();
                ModelIPDropdownBox.selectedText.text = "/"; 
                ModelIPDropdownBox.currentSelection = -1;
            }
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
        }

        public void RemoveModel()
        {
            var model = GetCubismModelSelected();
            if (model != null)
            {
                Globle.ModelList.Remove(model);
                UnityEngine.Object.Destroy(model.gameObject.GetComponent<Live2DModelController>().ConnectionLost);
                UnityEngine.Object.Destroy(model.gameObject.GetComponent<Live2DModelController>());
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
                var model = (CubismModel)list[i];
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
        }

        public void ResetShortcutPanel()
        {
            ShortcutPanelController.OnDisable();
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
    }
}
