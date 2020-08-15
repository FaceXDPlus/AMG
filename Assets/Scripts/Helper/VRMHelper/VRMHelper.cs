using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VRM;

namespace AMG
{
    public class VRMHelper : MonoBehaviour
    {
        [SerializeField] public SettingPanelController SettingPanelController;

        public ArrayList GetModelsFromAssets()
        {
            ArrayList returnArray = new ArrayList();
            string streamingAssetsPath = Application.streamingAssetsPath + "../../../Data/vrm";
            DirectoryInfo dir = new DirectoryInfo(streamingAssetsPath);
            foreach (FileInfo file in dir.GetFiles("*.vrm", SearchOption.AllDirectories))
            {
                returnArray.Add(file.Directory.Name + "/" + file.Name.Substring(0, file.Name.Length - 4));
            }
            return returnArray;
        }

        public void GetModelFromName(string name, GameObject parent, GameObject MouseObject)
        {
            try
            {
                string ModelPath = Application.streamingAssetsPath + "../../../Data/vrm/" + name + ".vrm";
                if (System.IO.File.Exists(ModelPath))
                {
                    var bytes = File.ReadAllBytes(ModelPath);
                    var context = new VRMImporterContext();
                    context.ParseGlb(bytes);
                    var metaObject = context.ReadMeta(false);
                    /*Debug.LogFormat("meta: title:{0}", metaObject.Title);
                    Debug.LogFormat("meta: version:{0}", metaObject.Version);
                    Debug.LogFormat("meta: author:{0}", metaObject.Author);
                    Debug.LogFormat("meta: exporterVersion:{0}", metaObject.ExporterVersion);*/
                    var modelD = Path.GetDirectoryName(ModelPath);
                    context.LoadAsync(() => {
                        OnLoaded(context, parent, metaObject, modelD, MouseObject);
                    }, OnError);
                }
                else
                {
                    throw new Exception(Globle.LangController.GetLang("LOG.FileUnisset"));
                }
            }
            catch (Exception err)
            {
                Globle.AddDataLog("Main", Globle.LangController.GetLang("LOG.ModelAddationException", err.Message));
            }
        }
        protected virtual void OnLoaded(VRMImporterContext context, GameObject parent, VRMMetaObject metaObject, string path, GameObject MouseObject)
        {
            var root = context.Root;
            context.ShowMeshes();

            root.transform.SetParent(parent.transform, false);
            var meta = root.GetComponent<VRMMeta>();
            meta.transform.localPosition = new Vector3(0, 0, 0);
            SetupTarget(meta, root, metaObject, path, MouseObject);
        }

        protected virtual void OnError(Exception e)
        {
            Debug.LogError(e);
            //Dispose();
        }

        private void SetupTarget(VRMMeta meta, GameObject root, VRMMetaObject metaObject, string path, GameObject MouseObject)
        {
            root.name = metaObject.Title + "(" + Globle.ModelNum.ToString() + ")";
            Globle.ModelNum++;
            Globle.ModelList.Add(root);
            SettingPanelController.ResetModelSelectionDropdown();
            var modelController = root.gameObject.AddComponent<VRMModelController>();
            modelController.DisplayName = root.name;
            modelController.ModelPath = path;
            modelController.MouseObject = MouseObject;
            modelController.SettingPanelController = SettingPanelController;
        }
    }
}
