// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9420,x:34312,y:32573,varname:node_9420,prsc:2|emission-1391-OUT,alpha-4407-OUT;n:type:ShaderForge.SFN_Tex2d,id:1664,x:32655,y:32644,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_1664,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_If,id:9812,x:32259,y:33273,varname:node_9812,prsc:2|A-5000-OUT,B-2430-R,GT-7703-OUT,EQ-7703-OUT,LT-1913-OUT;n:type:ShaderForge.SFN_Vector1,id:7703,x:31877,y:33485,varname:node_7703,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:1913,x:31877,y:33557,varname:node_1913,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:2430,x:31877,y:33309,ptovrint:False,ptlb:noise_rongjie,ptin:_noise_rongjie,varname:_noise03,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4811-OUT;n:type:ShaderForge.SFN_If,id:6848,x:32250,y:33613,varname:node_6848,prsc:2|A-7044-A,B-2430-R,GT-7703-OUT,EQ-7703-OUT,LT-1913-OUT;n:type:ShaderForge.SFN_Add,id:5000,x:31877,y:33081,varname:node_5000,prsc:2|A-7044-A,B-3986-OUT;n:type:ShaderForge.SFN_Subtract,id:8249,x:32544,y:33539,varname:node_8249,prsc:2|A-9812-OUT,B-6848-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3986,x:31663,y:33178,ptovrint:False,ptlb:0_勾边粗细,ptin:_0_,varname:node_3986,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.02;n:type:ShaderForge.SFN_Multiply,id:8634,x:32974,y:33375,varname:node_8634,prsc:2|A-1197-OUT,B-8249-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1197,x:32775,y:33349,ptovrint:False,ptlb:1_勾边强度,ptin:_1_,varname:node_1197,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:7722,x:33127,y:33202,varname:node_7722,prsc:2|A-9812-OUT,B-8634-OUT;n:type:ShaderForge.SFN_Multiply,id:9000,x:33433,y:32745,varname:node_9000,prsc:2|A-1664-A,B-7722-OUT;n:type:ShaderForge.SFN_VertexColor,id:7044,x:30836,y:32672,varname:node_7044,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4407,x:33913,y:33037,varname:node_4407,prsc:2|A-9000-OUT,B-7044-A;n:type:ShaderForge.SFN_Multiply,id:5414,x:33007,y:32533,varname:node_5414,prsc:2|A-2424-OUT,B-1664-RGB;n:type:ShaderForge.SFN_ValueProperty,id:2424,x:32655,y:32480,ptovrint:False,ptlb:2_diffuse强度,ptin:_2_diffuse,varname:node_2424,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1391,x:33295,y:32366,varname:node_1391,prsc:2|A-7044-RGB,B-5414-OUT;n:type:ShaderForge.SFN_Tex2d,id:4073,x:30799,y:33398,ptovrint:False,ptlb:noise02,ptin:_noise02,varname:node_4073,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9908-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7495,x:30799,y:33184,ptovrint:False,ptlb:noise01,ptin:_noise01,varname:node_7495,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9908-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6711,x:31273,y:33388,varname:node_6711,prsc:2|A-1187-OUT,B-4258-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4258,x:31049,y:33510,ptovrint:False,ptlb:3_扰动强度,ptin:_3_,varname:node_4258,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Add,id:4811,x:31479,y:33261,varname:node_4811,prsc:2|A-5014-UVOUT,B-6711-OUT;n:type:ShaderForge.SFN_Multiply,id:1187,x:31061,y:33290,varname:node_1187,prsc:2|A-7495-R,B-4073-R;n:type:ShaderForge.SFN_TexCoord,id:5014,x:31235,y:33178,varname:node_5014,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:9908,x:30402,y:33294,varname:node_9908,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3662,x:32045,y:33263,ptovrint:False,ptlb:Speed_U_copy_copy,ptin:_Speed_U_copy_copy,varname:_Speed_U_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;proporder:1664-2430-3986-1197-2424-4073-7495-4258;pass:END;sub:END;*/

Shader "Shader Forge/chongji" {
    Properties {
        _mask ("mask", 2D) = "white" {}
        _noise_rongjie ("noise_rongjie", 2D) = "white" {}
        _0_ ("0_勾边粗细", Float ) = 0.02
        _1_ ("1_勾边强度", Float ) = 2
        _2_diffuse ("2_diffuse强度", Float ) = 1
        _noise02 ("noise02", 2D) = "white" {}
        _noise01 ("noise01", 2D) = "white" {}
        _3_ ("3_扰动强度", Float ) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _noise_rongjie; uniform float4 _noise_rongjie_ST;
            uniform float _0_;
            uniform float _1_;
            uniform float _2_diffuse;
            uniform sampler2D _noise02; uniform float4 _noise02_ST;
            uniform sampler2D _noise01; uniform float4 _noise01_ST;
            uniform float _3_;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 emissive = (i.vertexColor.rgb*(_2_diffuse*_mask_var.rgb));
                float3 finalColor = emissive;
                float4 _noise01_var = tex2D(_noise01,TRANSFORM_TEX(i.uv0, _noise01));
                float4 _noise02_var = tex2D(_noise02,TRANSFORM_TEX(i.uv0, _noise02));
                float2 node_4811 = (i.uv0+((_noise01_var.r*_noise02_var.r)*_3_));
                float4 _noise_rongjie_var = tex2D(_noise_rongjie,TRANSFORM_TEX(node_4811, _noise_rongjie));
                float node_9812_if_leA = step((i.vertexColor.a+_0_),_noise_rongjie_var.r);
                float node_9812_if_leB = step(_noise_rongjie_var.r,(i.vertexColor.a+_0_));
                float node_1913 = 0.0;
                float node_7703 = 1.0;
                float node_9812 = lerp((node_9812_if_leA*node_1913)+(node_9812_if_leB*node_7703),node_7703,node_9812_if_leA*node_9812_if_leB);
                float node_6848_if_leA = step(i.vertexColor.a,_noise_rongjie_var.r);
                float node_6848_if_leB = step(_noise_rongjie_var.r,i.vertexColor.a);
                fixed4 finalRGBA = fixed4(finalColor,((_mask_var.a*(node_9812+(_1_*(node_9812-lerp((node_6848_if_leA*node_1913)+(node_6848_if_leB*node_7703),node_7703,node_6848_if_leA*node_6848_if_leB)))))*i.vertexColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
