using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
    public class ModelAdvancedController : MonoBehaviour
    {
        [SerializeField] private GameObject AdvancedObject;
        [SerializeField] private GameObject AdvancedObjectParent;
        [SerializeField] private SettingPanelController SettingPanelController;
        public bool IsProcessing = false;
        private ArrayList Objects = new ArrayList();

        public void OnEnable()
        {
            if (!IsProcessing) {
                IsProcessing = true;
                var model = SettingPanelController.GetCubismModelSelected();
                if (model != null)
                {
                    if (model.GetComponent<Live2DModelController>() != null)
                    {
                        var InitedParameters = model.GetComponent<Live2DModelController>().InitedParameters;
                        foreach (KeyValuePair<string, ParametersClass> kvp in InitedParameters)
                        {
                            if (kvp.Value.Name != "paramBreath")
                            {
                                var advancedObject = Instantiate(AdvancedObject);
                                advancedObject.transform.SetParent(AdvancedObjectParent.transform, false);
                                advancedObject.SetActive(true);
                                var controller = advancedObject.GetComponent<ModelAdvancedItemController>();
                                controller.ParameterName.text = kvp.Value.Name;
                                var increase = 2f;
                                if (kvp.Value.MaxValue > 1)
                                {
                                    increase = 50;
                                }
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
                            }
                        }
                    }
                }
                IsProcessing = false;
            }
        }

        public void OnDisable()
        {
            if (!IsProcessing)
            {
                IsProcessing = true;
                foreach (var aobject in Objects)
                {
                    UnityEngine.Object.Destroy((GameObject)aobject);
                }
                IsProcessing = false;
            }
        }
    }
}