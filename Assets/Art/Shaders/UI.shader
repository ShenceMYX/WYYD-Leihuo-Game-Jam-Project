Shader "yx/UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        //_Ref ("Stencil Ref", int) = 0
        //[Enum(UnityEngie.Rendering.CompareFunction)] _StencilComp ("Stencil Comp", int) = 0
        //[Enum(UnityEngie.Rendering.StencilOp)] _StencilOp ("Stencil OP", int) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        ZWrite Off
        ZTest Always

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ UNITY_UI_CLIP_RECT

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float4 vertex : TEXCOORD1;
            };

            sampler2D _MainTex; float4 _MaiTex_ST;
            float4 _ClipRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.vertex = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                c *= i.color;

                #if UNITY_UI_CLIP_RECT
                    //方法一：利用if来理解原理
                    //if(_ClipRect.x < i.vertex.x && i.vertex.x < _ClipRect.z && _ClipRect.y < i.vertex.y && i.vertex.y < _ClipRect.w) return 1;
                    //else return 0;
                    //方法二：利用step优化掉if指令
                    //return step(_ClipRect.x, i.vertex.x) * step(i.vertex.x, _ClipRect.z) * step(_ClipRect.y, i.vertex.y) * step(i.vertex.y, _ClipRect.w);
                    //方法三：优化step的数量
                    //fixed2 rect = step(_ClipRect.xy, i.vertex.xy) * step(i.vertex.xy, _ClipRect.zw);
                    //c.a *= rect.x * rect.y;
                    //方法四：用Unity自带函数
                    c.a *= UnityGet2DClippig(i.vertex, _ClipRect);
                #endif
             
               
                return c;
            }
            ENDCG
        }
    }
}
