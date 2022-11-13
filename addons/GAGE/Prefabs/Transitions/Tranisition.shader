shader_type canvas_item;
render_mode unshaded;

uniform float delta : hint_range(0.0, 1.0);
uniform sampler2D old_screen: hint_albedo;
uniform sampler2D mask : hint_albedo;
uniform bool invert;
uniform float smoothing;
uniform bool distort;

void fragment()
{
	vec4 maskColor = texture(mask,UV);
	vec4 oldColor = texture(old_screen,UV);
	vec4 newColor = texture(TEXTURE,UV);
	float maskValue;
	if(invert)
		maskValue = 1f-maskColor.b;
	else
		maskValue = maskColor.b;
			
	float alpha = smoothstep(delta, delta + smoothing, maskValue * (1.0-smoothing) + smoothing);
			
	if(!distort)
	{
		if(maskValue<=delta*2.0)
		{
			COLOR = mix(oldColor,newColor,1.0-alpha);
		}
		else
		{
			COLOR = oldColor;
		}
	}
	else
	{
		vec2 val = vec2(maskColor.r,maskColor.g);
		val = (val-0.5)*2f;
		vec2 direction = normalize(val);
		vec4 color = texture(old_screen,UV+delta*direction);
		if(maskValue<=delta*2.0)
		{
			COLOR = mix(color,newColor,1.0-alpha);
		}
		else
		{
			COLOR=oldColor;
		}
	}
}