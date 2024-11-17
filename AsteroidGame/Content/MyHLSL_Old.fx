float4x4 RotationMatrix;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;
float4x4 WorldViewProjMatrix;
float3 Scale;
float Transparency;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR;AddressU = wrap; AddressV = wrap;}; //http://xboxforums.create.msdn.com/forums/p/110164/653467.aspx Explanation


struct VertexToPixel
{
    float4 Position     : POSITION;
    float2 TexCoords	: TEXCOORD0;
};
struct PixelToFrame
{
    float4 Color        : COLOR0;
};




VertexToPixel Texture_VertexShader(float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;
	inPos.x *= Scale.x;
	inPos.y *= Scale.y;
	Output.Position = mul( mul(inPos , RotationMatrix), WorldViewProjMatrix);	//Rotates the Position by the Rotation and then applies the World View Projection Matrix.

	Output.TexCoords = inTexCoords;

    
    return Output;
}


PixelToFrame Texture_PixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    

    Output.Color = tex2D(TextureSampler, PSIn.TexCoords);  
	Output.Color.a -= Transparency;

    return Output;
}

technique Technique_Textured
{
    pass Pass0
    {
        VertexShader = compile vs_4_0_level_9_1 Texture_VertexShader();
        PixelShader = compile ps_4_0_level_9_1 Texture_PixelShader();
    }
}























//------- Technique: Technique_Colored --------

struct VertexToPixel_NoTexure
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
};

VertexToPixel_NoTexure ColoredNoShadingVS( float4 inPos : POSITION, float4 inColor: COLOR)
{	
	VertexToPixel_NoTexure Output = (VertexToPixel_NoTexure)0;
	inPos.x *= Scale.x;
	inPos.y *= Scale.y;

	Output.Position = mul( mul(inPos, RotationMatrix), WorldViewProjMatrix);	//Rotates the Position by the Rotation and then applies the World View Projection Matrix.
	Output.Color = inColor;
    
	return Output;    
}

PixelToFrame ColoredNoShadingPS(VertexToPixel_NoTexure PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	Output.Color = PSIn.Color;
	return Output;
}

technique Technique_Colored
{
	pass Pass0
	{   
        VertexShader = compile vs_4_0_level_9_1 ColoredNoShadingVS();
        PixelShader = compile ps_4_0_level_9_1 ColoredNoShadingPS();
    }
}










//------- Technique: PreTransformed --------


struct VTP
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};


VTP PretransformedVS( float4 inPos : POSITION, float4 inColor: COLOR)
{	
	VTP Output = (VTP)0;
	
	Output.Position = inPos;
	Output.Color = inColor;
    
	return Output;    
}

PixelToFrame PretransformedPS(VTP PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed
{
	pass Pass0
	{   
        VertexShader = compile vs_4_0_level_9_1 PretransformedVS();
        PixelShader = compile ps_4_0_level_9_1 PretransformedPS();
    }
}