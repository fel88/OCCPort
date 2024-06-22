namespace OCCPort.OpenGL
{
	public enum Graphic3d_AlphaMode
	{

		//Defines how alpha value of base color / texture should be treated.

		Graphic3d_AlphaMode_Opaque,

		//rendered output is fully opaque and alpha value is ignored
		Graphic3d_AlphaMode_Mask,

		//rendered output is either fully opaque or fully transparent depending on the alpha value and the alpha cutoff value
		Graphic3d_AlphaMode_Blend,

		//rendered output is combined with the background
		Graphic3d_AlphaMode_MaskBlend,
		//performs in-place blending (without implicit reordering of opaque objects) with alpha-test
		Graphic3d_AlphaMode_BlendAuto,

		//special value defined for backward compatibility - it is equal to Graphic3d_AlphaMode_Blend when Material transparency is not zero and Graphic3d_AlphaMode_Opaque otherwise;
	}
}