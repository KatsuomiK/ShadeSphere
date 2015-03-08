Shader "Frame Synthesis/FadeSphere" {
	SubShader {
		Tags {"Queue"="Overlay-1"}
		
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct meshdata {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};
			
			struct v2f {
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			v2f vert(meshdata mesh)
			{
				v2f o;
				o.position = mul(UNITY_MATRIX_MVP, mesh.vertex);
				o.color = mesh.color;
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	} 
}
