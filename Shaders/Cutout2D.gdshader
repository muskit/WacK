shader_type canvas_item;

uniform sampler2D mask;

void fragment( )
{
	vec4 mask_color = texture(mask, UV).rgba;
	vec4 sprite_color = texture(TEXTURE, UV).rgba;
	
	sprite_color.a = mask_color.a;
	COLOR = sprite_color;
}