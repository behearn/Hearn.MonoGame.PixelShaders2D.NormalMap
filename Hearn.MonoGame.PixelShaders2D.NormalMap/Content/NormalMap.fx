#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
Texture2D NormalMapTexture;

float3 LightPos;
float4 LightColor;
float4 AmbientColor;
float3 Falloff;
float2 Resolution;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

sampler2D NormalMapTextureSampler = sampler_state
{
	Texture = <NormalMapTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 diffuseColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);

	float4 normalMap = tex2D(NormalMapTextureSampler, input.TextureCoordinates);

	float2 currentPos = float2(Resolution.x * input.TextureCoordinates.x, Resolution.y * input.TextureCoordinates.y);

	float lx = LightPos.x - (currentPos.x / Resolution.x);
	float ly = LightPos.y - (currentPos.y / Resolution.y);
	float3 lightDir = float3(lx, ly, LightPos.z);

	lightDir.x *= Resolution.x / Resolution.y;

	float d = length(lightDir);

	normalMap = normalMap * 2.0 - 1;

	float4 n4 = normalize(normalMap);
	float3 n = float3(n4.x, n4.y, n4.z);
	float3 l = normalize(lightDir);

	float3 diffuse = (LightColor.rgb * LightColor.a) * max(dot(n, l), 0.0);

	float3 ambient = AmbientColor.rgb * AmbientColor.a;

	float attenuation = 1.0 / (Falloff.x + (Falloff.y * d) + (Falloff.z * d * d));

	float3 intensity = ambient + diffuse * attenuation;
	float3 finalColor = diffuseColor.rgb * intensity;

	return input.Color * float4(finalColor.r, finalColor.g, finalColor.b, diffuseColor.a);

}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};