using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using D2LiveManager;
using D2LiveManager.Live2DCubism3;
using OpenCVForUnity.UnityUtils.Helper;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using System.IO;
using Live2D.Cubism.Rendering;
using EventSystem = UnityEngine.EventSystems.EventSystem;
using UnityEditor;

namespace D2Live
{
    public class D2LiveController : MonoBehaviour
    {
        public DlibFaceLandmarkGetter DlibFaceLandmarkGetter;
        public WebCamTextureMatSourceGetter webCamTextureMatSourceGetter;
        public FaceLandmarkHeadPositionAndRotationGetter FaceLandmarkHeadPositionAndRotationGetter;
        public Live2DCubism3HeadRotationController Live2DCubism3HeadRotationController;
        public Live2DCubism3FaceAnimationController Live2DCubism3FaceAnimationController;
        public FaceAnimationController FaceAnimationController;
        public WebCamTextureToMatHelper WebCamTextureToMatHelper;
        public Toggle socketSwitch;
        public Dropdown dropDownItem;
        public Text statusText ;
        public RawImage rawImage;

        public CubismModel live2DCubism3Model;
        public D2LiveModelController D2LiveModelController;
        public CubismModel Model { get; set; }

        private D2LiveSocketManager mySocketServer;
        // Start is called before the first frame update
        void Start()
        {
            dropDownItem.options.Clear();
            Dropdown.OptionData temoData;
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                //给每一个option选项赋值
                temoData = new Dropdown.OptionData();
                temoData.text = WebCamTexture.devices[i].name;
                //temoData.image = sprite_ilist[i];
                dropDownItem.options.Add(temoData);
            }
            //初始选项的显示
            dropDownItem.captionText.text = WebCamTexture.devices[0].name;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnToggleSwitched()
        {
            if (socketSwitch.isOn == true)
            {
                dropDownItem.enabled = false;
                statusText.text = "等待连接";
                rawImage.texture = null;
                print("Starting Socket Server");


                webCamTextureMatSourceGetter.enabled = false;
                DlibFaceLandmarkGetter.enabled = false;
                FaceLandmarkHeadPositionAndRotationGetter.enabled = false;
                Live2DCubism3HeadRotationController.enabled = false;
                Live2DCubism3FaceAnimationController.enabled = false;
                FaceAnimationController.enabled = false;
                WebCamTextureToMatHelper.enabled = false;
                D2LiveModelController.enabled = true;
                webCamTextureMatSourceGetter.Stop();
                mySocketServer = new D2LiveSocketManager(statusText, socketSwitch, D2LiveModelController);
            }
            else
            {
                statusText.text = "已关闭";
                dropDownItem.enabled = true;
                print("Stopping Socket Server");
                webCamTextureMatSourceGetter.enabled = true;
                DlibFaceLandmarkGetter.enabled = true;
                FaceLandmarkHeadPositionAndRotationGetter.enabled = true;
                Live2DCubism3HeadRotationController.enabled = true;
                Live2DCubism3FaceAnimationController.enabled = true;
                FaceAnimationController.enabled = true;
                WebCamTextureToMatHelper.enabled = true;
                D2LiveModelController.enabled = false;
                webCamTextureMatSourceGetter.Play();
                mySocketServer.StopServer();
            }
        }

        public void OnDropdownSelected()
        {
            rawImage.texture = null;
            try
            {
                int n = dropDownItem.value;
                print("选择了:" + dropDownItem.captionText.text + "|" + n);
                webCamTextureMatSourceGetter.ChangeCameraTo(n);
                webCamTextureMatSourceGetter.Play();
            }
            catch (Exception err)
            {
                print("错误：" + err.ToString());
            }

        }
        public void OnModelSelectClick()
        {
            Debug.Log("Stasrt loading Live2D");
            var ModelFullName = Application.streamingAssetsPath + "/Model" + "/xccc" + "/xccc.model3.json";
            var model3Json = CubismModel3Json.LoadAtPath(ModelFullName, BuiltinLoadAssetAtPath);
            var model = model3Json.ToModel();

            UnityEngine.Object.Destroy(live2DCubism3Model.gameObject);
            live2DCubism3Model = model;

            CubismRenderController cubisumRenderController = model.GetComponent<CubismRenderController>();
            cubisumRenderController.SortingMode = CubismSortingMode.BackToFrontOrder;

            Live2DCubism3HeadRotationController.target = model;
            Live2DCubism3HeadRotationController.isModelChanged = true;
            Live2DCubism3FaceAnimationController.live2DCubism3Model = model;
            Live2DCubism3FaceAnimationController.isModelChanged = true;
            D2LiveModelController.live2DCubism3Model = model;
            D2LiveModelController.isModelChanged = true;
        }

        public static object LoadAsset(Type assetType, string absolutePath)
        {
            if (assetType == typeof(byte[]))
            {
                return File.ReadAllBytes(absolutePath);
            }
            else if (assetType == typeof(string))
            {
                return File.ReadAllText(absolutePath);
            }
            else if (assetType == typeof(Texture2D))
            {
                var texture = new Texture2D(1, 1);


                texture.LoadImage(File.ReadAllBytes(absolutePath));


                return texture;
            }


            // Fail hard.
            throw new NotSupportedException();
        }

        public static object BuiltinLoadAssetAtPath(Type assetType, string absolutePath)
        {
            if (assetType == typeof(byte[]))
            {
                return File.ReadAllBytes(absolutePath);
            }
            if (assetType == typeof(string))
            {
                return File.ReadAllText(absolutePath);
            }
            if (assetType == typeof(Texture2D))
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes(absolutePath));
                return texture2D;
            }
            throw new NotSupportedException();
        }
    }
}