Shader "MaterialUI/ShadowedRipple"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Softening("Softening", Float) = 0
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			Pass
			{
				Name "Shadow"

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				half _Softening;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.worldPosition = IN.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
					OUT.texcoord = IN.texcoord;

					#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
					#endif

					OUT.color = IN.color * _Color;
					return OUT;
				}

				sampler2D _MainTex;
				float4 _MainTex_TexelSize;

				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = (tex2D(_MainTex, IN.texcoord + float2(0, _MainTex_TexelSize.y * 2)) + _TextureSampleAdd) * half4(IN.color.r, IN.color.g, IN.color.b, 1);
					color.a = smoothstep(0.35, 0.6, color.a);

					float scale = 1;

					color.r = 1 - color.r;
					color.g = 1 - color.g;
					color.b = 1 - color.b;
					color.a *= 0.15;

					color.a *= IN.color.a;

					bool isClipped = false;

					if (IN.worldPosition.y <= _ClipRect.y)
					{
						color.a *= 1 - (distance(IN.worldPosition.y / scale, _ClipRect.y / scale) * 0.2);

						color.a *= step((_ClipRect.x / scale - IN.worldPosition.x / scale > 0), step(distance(IN.worldPosition.x / scale, _ClipRect.x / scale), 3));
						color.a *= step((IN.worldPosition.x / scale - _ClipRect.z / scale > 0), step(distance(IN.worldPosition.x / scale, _ClipRect.z / scale), 3));

						isClipped = true;
					}
					else if (IN.worldPosition.y >= _ClipRect.w)
					{
						color.a *= 1 - (distance(IN.worldPosition.y / scale, _ClipRect.w / scale) * 0.4);

						color.a *= step((_ClipRect.x / scale - IN.worldPosition.x / scale > 0), step(distance(IN.worldPosition.x / scale, _ClipRect.x / scale), 3));
						color.a *= step((IN.worldPosition.x / scale - _ClipRect.z / scale > 0), step(distance(IN.worldPosition.x / scale, _ClipRect.z / scale), 3));

						isClipped = true;
					}

					if (IN.worldPosition.x <= _ClipRect.x)
					{
						color.a *= 1 - (distance(IN.worldPosition.x / scale, _ClipRect.x / scale) * 0.3);

						isClipped = true;
					}
					else if (IN.worldPosition.x >= _ClipRect.z)
					{
						color.a *= 1 - (distance(IN.worldPosition.x / scale, _ClipRect.z / scale) * 0.3);

						isClipped = true;
					}

					if (!isClipped)
					{
						half4 foregroundColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * half4(IN.color.r, IN.color.g, IN.color.b, 1);
						foregroundColor.a = smoothstep(0.5 - _Softening * 0.1, 0.5 + _Softening * 0.1, foregroundColor.a);

						if (foregroundColor.a >= 1)
						{
							return (0, 0, 0, 0);
						}
					}

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					return color;
				}
				ENDCG
			}

			Pass
			{
				Name "Ripple"

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
				};

				fixed4 _Color;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				half _Softening;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.worldPosition = IN.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
					OUT.texcoord = IN.texcoord;

					#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
					#endif

					OUT.color = IN.color * _Color;
					return OUT;
				}

				sampler2D _MainTex;
				float4 _MainTex_TexelSize;

				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) *half4(IN.color.r, IN.color.g, IN.color.b, 1);
					color.a = smoothstep(0.5 - _Softening * 0.1, 0.5 + _Softening * 0.1, color.a);

					color.a *= IN.color.a;

					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					return color;
				}
				ENDCG
			}
		}
}