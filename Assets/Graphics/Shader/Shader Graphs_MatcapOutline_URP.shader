Shader "Shader Graphs/MatcapOutline_URP" {
	Properties {
		[NoScaleOffset] _Matcap ("Matcap", 2D) = "white" {}
		[NoScaleOffset] _MainTexture ("MainTexture", 2D) = "white" {}
		[NoScaleOffset] _Matcapgloss ("MatcapGloss", 2D) = "white" {}
		_Color ("Color", Vector) = (0.67,0.24,0.1,1)
		_EmissionColor ("EmissionColor", Vector) = (0,0,0,0)
		_RimColor ("Rim Color", Vector) = (0,0,0,0)
		_RimBSP ("Rim BSP", Vector) = (0,1,0.77,0)
		[Toggle] _USEGLOSS ("Use Gloss", Float) = 0
		[Toggle] _VERTEXCOLORTINT ("VertexColorTint", Float) = 0
		_HitFlash ("Hit Flash", Float) = 0
		_GlossIntensity ("GlossIntensity", Float) = 0.5
		_DiffuseEmissive ("DiffuseEmissive", Float) = 0
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}