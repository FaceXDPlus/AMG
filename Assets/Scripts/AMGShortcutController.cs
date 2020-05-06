using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AMG
{
    public class AMGShortcutController : MonoBehaviour
    {
        private Button ShortcutItem;
        private VerticalLayoutGroup ShortcutGroup;

        public void setVerticalLayoutGroup(VerticalLayoutGroup ShortcutGroup)
        {
            this.ShortcutGroup = ShortcutGroup;
        }

        public void setExampleButton(Button ShortcutItem)
        {
            this.ShortcutItem = ShortcutItem;
        }

        public void refreshVerticalLayoutGroup()
        {
            for (int i = 0; i < ShortcutGroup.gameObject.transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(ShortcutGroup.gameObject.transform.GetChild(i).gameObject);
            }
        }

        public void addVerticalLayoutGroupItem(string name, CubismModel model)
        {
            Button citem = Instantiate<Button>(ShortcutItem);
            citem.name = name;
            citem.gameObject.SetActive(true);
            citem.transform.SetParent(ShortcutGroup.gameObject.transform, false);
            var controller = citem.GetComponent<AMGShortcutItemController>();
            controller.ItemAction.text = name;
            controller.SetAnimationName(name);
            controller.SetModel(model);
            controller.InitShortcutItem();
        }
    }
}