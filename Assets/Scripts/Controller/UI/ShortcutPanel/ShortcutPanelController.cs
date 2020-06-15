using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class ShortcutPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject ShortcutObject;
        [SerializeField] private GameObject ShortcutObjectParent;
        [SerializeField] private SettingPanelController SettingPanelController;
        [SerializeField] private ShortcutController ShortcutController;
        private ArrayList Objects = new ArrayList();

        public void OnEnable()
        {
            var model = SettingPanelController.GetCubismModelSelected();
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
                    var controller = model.GetComponent<Live2DModelController>();
                    foreach (string name in controller.animationClips)
                    {
                        var item = Instantiate(ShortcutObject);
                        item.transform.SetParent(ShortcutObjectParent.transform, false);
                        var itemController = item.GetComponent<Live2DShortcutItemController>();
                        itemController.Action.text = name;
                        itemController.Model = model;
                        itemController.Name = name;
                        itemController.isAnimation = true;
                        itemController.InvertToggle.gameObject.SetActive(false);
                        itemController.LockToggle.gameObject.SetActive(false);
                        if (aniDict.ContainsKey(name))
                        {
                            var sclass = aniDict[name];
                            itemController.UUID = sclass.UUID;
                            itemController.Shortcut.text = sclass.isPressedText;
                        }
                        item.SetActive(true);
                        Objects.Add(item);
                    }

                    foreach (CubismParameter param in model.Parameters)
                    {
                        if (!controller.AInitedParameters.Contains(param))
                        {
                            var item = Instantiate(ShortcutObject);
                            item.transform.SetParent(ShortcutObjectParent.transform, false);
                            var itemController = item.GetComponent<Live2DShortcutItemController>();
                            itemController.Action.text = param.name;
                            itemController.Model = model;
                            itemController.Name = param.name;
                            itemController.isAnimation = false;
                            if (aniDict.ContainsKey(param.name))
                            {
                                var sclass = aniDict[param.name];
                                itemController.UUID = sclass.UUID;
                                itemController.Shortcut.text = sclass.isPressedText;
                                itemController.InvertToggle.isOn = sclass.IsInvert;
                                itemController.LockToggle.isOn = sclass.IsLock;
                            }
                            item.SetActive(true);
                            Objects.Add(item);

                        }
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
        }

    }
}
