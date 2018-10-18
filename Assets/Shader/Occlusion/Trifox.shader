Shader "Dissolve/Trifox"
{
	Properties
	{
		//  [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_MyColor ("Color", Color) = (1,1,1,1)
		_OffsetXRange("OffsetXRange", Range(-20,60)) = 0
		_OffsetZRange("OffsetZRange", Range(-20,60)) = 0	
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "gray" {}		
		_ScreenSpaceMask("Screen Space Mask", 2D) = "white" {}
		_WorkDistance("Work Distance", Float) = 20
		_PlayerPos("Player Pos", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			sampler2D _ScreenSpaceMask;
			float _WorkDistance;
			float4 _PlayerPos;
			///
			uniform float _OffsetXRange; 
			uniform float _OffsetZRange;
			float4 _MyColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float toCamera = distance(i.worldPos, _WorldSpaceCameraPos.xyz);
				float playerToCamera = distance(_PlayerPos.xyz, _WorldSpaceCameraPos.xyz)-_OffsetZRange;

				fixed4 col = tex2D(_MainTex, i.uv)*_MyColor;

				if(toCamera < playerToCamera/*i.worldPos.z<_PlayerPos.z-_OffsetZRange*/)
				{
					if(i.worldPos.x<_PlayerPos.x+_OffsetXRange &&i.worldPos.x>_PlayerPos.x-_OffsetXRange )
					{
					float2 wcoord = (i.screenPos.xy / i.screenPos.w);
					float mask = tex2D(_ScreenSpaceMask, wcoord).r;
					float gradient = tex2D(_NoiseTex, i.uv).r;
					clip(gradient - mask + (toCamera - _WorkDistance) / _WorkDistance);
					}
				}

				return col;
			}
			ENDCG
		}
	}
}