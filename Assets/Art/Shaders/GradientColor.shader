Shader "yx/GradientColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"{}
        _Tint ("Tint", Color) = (1,1,1,1)
        [Gamma] _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness (Metallic.a)", Range(0,1)) = 0.5

        _GradientColor ("Gradient Color", Color) = (0,0,0,1)
        _GradientIntensity ("Gradient Intensity", Range(0,1)) = 0.5
        _GradientIntensity2 ("Gradient Intensity2", Range(0,1)) = 0.5

        _XRayColor ("X-Ray Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        UsePass "yx/XRay/XRay"
        Pass
        {
            Tags {"LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
                float4 relativeWorldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Tint;
            float _Metallic, _Smoothness;

            fixed4 _GradientColor; float _GradientIntensity;float _GradientIntensity2;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.relativeWorldPos = o.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1));
                return o;
            }

            half3 DirectLightDiffuse(float kd, half3 albedo, float NdotV, float NdotL, float LdotH, float perceptualRoughness)
            {
                //使用迪士尼模型时需要 * albedo / PI, 而Unity为了将整体效果提亮选择不除PI
                half3 disneyDiffuse = DisneyDiffuse(NdotV, NdotL, LdotH, perceptualRoughness) * albedo /* / UNITY_PI */;

                half3 result = kd * disneyDiffuse * NdotL * _LightColor0.rgb;
                return result;
            }

            half3 DirectLightSpecular(half3 albedo, float NdotV, float NdotL, float LdotH, float NdotH, float roughness)
            {
                //D项（GGX）法线分布函数
                float D = GGXTerm (NdotH, roughness);

                //G项 几何函数，微表面遮蔽效果
                float G = SmithJointGGXVisibilityTerm (NdotL, NdotV, roughness);

                //F项 菲涅尔
                half3 specColor = lerp (unity_ColorSpaceDielectricSpec.rgb, albedo, _Metallic);
                half3 F = FresnelTerm(specColor, LdotH);

                //Cook - Torrance BRDF
                half3 result = D * G * F /* / (4 * NdotL * NdotV) */ * NdotL * _LightColor0;
                //为了保证能量守恒，因为irectLightDiffuse乘了一个PI，这里也要乘一个PI
                result *= UNITY_PI;

                return result;
            }

            half3 IndirectLightDiffuse(float kd, half3 albedo, half3 worldNormal)
            {
                half3 iblDiffuse = ShadeSH9(half4(worldNormal, 1));
                half3 result = kd * albedo * iblDiffuse;
                return result;
            }

            half3 IndirectLightSpecular(float kd, half3 albedo, float NdotV, half3 R, float roughness, float perceptualRoughness)
            {
                //1. 使用粗糙度来采样cubemap的mipmap
                //使Unity的粗糙度与mipmap采样呈非线性关系
                float mipRoughness = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);

                //mipRoughness(0~1)映射到实际的mip层级(0~6) 粗糙度x6得到最终的mip级别 UNITY_SPECCUBE_LOD_STEPS = 6表示反射探针的mip级别有六级   
                float mip = perceptualRoughness * UNITY_SPECCUBE_LOD_STEPS;

                //UNITY_SAMPLE_TEXCUBE_LOD是采样函数
                //cubemap采样是三线性差值：1.从两张临近的mipmap各做一次二线性差值 2.将结果再进行一次差值
                half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, R, mip);

                //将颜色从HDR解码，因为rgbm是HDR格式，颜色会超过1
                half3 specular = DecodeHDR(rgbm, unity_SpecCube0_HDR);

                //2. 使用SurfReduction来拟合间接光照在粗糙物体上过亮的问题
                //Unity使用拟合函数来实时计算查找图（LUT）的数据
                float surfReduction = 1 / (roughness * roughness + 1);

                //unity考虑掠射镜面反射(grazing specular reflections)   
                float oneMinusReflectivity = kd;
                float grazingTerm = saturate(_Smoothness + (1-oneMinusReflectivity));
                half3 F0 = lerp (unity_ColorSpaceDielectricSpec.rgb, albedo, _Metallic);
                //F0:视线与物体法线夹角为0的情况, F90:视线与物体法线夹角为90度的情况, cosA:NdotV视线与法线的夹角 
                half3 fresnelTerm = FresnelLerp(F0, grazingTerm, NdotV);

                half3 result = specular * surfReduction * fresnelTerm;

                return result;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                //准备PBR shader计算所需参数
                half3 N = normalize(i.worldNormal);
                half3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                half3 L = _WorldSpaceLightPos0;
                half3 H = normalize(L + V);
                half3 R = reflect(-V, N);

                float NdotV = max(0, dot(N, V));
                float NdotL = max(0, dot(N, L));
                float LdotH = max(0, dot(L, H));
                float NdotH = max(0, dot(N, H));

                fixed4 albedo = tex2D(_MainTex, i.uv) * _Tint;

                float relativeYPos =  i.relativeWorldPos.y * 0.5 + 0.5;
                float lp = smoothstep(relativeYPos, 1, _GradientIntensity);
                albedo = lerp(albedo, _GradientColor, lp+_GradientIntensity2);

                float perceptualRoughness = 1 - _Smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
                roughness = max(roughness, 0.002);//roughness为1时仍有高光点

                float kd = OneMinusReflectivityFromMetallic(_Metallic);

                half3 directLightColor = DirectLightDiffuse(kd, albedo, NdotV, NdotL, LdotH, perceptualRoughness)
                                        + DirectLightSpecular(albedo, NdotV, NdotL, LdotH, NdotH, roughness);
                half3 indirectLightColor = IndirectLightDiffuse(kd, albedo, N)
                                        + IndirectLightSpecular(kd, albedo, NdotV, R, roughness, perceptualRoughness);

                fixed4 c = fixed4(directLightColor + indirectLightColor, 1);
                
                return c;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
