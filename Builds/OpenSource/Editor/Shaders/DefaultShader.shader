Shader "SPEditor/DefaultShader"
{
	SubShader {
		Pass {
			BindChannels{
				Bind "Color", color
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			Color(1, 1, 1, 1)
		}
	}
}
