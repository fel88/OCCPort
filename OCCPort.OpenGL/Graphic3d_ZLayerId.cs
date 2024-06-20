namespace OCCPort
{
	public enum Graphic3d_ZLayerId
    {
		//Graphic3d_ZLayerId_UNKNOWN

		//identifier for invalid ZLayer
		Graphic3d_ZLayerId_Default,

		//default Z-layer for main presentations
		Graphic3d_ZLayerId_Top,

		//overlay for 3D presentations which inherits Depth from previous ZLayer
		Graphic3d_ZLayerId_Topmost,

		//overlay for 3D presentations with independent Depth
		Graphic3d_ZLayerId_TopOSD,

		//overlay for 2D presentations (On-Screen-Display)
		Graphic3d_ZLayerId_BotOSD

		//underlay for 2D presentations (On-Screen-Display) 
    }
}