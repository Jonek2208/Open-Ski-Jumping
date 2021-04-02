using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTI {
	
	public static class CTI_Utils {

	
	//	Function to adjust the translucent lighting fade according to the given shadow distance – or any othe distance that is passed in
	//	@params:
	//	(float) TranslucentLightingRange	: Range in which translucent lighting will be applied – most likely the shadow distance as set in Quality Settings
	//	(float) FadeLengthFactor			: Lenth relative to TranslucentLightingRange over which the fade will take place (0.0 - 1.0 range)

		public static void SetTranslucentLightingFade(float TranslucentLightingRange, float FadeLengthFactor) {
			TranslucentLightingRange *= 0.9f; // Add some padding as real time shadows fade out as well
			var FadeLength = TranslucentLightingRange * FadeLengthFactor;
		//	Pleae note: We use sqr distances here!
			Shader.SetGlobalVector ("_CTI_TransFade", 
				new Vector2( 
					TranslucentLightingRange * TranslucentLightingRange,
					FadeLength * FadeLength * ( (TranslucentLightingRange / FadeLength) * 2.0f )
				)
			);
		}

	}
}
