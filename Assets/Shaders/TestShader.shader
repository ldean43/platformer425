// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
//      Tags { "Queue" = "Geometry" }
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
            #include "UnityCG.cginc"

			struct appdata_t {
				fixed4 vertex : POSITION;
			};

			struct v2f {
				fixed2 coords : COLOR;
				fixed4 vertex : SV_POSITION;
			};

			v2f vert ( appdata_t input ) {
				v2f output;
				output.vertex = UnityObjectToClipPos( input.vertex );
				output.coords = output.vertex.zw;
				return output;
			}

			fixed4 frag ( v2f input ) : SV_Target {
				fixed4 output = fixed4( input.coords.x, input.coords.y, 0.0, 1.0 );
				return output;
			}
			ENDCG
		}

// 		Pass {
// 			GLSLPROGRAM
// 			#ifdef VERTEX // here begins the vertex shader
//
// 			varying vec2 Coord;
//
// 			void main () {
// 				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
// 				Coord = vec2( gl_Position.xy );
// 			}
//
// 			#endif // here ends the definition of the vertex shader
//
//
// 			#ifdef FRAGMENT // here begins the fragment shader
//
// 			varying vec2 Coord;
//
// 			void main () {
// 				gl_FragColor = vec4( Coord.x, Coord.y, _SinTime, 1.0 );
// 			}
//
// 			#endif // here ends the definition of the fragment shader
//
// 			ENDGLSL // here ends the part in GLSL
// 		}
    }
//     FallBack "Diffuse"
}
