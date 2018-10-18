// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:32295,y:32477,varname:node_9361,prsc:2|custl-1168-OUT,alpha-4250-OUT;n:type:ShaderForge.SFN_Tex2d,id:9391,x:31587,y:32627,ptovrint:False,ptlb:texture,ptin:_texture,varname:node_9391,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d3d7af6632ecc5b46a188a7a47920096,ntxv:3,isnm:False|UVIN-6591-OUT;n:type:ShaderForge.SFN_VertexColor,id:1049,x:31476,y:32810,varname:node_1049,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:8514,x:31859,y:32526,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_8514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:4250,x:32077,y:32763,varname:node_4250,prsc:2|A-9391-R,B-1049-A;n:type:ShaderForge.SFN_TexCoord,id:2664,x:30679,y:32488,varname:node_2664,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6561,x:30909,y:32551,varname:node_6561,prsc:2|A-2664-U,B-3832-OUT;n:type:ShaderForge.SFN_Append,id:9214,x:31642,y:32455,varname:node_9214,prsc:2|A-5982-OUT,B-2664-V;n:type:ShaderForge.SFN_Add,id:9503,x:31222,y:32393,varname:node_9503,prsc:2|A-6561-OUT,B-9600-OUT;n:type:ShaderForge.SFN_Vector1,id:3299,x:30556,y:32826,varname:node_3299,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:4251,x:30909,y:32783,varname:node_4251,prsc:2|A-3832-OUT,B-3299-OUT;n:type:ShaderForge.SFN_Subtract,id:9600,x:31128,y:32647,varname:node_9600,prsc:2|A-3299-OUT,B-4251-OUT;n:type:ShaderForge.SFN_Slider,id:1492,x:30350,y:32663,ptovrint:False,ptlb:S,ptin:_S,varname:node_1492,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:5982,x:31453,y:32416,varname:node_5982,prsc:2|IN-9503-OUT;n:type:ShaderForge.SFN_RemapRange,id:3832,x:30679,y:32663,varname:node_3832,prsc:2,frmn:0,frmx:1,tomn:0.65,tomx:5|IN-1492-OUT;n:type:ShaderForge.SFN_Multiply,id:1168,x:31877,y:32626,varname:node_1168,prsc:2|A-9391-RGB,B-1049-RGB,C-8514-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6591,x:31383,y:32627,ptovrint:False,ptlb:switch,ptin:_switch,varname:node_6591,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2664-UVOUT,B-9214-OUT;proporder:9391-8514-6591-1492;pass:END;sub:END;*/

Shader "Shader Forge/highLight" {
    Properties {
        _texture ("texture", 2D) = "bump" {}
        _Power ("Power", Float ) = 3
        [MaterialToggle] _switch ("switch", Float ) = 0
        _S ("S", Range(0, 1)) = 0
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
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _Power;
            uniform float _S;
            uniform fixed _switch;
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
                float node_3832 = (_S*4.35+0.65);
                float node_3299 = 0.5;
                float2 _switch_var = lerp( i.uv0, float2(saturate(((i.uv0.r*node_3832)+(node_3299-(node_3832*node_3299)))),i.uv0.g), _switch );
                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(_switch_var, _texture));
                float3 finalColor = (_texture_var.rgb*i.vertexColor.rgb*_Power);
                fixed4 finalRGBA = fixed4(finalColor,(_texture_var.r*i.vertexColor.a));
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
