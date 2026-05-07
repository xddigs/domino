float iTime;
float2 iResolution;

#define SPIN_ROTATION -2.0
#define SPIN_SPEED 7.0
#define COLOUR_1 float4(0.0, 0.45, 0.95, 1.0) 
#define COLOUR_2 float4(0.0, 0.15, 0.50, 1.0) 
#define COLOUR_3 float4(0.02, 0.04, 0.10, 1.0)
#define CONTRAST 3.5
#define LIGTHING 0.4
#define SPIN_AMOUNT 0.25
#define PIXEL_FILTER 745.0
#define SPIN_EASE 1.0

sampler TextureSampler : register(s0);

float4 MainPS(float4 pos : SV_POSITION, float4 color : COLOR0, 
float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 screen_coords = texCoord * iResolution;
    float pixel_size = length(iResolution) / PIXEL_FILTER;
    float2 uv = (floor(screen_coords * (1.0 / pixel_size)) * pixel_size - 0.5 * 
    iResolution) / length(iResolution);
    
    float uv_len = length(uv);
    float speed = (SPIN_ROTATION * SPIN_EASE * 0.2) + 302.2;
    
    float angle = atan2(uv.y, uv.x) + speed - SPIN_EASE * 20.0 * (SPIN_AMOUNT * uv_len + (1.0 - SPIN_AMOUNT));
    float2 mid = (iResolution / length(iResolution)) / 2.0;
    
    uv = float2(uv_len * cos(angle) + mid.x, uv_len * sin(angle) + mid.y) - mid;
    uv *= 30.0;
    
    float timeSpeed = iTime * SPIN_SPEED;
    float2 uv2 = float2(uv.x + uv.y, uv.x + uv.y);
    
    for(int i = 0; i < 5; i++) {
        uv2 += sin(max(uv.x, uv.y)) + uv;
        uv += 0.5 * float2(cos(5.1123 + 0.353 * uv2.y + timeSpeed * 0.1311), sin(uv2.x - 0.113 * timeSpeed));
        uv -= 1.0 * cos(uv.x + uv.y) - 1.0 * sin(uv.x * 0.711 - uv.y);
    }
    
    float contrast_mod = (0.25 * CONTRAST + 0.5 * SPIN_AMOUNT + 1.2);
    float paint_res = min(2.0, max(0.0, length(uv) * 0.035 * contrast_mod));
    float c1p = max(0.0, 1.0 - contrast_mod * abs(1.0 - paint_res));
    float c2p = max(0.0, 1.0 - contrast_mod * abs(paint_res));
    float c3p = 1.0 - min(1.0, c1p + c2p);
    
    float light = (LIGTHING - 0.2) * max(c1p * 5.0 - 4.0, 0.0) + LIGTHING * max(c2p * 5.0 - 4.0, 0.0);
    
    float4 finalCol = (0.3 / CONTRAST) * COLOUR_1 + (1.0 - 0.3 / CONTRAST) * (COLOUR_1 * c1p + COLOUR_2 * c2p + float4(c3p * COLOUR_3.rgb, c3p * COLOUR_1.a)) + light;
    return finalCol;
}

technique SpriteBatch
{
    pass P0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};