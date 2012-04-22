// Define inputs.
sampler2D TextureSampler;

// Define structures. 
struct PS_INPUT
{
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

float4 BackgroundPixelShader(PS_INPUT input) : COLOR0
{
    float4 c = tex2D(TextureSampler, input.TexCoord);
    return c * 0.5; 
}

technique Blur
{
    pass
    {
        PixelShader = compile ps_2_0 BackgroundPixelShader();
    }
}
