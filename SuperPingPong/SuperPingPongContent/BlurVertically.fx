sampler TextureSampler : register(s0);
float PixelSize = 0.00625;
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 Color = tex2D(TextureSampler, coords) * 0.3125;
	float2 temp = coords;
	temp.y += PixelSize * 1;
	Color += tex2D(TextureSampler, temp) * 0.15;
	temp = coords;
	temp.y += PixelSize * 2;
	Color += tex2D(TextureSampler, temp) * 0.12;
	temp = coords;
	temp.y += PixelSize * 3;
	Color += tex2D(TextureSampler, temp) * 0.09;
	temp = coords;
	temp.y -= PixelSize * 1;
	Color += tex2D(TextureSampler, temp) * 0.15;
	temp = coords;
	temp.y -= PixelSize * 2;
	Color += tex2D(TextureSampler, temp) * 0.12;
	temp = coords;
	temp.y -= PixelSize * 3;
	Color += tex2D(TextureSampler, temp) * 0.09;
    return Color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
