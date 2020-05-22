using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AMG
{
    public class AMGConnectionLostPNGController : MonoBehaviour
    {
        public UnityEngine.UI.Image Im;
        private float fps = 20f;
        private Dictionary<string, Texture2D> Texturelist = new Dictionary<string, Texture2D>();
        private float time;
        public void Init()
        {
            string path = Application.streamingAssetsPath + "/gif/ConnectionLost/";
            if (Directory.Exists(path))
            {
                DirectoryInfo direction = new DirectoryInfo(path);

                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                byte[] fileData;

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".png"))
                    {
                        fileData = File.ReadAllBytes(files[i].ToString());
                        string name = Path.GetFileNameWithoutExtension(files[i].ToString());
                        Texture2D Texture2DA = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                        Texture2DA.LoadImage(fileData);
                        Texturelist.Add(name, Texture2DA);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (Texturelist.Count > 0)
            {
                time += Time.deltaTime;
                int index = (int)(time * fps) % Texturelist.Count;
                if (Im != null)
                {
                    Im.sprite = Sprite.Create(Texturelist[index.ToString()], new Rect(0, 0, Texturelist[index.ToString()].width, Texturelist[index.ToString()].height), new Vector2(0.5f, 0.5f));
                    var rt = (RectTransform)Im.transform;
                    rt.sizeDelta = new Vector2(Texturelist[index.ToString()].width / 10, Texturelist[index.ToString()].height / 10);
                }
            }
        }
    }
}
