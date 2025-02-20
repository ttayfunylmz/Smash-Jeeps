// Writen by Martin Nerurkar ( www.sharkbombs.com)

// Return the current time based on wave input
// Lower and higher bounds adjusted for width and fade
float getShineTime(float freq, float pause, float x, float width, float fade) {
	x = -pause + fmod(x * freq, 1 + pause);
	
	// Do we reverse this?
	#if SB_SHINE_REVERSE
		x = 1 - x;
	#endif

	// We use an easing function to modify X (which goes 0 to 1)?
	#if SB_SHINE_SMOOTH
		x = smoothstep(0, 1, x);
	#elif SB_SHINE_CUBIC
		x = pow(x, 3);
	#endif
	
	// Adjust our lower and higher bounds to account for the width of our fade
	return - (width + fade) + (x * (1 + 2 * (width + fade)));
}

// Return the strength of the wave based on time and the ramp image (x = red, y = blue)
float getShinePixelAlpha(float x, float width, float fade, float2 ramp) {
	if (x > (-width - fade))
		return (smoothstep(x - width - fade, x - width, ramp.x) - smoothstep(x + width, x + width + fade, ramp.x)) * ramp.y;
	else 
		return 0;
}