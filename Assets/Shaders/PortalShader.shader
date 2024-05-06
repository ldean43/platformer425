// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PortalShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_ColorOpacity ("ColorOpacity", float) = 0.2
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
// 		_SpiralTex ("SpiralTexture", 2D) = "white" {}
// 		_SpiralOpacity ("SpiralOpacity", float) = 0.2
    }
    SubShader
    {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        // Use shader model 3.0 target, to get nicer looking lighting

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
            #include "UnityCG.cginc"
			#pragma target 3.0

			sampler2D _MainTex;
			float4 _Color;
			float _ColorOpacity;
// 			sampler2D _SpiralTex;
// 			float _SpiralOpacity;

			struct appdata_t {
				fixed4 vertex : POSITION;
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				fixed4 coords : TEXCOORD0;
				fixed4 circ_coords : TEXCOORD1;
			};

			v2f vert ( appdata_t input ) {
				const float4 lots_of_5s = float4( 0.5, 0.5, 0.5, 0.5 );
				v2f output;
				output.vertex = UnityObjectToClipPos( input.vertex );
				output.coords = ComputeScreenPos( output.vertex );
// 				output.coords /= ;
				output.circ_coords = input.vertex + lots_of_5s;

// 				output.vertex = UnityObjectToClipPos( input.vertex );
				return output;
			}

			fixed4 frag ( v2f input ) : SV_Target {
				const float borderIntensity = 6;
				const float smooth = 0.02;
				const float2 origin = float2( 0.5, 0.5 );
				const float targetRadius = 0.25;

				fixed2 ccd = input.circ_coords.xy;

// 				We compute opacity dynamically
				float2 adj = ccd - origin;
				float d_sq = adj.x * adj.x + adj.y * adj.y;

// 				If opacity should be zero, we return early, saving work (maybe lol).
				if ( d_sq > targetRadius )
					return fixed4( 0, 0, 0, 0 );

				float opacity = smoothstep( -targetRadius, smooth - targetRadius, -d_sq );

				fixed4 texcd = input.coords / input.coords.w;

// 				lens distortion
// 				texcd = texcd;
// 				return texcd;

// 				Spiral code (I didn't think it looked that good :/)
// 				float2 s_c = float2( _CosTime.y, _SinTime.y ) / 2.;
// 				float2 rotation = fixed2( adj.x * s_c.x - adj.y * s_c.y, adj.x * s_c.y + adj.y * s_c.x );
// 				fixed4 spiral = tex2D( _SpiralTex, rotation + origin );

// 				color filter
				float cop = _ColorOpacity; /*+ spiral * _SpiralOpacity;*/
				fixed4 output = tex2D( _MainTex, texcd.xy ) * ( 1 - cop ) + _Color * cop;

// 				relating to the border mask
				float bmask_val = min( ( 1 - opacity ) * opacity * borderIntensity, 1 );
				fixed3 border_mask = fixed3( bmask_val, bmask_val, bmask_val );

				output = fixed4( output.xyz, opacity )
// 				creates a little border around the portal
						+ fixed4( _Color.xyz * border_mask, 0 );
				return output;
			}
			ENDCG
		}
    }
    FallBack "Diffuse"
}
