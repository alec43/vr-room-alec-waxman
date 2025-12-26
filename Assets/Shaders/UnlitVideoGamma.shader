Shader "Custom/UnlitVideoGamma"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Lighting Off
        Cull Off
        ZWrite Off
        ZTest LEqual   // <-- Important fix

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float4 c = tex2D(_MainTex, i.uv);
                c.rgb = pow(c.rgb, 2.2);  // gamma correction
                return c;
            }
            ENDCG
        }
    }
}
