Shader "Custom/ScreenGreenRed" 
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BlendTex("Blend Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_ColorRed("Tint", Color) = (0.6,0.6,0,1)
		_ColorGreen("Tint", Color) = (0,1,0,1)
		_LeftRight("Bool for left right", Range(0,1)) = 1.0
		_FrontBack("Bool for front back", Range(0,1)) = 1.0
		_Pitch("Pitch", Range(0,1)) = 1.0
	}
		SubShader
		{
			Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BlendTex;
			sampler2D _NoiseTex;
			half _GreenRed;
			float4 _ColorRed;
			float4 _ColorGreen;
			half _LeftRight;
			half _FrontBack;
			half _Pitch;
			half _RandomValue;
			half _NoiseXSpeed;
			half _NoiseYSpeed;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed4 finalColor = (1,1,1,1);
				fixed4 renderTex = tex2D(_MainTex, i.uv);


				half2 noiseUV = half2(i.uv.x*_Pitch + (_RandomValue * _SinTime.z * _NoiseXSpeed), i.uv.y*_Pitch + (_Time.x * _NoiseYSpeed));

				half2 shookUV = i.uv;
				half shookFac = length(shookUV - 0.5) * 2;
				shookUV = lerp(shookUV, float2(0.5, 0.5), shookFac * _RandomValue * 0.005 * _Pitch);


				fixed4 blendTex = tex2D(_BlendTex, shookUV);
				
				fixed4 noiseTex = tex2D(_NoiseTex, noiseUV);

				if (blendTex.b > 0)
				{
					if (_LeftRight && _Pitch >= 1)
					{
						if (i.uv.x > 0.5)
						{
							finalColor = lerp(_ColorRed, _ColorGreen, 1);
						}
						else
						{
							finalColor = lerp(_ColorGreen, _ColorRed, (2 - _Pitch));
						}
					}

					if (!_LeftRight && _Pitch >= 1)
					{
						if (i.uv.x < 0.5)
						{
							finalColor = lerp(_ColorRed, _ColorGreen, 1);
						}
						else
						{
							finalColor = lerp(_ColorGreen, _ColorRed, (2 - _Pitch));
						}
					}

					if (!_FrontBack)
					{
						if (_LeftRight)
						{
							if (i.uv.x > 0.5)
							{
								finalColor = lerp(_ColorRed, _ColorGreen, _Pitch);
							}
						}

						else
						{
							if (i.uv.x < 0.5)
							{
								finalColor = lerp(_ColorRed, _ColorGreen, _Pitch);
							}
						}
					}
					return lerp (renderTex, finalColor * 2.5 * blendTex.g * noiseTex, 0.7);
				}
				return renderTex * finalColor;

			}
			ENDCG
		}
}
	FallBack Off
}
