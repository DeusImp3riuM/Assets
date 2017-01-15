    Shader "Custom/Overlay" {
        Properties{
            _MainTex("Texture", 2D) = "" {}
            _str("Overlay Strength", Float) = 0.45
        }
     
        SubShader{
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
     
            GrabPass{}
     
            Pass{
                CGPROGRAM
     
    #pragma vertex vert
    #pragma fragment frag
     
    #include "UnityCG.cginc"
     
                sampler2D _GrabTexture;
                sampler2D _MainTex;
                float _str;
     
                struct appdata_t {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
     
                struct v2f {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float4 projPos : TEXCOORD1;
                };
     
                float4 _MainTex_ST;
     
                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.projPos = ComputeScreenPos(o.vertex);
                    return o;
                }
     
                half4 frag(v2f i) : COLOR{
                    i.projPos /= i.projPos.w;
                    half4 base = tex2D(_GrabTexture, float2(i.projPos.x, 1-i.projPos.y));
                    half gray = (base.r + base.g + base.b) / 3;
                    half4 overlay = tex2D(_MainTex, i.uv);
     
                    float4 effect = lerp(1 - (2 * (1 - base)) * (1 - overlay), (2 * base) * overlay, step(gray, 0.5f));
                 
                    return lerp(base, effect, (overlay.w * _str));
                }
     
                ENDCG
            }
        }
    }
