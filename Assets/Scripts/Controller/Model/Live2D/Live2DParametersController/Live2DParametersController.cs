using Live2D.Cubism.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMG
{
	public class Live2DParametersController : MonoBehaviour
	{
		public static JObject getParametersJson(string jsonDataPath)
		{
			System.IO.StreamReader file = System.IO.File.OpenText(jsonDataPath);
			JsonTextReader reader = new JsonTextReader(file);
			JObject jsonParams = (JObject)JToken.ReadFrom(reader);
			return jsonParams;
		}

		public static CubismParameter getParametersFromJson(string name, JObject jsonData, CubismModel live2DCubism3Model)
		{
			foreach (var Parameters in jsonData)
			{
				//Debug.Log("Checking " + Parameters.Key.ToString());
				if (Parameters.Key == name)
				{
					foreach (var data in Parameters.Value)
					{
						if (live2DCubism3Model.Parameters.FindById(data.ToString()) != null)
						{
							return live2DCubism3Model.Parameters.FindById(data.ToString());
						}
					}
				}
			}
			return null;
		}
	}
}
