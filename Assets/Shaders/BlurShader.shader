Shader "Custom/BlurShader" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_BlurSize("Blur Size", Int) = 1
	}

	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc" // required for v2f_img

			// Properties
			sampler2D _MainTex;
			const int _BlurSize;
			uniform fixed4 _MainTex_TexelSize;

			const float PI = 90;
			const float realPI = 3.14195265;

			float4 blur ( int blur_sz, fixed4 base, fixed2 uv ) {
				int fac = 0;
				float blur_sz_ov_2 = blur_sz / 2.;

				for ( int i = -blur_sz + 1; i < blur_sz - 1; i++ ) {
					int absi = ceil( sqrt( blur_sz_ov_2*blur_sz_ov_2 - i*i ) );
					int l = ( blur_sz - abs( i ) );
					for ( int j = -absi + 1; j < absi - 1; j++ ) {
						int k = l * ( absi - abs( j ) );
						base += k * tex2D( _MainTex, uv + fixed2( i*_MainTex_TexelSize.x, j*_MainTex_TexelSize.y ) );
						fac += k;
					}
				}

				base /= fac;
				return base;
			}

			float4 frag ( v2f_img input ) : COLOR {
				int blur_sz = round( ( _SinTime.w + 1 ) * _BlurSize / 2 ) + 3;
				float4 base = 0;

				// my attempt at a very basic circular box shader
				base = blur( blur_sz, base, input.uv );

				// sample texture for color
				return base;
			}
			ENDCG
		}
	}
}
