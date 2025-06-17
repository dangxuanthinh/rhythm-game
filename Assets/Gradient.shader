Shader "Sprites/VerticalGradient"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _TopColor("Top Color", Color) = (1,1,1,1)
        _BottomColor("Bottom Color", Color) = (0,0,0,1)
        _Color("Tint", Color) = (1,1,1,1) // Used by SpriteRenderer.color
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Sprite" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TopColor;
            fixed4 _BottomColor;
            fixed4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float gradientT : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.gradientT = v.uv.y; // vertical gradient based on Y
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 gradientColor = lerp(_BottomColor, _TopColor, i.gradientT);
                fixed4 finalColor = tex * gradientColor * _Color;
                return finalColor;
            }
            ENDCG
        }
    }
}
