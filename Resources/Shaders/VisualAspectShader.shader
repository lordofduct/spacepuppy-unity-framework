Shader "SPEditor/VisualAspectShader"
{
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Off
		ZWrite Off
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
	
			float4 _colorSolid;
			float _colorAlpha;
			
			struct vert_out
			{
				float4 position : POSITION;
			};
			
			vert_out vert(appdata_base v)
			{
				vert_out tOut;
				tOut.position = UnityObjectToClipPos(v.vertex);
				
				return tOut;
			}
	
			float4 frag(vert_out f) : COLOR
			{
				return float4(_colorSolid.rgb, _colorAlpha);
			}
				
			ENDCG
		}

		Blend One Zero
		ZTest LEqual
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
	
			float4 _colorSolid;
			
			struct vert_out
			{
				float4 position : POSITION;
			};
			
			vert_out vert(appdata_base v)
			{
				vert_out tOut;
				tOut.position = UnityObjectToClipPos(v.vertex);
				
				return tOut;
			}
	
			float4 frag(vert_out f) : COLOR
			{
				return _colorSolid;
			}
				
			ENDCG
		}
    }
    FallBack "Diffuse"
}
