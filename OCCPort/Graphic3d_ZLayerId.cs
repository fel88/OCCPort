namespace OCCPort
{
	public enum Graphic3d_ZLayerId
	{

		Graphic3d_ZLayerId_UNKNOWN = -1,
		//identifier for invalid ZLayer
		Graphic3d_ZLayerId_Default = 0,

		//default Z-layer for main presentations
		Graphic3d_ZLayerId_Top = -2,

		//overlay for 3D presentations which inherits Depth from previous ZLayer
		Graphic3d_ZLayerId_Topmost = -3,

		//overlay for 3D presentations with independent Depth
		Graphic3d_ZLayerId_TopOSD = -4,

		//overlay for 2D presentations (On-Screen-Display)
		Graphic3d_ZLayerId_BotOSD = -5

		//underlay for 2D presentations (On-Screen-Display) 
	}
}