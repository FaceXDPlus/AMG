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
                        var itemController = item.GetComponent<ShortcutItemController>();
                        itemController.Action.text = name;
                        item.SetActive(true);
                    }
                    /*var InitedParameters = model.GetComponent<Live2DModelController>().InitedParameters;
                    foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
                    {
                        var advancedObject = Instantiate(AdvancedObject);
                        advancedObject.transform.SetParent(AdvancedObjectParent.transform, false);
                        advancedObject.SetActive(true);
                        var controller = advancedObject.GetComponent<ModelAdvancedItemController>();
                        controller.ParameterName.text = kvp.Value.Name;
                        var increase = 5;
                        //最小值
                        controller.MinSlider.minValue = kvp.Value.MinValue - increase;
                        controller.MinSlider.maxValue = kvp.Value.MaxValue + increase;
                        controller.MinSlider.value = kvp.Value.MinSetValue;
                        //最大值
                        controller.MaxSlider.minValue = kvp.Value.MinValue - increase;
                        controller.MaxSlider.maxValue = kvp.Value.MaxValue + increase;
                        controller.MaxSlider.value = kvp.Value.MaxSetValue;
                        //当前值
                        controller.NowSlider.value = kvp.Value.NowValue;
                        controller.NowSlider.minValue = kvp.Value.MinValue - increase;
                        controller.NowSlider.maxValue = kvp.Value.MaxValue + increase;
                        controller.parametersClass = kvp.Value;
                        Objects.Add(advancedObject);
                    }*/
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
