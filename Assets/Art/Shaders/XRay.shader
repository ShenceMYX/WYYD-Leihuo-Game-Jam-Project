Shader "yx/XRay"
{
    Properties
    {
        _XRayColor ("X-Ray Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            Name "XRay"
            
            //开启混合
            Blend SrcAlpha OneMinusSrcAlpha
            //关闭深度写入，避免深度缓冲混乱导致第2个pass被遮挡部分写入颜色缓冲区
            ZWrite Off
            //z值大于深度缓冲为通过 
            ZTest Greater

            Stencil
            {
                //参考值1，小于lighting pass的参考值
                Ref 1
                //参考值1大于等于模板缓冲区值时，pass模板测试
                Comp GEqual
                //通过模板测试，替换模板缓冲区为参考值1
                Pass Replace
                //默认值。未通过深度检测，保持Stencil buffer值
                //ZFail Keep
            }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 _XRayColor;

            struct a2v
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                return _XRayColor;
            }

            ENDCG
        }
    }
}
