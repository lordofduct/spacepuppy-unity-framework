Shader "SPEditor/VisualSensorArcShader"
{
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;

			float _angle;
			float _tiltAngle;
			
			struct vert_out
			{
				float4 position : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			
			vert_out vert(appdata_base v)
			{
				float tBackAngle = 3.14159265 * 2 * (v.texcoord.y - 0.5);
				float tForwardAngle = 3.14159265 * 2 * (v.texcoord.y - 0.5) * _angle;
				
				float4x4 tBackRotation = float4x4(cos(-tBackAngle), 0, sin(-tBackAngle), 0,
												  0, 1, 0, 0,
												  -sin(-tBackAngle), 0, cos(-tBackAngle), 0,
												  0, 0, 0, 1);
				float4x4 tTiltRotation = float4x4(1, 0, 0, 0,
												  0, cos(_tiltAngle), -sin(_tiltAngle), 0,
												  0, sin(_tiltAngle), cos(_tiltAngle), 0,
												  0, 0, 0, 1);
				float4x4 tForwardRotation = float4x4(cos(tForwardAngle), 0, sin(tForwardAngle), 0,
													 0, 1, 0, 0,
													 -sin(tForwardAngle), 0, cos(tForwardAngle), 0,
													 0, 0, 0, 1);
				
				vert_out tOut;
				tOut.position = mul(UNITY_MATRIX_MVP, mul(tForwardRotation, mul(tTiltRotation, mul(tBackRotation, v.vertex))));
				tOut.texcoord = v.texcoord;
				
				return tOut;
			}
	
			float4 frag(vert_out f) : COLOR
			{
				return _Color;
			}
				
			ENDCG
		}
    }
    FallBack "Diffuse"
}
