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
                        item.SetActive(true);
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
