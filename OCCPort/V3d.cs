namespace OCCPort
{
	internal class V3d
	{
		internal static gp_Dir GetProjAxis(V3d_TypeOfOrientation theOrientation)
		{

			switch (theOrientation)
			{
				case V3d_TypeOfOrientation.V3d_Xpos: return gp.DX();
				case V3d_TypeOfOrientation.V3d_Ypos: return gp.DY();
				case V3d_TypeOfOrientation.V3d_Zpos: return gp.DZ();
				case V3d_TypeOfOrientation.V3d_Xneg: return -gp.DX();
				case V3d_TypeOfOrientation.V3d_Yneg: return -gp.DY();
				case V3d_TypeOfOrientation.V3d_Zneg: return -gp.DZ();
				case V3d_TypeOfOrientation.V3d_XposYposZpos: return new gp_Dir(1, 1, 1);
				case V3d_TypeOfOrientation.V3d_XposYposZneg: return new gp_Dir(1, 1, -1);
				case V3d_TypeOfOrientation.V3d_XposYnegZpos: return new gp_Dir(1, -1, 1);
				case V3d_TypeOfOrientation.V3d_XposYnegZneg: return new gp_Dir(1, -1, -1);
				case V3d_TypeOfOrientation.V3d_XnegYposZpos: return new gp_Dir(-1, 1, 1);
				case V3d_TypeOfOrientation.V3d_XnegYposZneg: return new gp_Dir(-1, 1, -1);
				case V3d_TypeOfOrientation.V3d_XnegYnegZpos: return new gp_Dir(-1, -1, 1);
				case V3d_TypeOfOrientation.V3d_XnegYnegZneg: return new gp_Dir(-1, -1, -1);
				case V3d_TypeOfOrientation.V3d_XposYpos: return new gp_Dir(1, 1, 0);
				case V3d_TypeOfOrientation.V3d_XposYneg: return new gp_Dir(1, -1, 0);
				case V3d_TypeOfOrientation.V3d_XnegYpos: return new gp_Dir(-1, 1, 0);
				case V3d_TypeOfOrientation.V3d_XnegYneg: return new gp_Dir(-1, -1, 0);
				case V3d_TypeOfOrientation.V3d_XposZpos: return new gp_Dir(1, 0, 1);
				case V3d_TypeOfOrientation.V3d_XposZneg: return new gp_Dir(1, 0, -1);
				case V3d_TypeOfOrientation.V3d_XnegZpos: return new gp_Dir(-1, 0, 1);
				case V3d_TypeOfOrientation.V3d_XnegZneg: return new gp_Dir(-1, 0, -1);
				case V3d_TypeOfOrientation.V3d_YposZpos: return new gp_Dir(0, 1, 1);
				case V3d_TypeOfOrientation.V3d_YposZneg: return new gp_Dir(0, 1, -1);
				case V3d_TypeOfOrientation.V3d_YnegZpos: return new gp_Dir(0, -1, 1);
				case V3d_TypeOfOrientation.V3d_YnegZneg: return new gp_Dir(0, -1, -1);
			}
			return new gp_Dir(0, 0, 0);

		}
	}
}
