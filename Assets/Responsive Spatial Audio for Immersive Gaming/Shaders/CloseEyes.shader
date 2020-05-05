// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CloseEyes" {
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_CutOff("CutAwayDirction", Range(0.0, 1)) = 0.0
		_Color("Colour", 2D) = "white" {}
	}

	SubShader
	{
		Pass
	{
	CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment simplefrag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	fixed _CutOff;
	fixed4 _Color;

	fixed4 simplefrag(v2f_img i) : SV_Target
	{
		if (0.5 - abs(i.uv.y - 0.5) < abs(_CutOff) * 0.5)
			return _Color;
		
	return tex2D(_MainTex, i.uv);
	}

		ENDCG
	}
	}
		FallBack off
}
