using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

namespace AMG
{
    public class NewShortcutPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject ShortcutObject;
        [SerializeField] private GameObject ShortcutObjectParent;
        [SerializeField] private Button ShortcutItemAddButton;
        [SerializeField] private SettingPanelController SettingPanelController;
        [SerializeField] private ShortcutController ShortcutController;

        private List<OptionData>  OptionDatas  = new List<OptionData>();
        private List<string>  OptionDataString = new List<string>();
        private ArrayList     Objects          = new ArrayList();

        void Start()
        {
            ShortcutItemAddButton.onClick.AddListener(OnShortcutItemAddButtonClick);
        }

        public void OnShortcutItemAddButtonClick()
        {
            var model = SettingPanelController.GetModelObjectSelected();
            if (model != null)
            {
                var item = Instantiate(ShortcutObject);
                item.transform.SetParent(ShortcutObjectParent.transform, false); 
                var itemController = item.GetComponent<NewShortcutItemController>();
                itemController.DAction.ClearOptions();
                itemController.DAction.options = OptionDatas;
                itemController.Model = model;
                item.SetActive(true);
            }
        }

        public void OnEnable()
        {
            Enabled(SettingPanelController);
        }

        public void Enabled(SettingPanelController SettingPanelController)
        {
            var model = SettingPanelController.GetModelObjectSelected();
            if (model != null)
            {
                var aniDict = new Dictionary<string, ShortcutClass>();
                foreach (KeyValuePair<List<string>, Dictionary<string, ShortcutClass>> kvp in ShortcutController.ShortcutDict)
                {
                    foreach (KeyValuePair<string, ShortcutClass> kkvp in ShortcutController.ShortcutDict[kvp.Key])
                    {
                        if (kkvp.Value.Model == model)
                        {
                            if (kkvp.Value.Type == 0)
                            {
                                aniDict.Add(kkvp.Value.AnimationClip, kkvp.Value);
                            }
                            else if (kkvp.Value.Type == 1)
                            {
                                aniDict.Add(kkvp.Value.Parameter, kkvp.Value);
                            }
                        }
                    }
                }
                if (model.GetComponent<Live2DModelController>() != null)
                {
                    var cubismmodel = model.FindCubismModel();
                    var controller = model.GetComponent<Live2DModelController>();

                    var temoData = new Dropdown.OptionData();
                    temoData.text = "/";
                    OptionDatas.Add(temoData);
                    OptionDataString.Add("/");

                    for (int i = 0; i < cubismmodel.Parameters.Count(); i++) { 
                        temoData = new Dropdown.OptionData();
                        temoData.text = cubismmodel.Parameters[i].name;
                        OptionDatas.Add(temoData);
                        OptionDataString.Add(cubismmodel.Parameters[i].name);
                    }

                    for (int i = 0; i < controller.animationClips.Count; i++)
                    {
                        temoData = new Dropdown.OptionData();
                        temoData.text = controller.animationClips[i].ToString();
                        OptionDatas.Add(temoData);
                        OptionDataString.Add(controller.animationClips[i].ToString());
                    }

                    foreach (KeyValuePair<string, ShortcutClass> kvp in aniDict)
                    {
                        var item = Instantiate(ShortcutObject);
                        item.transform.SetParent(ShortcutObjectParent.transform, false);
                        var itemController = item.GetComponent<NewShortcutItemController>();

                        itemController.DAction.ClearOptions();
                        itemController.DAction.options = OptionDatas;

                        if (kvp.Value.Type == 0)
                        {
                            itemController.isAnimation = true;
                            itemController.DAction.value = OptionDataString.IndexOf(kvp.Value.AnimationClip);
                            itemController.Name = kvp.Value.AnimationClip;
                        }
                        else if (kvp.Value.Type == 1)
                        {
                            itemController.isAnimation = false;
                            itemController.DAction.value = OptionDataString.IndexOf(kvp.Value.Parameter);
                            itemController.Name = kvp.Value.Parameter;
                        }

                        itemController.Shortcut.text = kvp.Value.isPressedText;
                        itemController.UUID = kvp.Value.UUID;
                        itemController.InvertToggle.isOn = kvp.Value.IsInvert;
                        itemController.LockToggle.isOn = kvp.Value.IsLock;
                        itemController.LoopToggle.isOn = kvp.Value.IsLoop;
                        itemController.Model = model;

                        item.SetActive(true);
                        Objects.Add(item);
                    }
                }
            }
        } 

        public void OnDisable()
        {
            foreach (var aobject in Objects)
            {
                UnityEngine.Object.Destroy((GameObject)aobject);
            }
            OptionDatas = new List<OptionData>(); 
            OptionDataString = new List<string>();
        }
    }
}