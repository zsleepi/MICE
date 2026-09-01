shader_type canvas_item;

uniform sampler2D screen_texture: hint_screen_texture;

vec3 monochromeColor(vec3 Color) {
	float averagedColor = (Color.r + Color.g + Color.b) / 7.0;
	return vec3(averagedColor*1.2,averagedColor,averagedColor*1.5);
}

void fragment() {
	// Called for every pixel the material is visible on.
	
	vec3 baseColor = texture(screen_texture, SCREEN_UV).rgb;
	vec4 monoColor = vec4(monochromeColor(baseColor).rgb, texture(TEXTURE, UV).a);
	COLOR = monoColor;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
