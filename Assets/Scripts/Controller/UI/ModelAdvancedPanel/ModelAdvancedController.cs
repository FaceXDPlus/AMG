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
                var model = SettingPanelController.GetModelObjectSelected();
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
                                var increase = 1f;
                                if (kvp.Value.MaxValue > 5)
                                {
                                    increase = 50;
                                }
                                //最小值
                                controller.TMinSlider.minValue = kvp.Value.MinValue - increase;
                                controller.TMinSlider.maxValue = kvp.Value.MaxValue + increase;
                                controller.TMinSlider.value = kvp.Value.MinSetValue;

                                controller.MMinSlider.minValue = kvp.Value.MinValue;
                                controller.MMinSlider.maxValue = kvp.Value.MaxValue;
                                controller.MMinSlider.value = kvp.Value.MinParamValue;
                                //最大值
                                controller.TMaxSlider.minValue = kvp.Value.MinValue - increase;
                                controller.TMaxSlider.maxValue = kvp.Value.MaxValue + increase;
                                controller.TMaxSlider.value = kvp.Value.MaxSetValue;

                                controller.MMaxSlider.minValue = kvp.Value.MinValue;
                                controller.MMaxSlider.maxValue = kvp.Value.MaxValue;
                                controller.MMaxSlider.value = kvp.Value.MaxParamValue;

                                //当前值
                                controller.TNowSlider.value = kvp.Value.NowValue;

                                controller.MNowSlider.value = kvp.Value.NowValue;

                                controller.TNowSlider.minValue = kvp.Value.MinValue - increase;
                                controller.TNowSlider.maxValue = kvp.Value.MaxValue + increase;

                                controller.MNowSlider.minValue = kvp.Value.MinValue;
                                controller.MNowSlider.maxValue = kvp.Value.MaxValue;

                                controller.SSlider.value = kvp.Value.SmoothValue;
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