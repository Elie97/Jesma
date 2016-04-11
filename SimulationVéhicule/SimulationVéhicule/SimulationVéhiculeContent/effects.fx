//------------------------------------------------------
//--                                                  --
//--		   www.riemers.net                    --
//--   		    Basic shaders                     --
//--		Use/modify as you like                --
//--                                                  --
//------------------------------------------------------

struct VertexToPixel
{
	float4 Position   	: POSITION;
	float4 Color		: COLOR0;
	float LightingFactor : TEXCOORD0;
	float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
};

//------- Constants --------
float4x4 MatriceVue;
float4x4 MatriceProjection;
float4x4 Monde;
float3 DirectionLumiere;
float LumiereAmbiante;
bool LumiereActive;
bool xShowNormals;
float3 xCamPos;
float3 xCamUp;
float xPointSpriteSize;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };


//------- Technique: Colored --------

VertexToPixel ColoredVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float4 inColor : COLOR)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(MatriceVue, MatriceProjection);
		float4x4 preWorldViewProjection = mul(Monde, preViewProjection);

		Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;

	float3 Normal = normalize(mul(normalize(inNormal), Monde));
		Output.LightingFactor = 1;
	if (LumiereActive)
		Output.LightingFactor = dot(Normal, -DirectionLumiere);

	return Output;
}

PixelToFrame ColoredPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + LumiereAmbiante;

	return Output;
}

technique Colored
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader = compile ps_2_0 ColoredPS();
	}
}



//------- Technique: Textured --------

VertexToPixel TexturedVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(MatriceVue, MatriceProjection);
		float4x4 preWorldViewProjection = mul(Monde, preViewProjection);

		Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	float3 Normal = normalize(mul(normalize(inNormal), Monde));
		Output.LightingFactor = 1;
	if (LumiereActive)
		Output.LightingFactor = dot(Normal, -DirectionLumiere);

	return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + LumiereAmbiante;

	return Output;
}

technique Textured
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader = compile ps_2_0 TexturedPS();
	}
}

//------- Technique: TexturedNoShading --------

VertexToPixel TexturedNoShadingVS(float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul(MatriceVue, MatriceProjection);
		float4x4 preWorldViewProjection = mul(Monde, preViewProjection);

		Output.Position = mul(inPos, preWorldViewProjection);
	Output.TextureCoords = inTexCoords;

	return Output;
}

PixelToFrame TexturedNoShadingPS(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;

	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);

	return Output;
}

technique TexturedNoShading
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 TexturedNoShadingVS();
		PixelShader = compile ps_2_0 TexturedNoShadingPS();
	}
}