Shader "SPEditor/VisualSensorLineShader"
{
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
#pragma exclude_renderers gles

			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;

			float4 frag() : COLOR
			{
				return _Color;
			}
				
			ENDCG
		}
    }
    FallBack "Diffuse"
}
