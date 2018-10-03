// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33165,y:32490,varname:node_9361,prsc:2|custl-5472-OUT,alpha-4250-OUT;n:type:ShaderForge.SFN_Tex2d,id:9391,x:31554,y:32644,ptovrint:False,ptlb:texture,ptin:_texture,varname:node_9391,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d3d7af6632ecc5b46a188a7a47920096,ntxv:3,isnm:False|UVIN-6591-OUT;n:type:ShaderForge.SFN_VertexColor,id:1049,x:31430,y:32845,varname:node_1049,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9660,x:32736,y:32690,varname:node_9660,prsc:2|A-8514-OUT,B-8734-OUT,C-7781-RGB;n:type:ShaderForge.SFN_ValueProperty,id:8514,x:32522,y:32626,ptovrint:False,ptlb:Power,ptin:_Power,varname:node_8514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4250,x:32736,y:32860,varname:node_4250,prsc:2|A-9391-A,B-1049-A;n:type:ShaderForge.SFN_TexCoord,id:2664,x:30706,y:32514,varname:node_2664,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6561,x:30984,y:32594,varname:node_6561,prsc:2|A-2664-U,B-3832-OUT;n:type:ShaderForge.SFN_Append,id:9214,x:31608,y:32457,varname:node_9214,prsc:2|A-5982-OUT,B-2664-V;n:type:ShaderForge.SFN_Add,id:9503,x:31234,y:32457,varname:node_9503,prsc:2|A-7921-OUT,B-6561-OUT,C-9600-OUT;n:type:ShaderForge.SFN_Vector1,id:3299,x:30539,y:32886,varname:node_3299,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:4251,x:30849,y:32860,varname:node_4251,prsc:2|A-3832-OUT,B-3299-OUT;n:type:ShaderForge.SFN_Subtract,id:9600,x:31028,y:32743,varname:node_9600,prsc:2|A-3299-OUT,B-4251-OUT;n:type:ShaderForge.SFN_Vector1,id:7921,x:30950,y:32481,varname:node_7921,prsc:2,v1:-0.03;n:type:ShaderForge.SFN_Slider,id:1492,x:30424,y:32700,ptovrint:False,ptlb:S,ptin:_S,varname:node_1492,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2809675,max:1;n:type:ShaderForge.SFN_Clamp01,id:5982,x:31405,y:32457,varname:node_5982,prsc:2|IN-9503-OUT;n:type:ShaderForge.SFN_RemapRange,id:3832,x:30780,y:32662,varname:node_3832,prsc:2,frmn:0,frmx:1,tomn:0.65,tomx:5|IN-1492-OUT;n:type:ShaderForge.SFN_Slider,id:1931,x:31634,y:32847,ptovrint:False,ptlb:edge_size,ptin:_edge_size,varname:node_1931,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5953656,max:1;n:type:ShaderForge.SFN_Subtract,id:7558,x:31984,y:32752,varname:node_7558,prsc:2|A-9391-RGB,B-1931-OUT;n:type:ShaderForge.SFN_Sign,id:9765,x:32154,y:32752,varname:node_9765,prsc:2|IN-7558-OUT;n:type:ShaderForge.SFN_Multiply,id:8734,x:32343,y:32680,varname:node_8734,prsc:2|A-1168-OUT,B-9765-OUT;n:type:ShaderForge.SFN_Subtract,id:9249,x:32343,y:32524,varname:node_9249,prsc:2|A-1168-OUT,B-8734-OUT;n:type:ShaderForge.SFN_Multiply,id:5255,x:32736,y:32491,varname:node_5255,prsc:2|A-7367-OUT,B-9249-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4218,x:32289,y:32380,ptovrint:False,ptlb:edge_ad,ptin:_edge_ad,varname:node_4218,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.45;n:type:ShaderForge.SFN_Add,id:5472,x:32933,y:32634,varname:node_5472,prsc:2|A-5255-OUT,B-9660-OUT;n:type:ShaderForge.SFN_Add,id:7367,x:32504,y:32380,varname:node_7367,prsc:2|A-4218-OUT,B-8514-OUT;n:type:ShaderForge.SFN_Multiply,id:1168,x:31802,y:32625,varname:node_1168,prsc:2|A-9391-RGB,B-1049-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:6591,x:31360,y:32644,ptovrint:False,ptlb:switch,ptin:_switch,varname:node_6591,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2664-UVOUT,B-9214-OUT;n:type:ShaderForge.SFN_Color,id:7781,x:32452,y:32737,ptovrint:False,ptlb:node_7781,ptin:_node_7781,varname:node_7781,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:9391-8514-6591-1492-1931-4218-7781;pass:END;sub:END;*/

Shader "Shader Forge/highLight" {
    Properties {
        _texture ("texture", 2D) = "bump" {}
        _Power ("Power", Float ) = 1
        [MaterialToggle] _switch ("switch", Float ) = 0
        _S ("S", Range(0, 1)) = 0.2809675
        _edge_size ("edge_size", Range(0, 1)) = 0.5953656
        _edge_ad ("edge_ad", Float ) = -0.45
        [HDR]_node_7781 ("node_7781", Color) = (0.5,0.5,0.5,1)
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
            uniform float _edge_size;
            uniform float _edge_ad;
            uniform fixed _switch;
            uniform float4 _node_7781;
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
                float2 _switch_var = lerp( i.uv0, float2(saturate(((-0.03)+(i.uv0.r*node_3832)+(node_3299-(node_3832*node_3299)))),i.uv0.g), _switch );
                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(_switch_var, _texture));
                float3 node_1168 = (_texture_var.rgb*i.vertexColor.rgb);
                float3 node_8734 = (node_1168*sign((_texture_var.rgb-_edge_size)));
                float3 finalColor = (((_edge_ad+_Power)*(node_1168-node_8734))+(_Power*node_8734*_node_7781.rgb));
                fixed4 finalRGBA = fixed4(finalColor,(_texture_var.a*i.vertexColor.a));
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
