Shader "Emir/URP/ToonMain"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Header(Toon Shading)]
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSoftness("Shadow Softness", Range(0, 1)) = 0.05
        _ShadowColor("Shadow Color", Color) = (0.7, 0.7, 0.8, 1)
        
        [Space(10)]
        [Header(Rim Lighting)]
        _RimPower("Rim Power", Range(0, 10)) = 2
        _RimIntensity("Rim Intensity", Range(0, 2)) = 1
        _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Header(Specular)]
        _SpecularGloss("Specular Gloss", Range(0, 1)) = 0.5
        _SpecularIntensity("Specular Intensity", Range(0, 2)) = 1
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Header(Outline)]
        _OutlineWidth("Outline Width", Range(0, 0.5)) = 0.005
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        
        [Space(10)]
        [Header(Surface Options)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2
        [Toggle] _ZWrite("Z Write", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        // Main Pass
        Pass
        {
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull [_Cull]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half _ShadowThreshold;
                half _ShadowSoftness;
                half4 _ShadowColor;
                half _RimPower;
                half _RimIntensity;
                half4 _RimColor;
                half _SpecularGloss;
                half _SpecularIntensity;
                half4 _SpecularColor;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float fogCoord : TEXCOORD3;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionHCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = input.uv;
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample base texture
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 baseColor = baseMap * _BaseColor;
                
                // Calculate lighting vectors
                float3 normalWS = normalize(input.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.positionWS);
                float3 halfDir = normalize(lightDir + viewDir);
                
                // Dot products
                float NdotL = dot(normalWS, lightDir);
                float NdotV = dot(normalWS, viewDir);
                float NdotH = dot(normalWS, halfDir);
                
                // Remap NdotL to 0-1 range like in working shader
                NdotL = NdotL * 0.5 + 0.5;
                
                // Get shadow coordinates properly
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                float shadow = MainLightRealtimeShadow(shadowCoord);
                
                // Toon shading calculation
                float shadowStep = smoothstep(_ShadowThreshold - _ShadowSoftness, 
                                            _ShadowThreshold + _ShadowSoftness, NdotL);
                
                // Specular calculation
                float specularStep = smoothstep((1.0 - _SpecularGloss * 0.05) - 0.05, 
                                              (1.0 - _SpecularGloss * 0.05) + 0.05, NdotH);
                
                // Rim lighting calculation
                float rimStep = smoothstep((1.0 - _RimPower * 0.1) - _RimIntensity * 0.5, 
                                         (1.0 - _RimPower * 0.1) + _RimIntensity * 0.5, 
                                         0.5 - NdotV);
                
                // Diffuse lighting
                half3 diffuse = _MainLightColor.rgb * baseColor.rgb * shadowStep * shadow;
                
                // Specular highlights
                half3 specular = _SpecularColor.rgb * shadow * shadowStep * specularStep;
                
                // Rim lighting + ambient
                half3 ambient = rimStep * _RimColor.rgb + SampleSH(normalWS) * baseColor.rgb;
                
                // Combine all lighting
                half3 finalColor = diffuse + ambient + specular;
                
                // Apply fog
                finalColor = MixFog(finalColor, input.fogCoord);
                
                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
        
        // Outline Pass
        Pass
        {
            Name "ToonOutline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                half4 _OutlineColor;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float fogCoord : TEXCOORD0;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                // Expand vertices along normals for outline
                float3 positionWS = vertexInput.positionWS + normalInput.normalWS * _OutlineWidth * 0.1;
                output.positionHCS = TransformWorldToHClip(positionWS);
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                float3 finalColor = MixFog(_OutlineColor.rgb, input.fogCoord);
                return half4(finalColor, _OutlineColor.a);
            }
            ENDHLSL
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}