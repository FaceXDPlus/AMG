using UnityEngine;
using System.Collections;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

namespace AMG
{
	public class CubismBreathController : MonoBehaviour
	{
		[SerializeField, Range(0.1f, 20f)]
		public float inTime = 1.5f;
		[SerializeField, Range(0.1f, 20f)]
		public float outTime = 1.5f;
		[SerializeField, Range(0.1f, 20f)]
		public float offTime = 0.5f;

		public float sendValue;
		public int phaseCode;

		public CubismParameterBlendMode mode = CubismParameterBlendMode.Override;

		/// <summary>
		/// Eye blink parameters cache.
		/// </summary>
		private CubismParameter[] Destinations { get; set; }

		/// <summary>
		/// Control over whether output should be evaluated.
		/// </summary>
		private Phase CurrentPhase { get; set; }

		/// <summary>
		/// Time until next eye blink.
		/// </summary>
		private float T { get; set; }

		/// <summary>
		/// Refreshes controller. Call this method after adding and/or removing <see cref="CubismEyeBlinkParameter"/>s.
		/// </summary>
		public void Refresh()
		{
			var model = this.FindCubismModel();


			// Fail silently...
			if (model == null)
			{
				return;
			}


			// Cache destinations.
			var tags = model
				.Parameters
				.GetComponentsMany<CubismBreathParameter>();


			Destinations = new CubismParameter[tags.Length];


			for (var i = 0; i < tags.Length; ++i)
			{
				Destinations[i] = tags[i].GetComponent<CubismParameter>();
			}
		}

		/// <summary>
		/// Called by Unity. Makes sure cache is initialized.
		/// </summary>
		private void Start()
		{
			// Initialize cache.
			Refresh();
			T = Time.time;
		}


		/// <summary>
		/// Called by Unity. Updates controller.
		/// </summary>
		private void LateUpdate()
		{
			var nowT = Time.time;
			var tilTime = nowT - T;
			if (CurrentPhase == Phase.inBreath)
			{
				phaseCode = 1;
				if (tilTime > inTime)
				{
					T = T + inTime;
					CurrentPhase = Phase.outBreath;
					LateUpdate();
					return;
				}
				sendValue = inOutQuad(tilTime, 0, 1, inTime);
				Destinations.BlendToValue(mode, sendValue);
			}
			else if (CurrentPhase == Phase.outBreath)
			{
				phaseCode = 2;
				if (tilTime > outTime)
				{
					T = T + outTime;
					CurrentPhase = Phase.Idling;
					LateUpdate();
					return;
				}
				sendValue = inOutQuad(outTime - tilTime, 0, 1, outTime);
				Destinations.BlendToValue(mode, sendValue);
			}
			else
			{
				phaseCode = 3;
				if (tilTime > offTime)
				{
					T = T + offTime;
					CurrentPhase = Phase.inBreath;
					LateUpdate();
					return;
				}
				sendValue = 0;
				Destinations.BlendToValue(mode, sendValue);
			}
		}

		private float inOutQuad(float t, float b, float c, float total)
		{
			t = t / total * 2;
			if (t < 1)
			{
				return c / 2 * t * t + b;
			}
			else
			{
				return -c / 2 * ((t - 1) * (t - 3) - 1) + b;
			}
		}

		/// <summary>
		/// Internal states.
		/// </summary>
		private enum Phase
		{
			/// <summary>
			/// Idle state.
			/// </summary>
			Idling,

			/// <summary>
			/// State when closing eyes.
			/// </summary>
			inBreath,

			/// <summary>
			/// State when opening eyes.
			/// </summary>
			outBreath
		}
	}
}