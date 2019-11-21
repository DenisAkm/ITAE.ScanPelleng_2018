matrix gWVP;

float4 VShader(float4 position : POSITION) : SV_POSITION
{
	return mul( position, gWVP);
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return float4(0.2f, 0.2f, 0.2f, 0.2f);
}

RasterizerState WireframeState
{
    FillMode = Wireframe;
    //CullMode = Front;
    //FrontCounterClockwise = true;
};

technique10 Render
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VShader() ));
		SetGeometryShader( NULL );
		SetPixelShader( CompileShader( ps_4_0, PShader() ));
		SetRasterizerState(WireframeState);
	}
}