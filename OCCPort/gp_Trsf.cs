using System;

namespace OCCPort
{
    public class gp_Trsf
    {

        public gp_Trsf()
        {
            scale = (1.0);
            shape = gp_TrsfForm.gp_Identity;
            matrix = new gp_Mat(1, 0, 0, 0, 1, 0, 0, 0, 1);
            loc = new gp_XYZ(0.0, 0.0, 0.0);
        }

		public gp_Trsf(gp_Trsf t)
		{
			scale = (1.0);
			shape = gp_TrsfForm.gp_Identity;
			matrix = new gp_Mat ( t.matrix);
			loc = t.loc;
		}

        private double scale;
        private gp_TrsfForm shape;
		public gp_Mat matrix;
        private gp_XYZ loc;

        //! Returns the nature of the transformation. It can be: an
        //! identity transformation, a rotation, a translation, a mirror
        //! transformation (relative to a point, an axis or a plane), a
        //! scaling transformation, or a compound transformation.
        public gp_TrsfForm Form() { return shape; }
		public static gp_Trsf operator *(gp_Trsf v, gp_Trsf theT)
		{
			return v.Multiplied(theT);
		}

		private gp_Trsf Multiplied(gp_Trsf theT)
		{
			gp_Trsf aTresult = (gp_Trsf)this.MemberwiseClone();
			aTresult.Multiply(theT);
			return aTresult;
		}

        internal void Multiply(gp_Trsf T)
        {
            if (T.shape == gp_TrsfForm.gp_Identity) { }
            else if (shape == gp_TrsfForm.gp_Identity)
            {
                shape = T.shape;
                scale = T.scale;
                loc = T.loc;
                matrix = T.matrix;
            }
            else if (shape == gp_TrsfForm.gp_Rotation && T.shape == gp_TrsfForm.gp_Rotation)
            {
                if (T.loc.X() != 0.0 || T.loc.Y() != 0.0 || T.loc.Z() != 0.0)
                {
                    loc.Add(T.loc.Multiplied(matrix));
                }
                matrix.Multiply(T.matrix);
            }
            else if (shape == gp_TrsfForm.gp_Translation && T.shape == gp_TrsfForm.gp_Translation)
            {
                loc.Add(T.loc);
            }
            else if (shape == gp_TrsfForm.gp_Scale && T.shape == gp_TrsfForm.gp_Scale)
            {
                loc.Add(T.loc.Multiplied(scale));
                scale = scale * T.scale;
            }
            else if (shape == gp_TrsfForm.gp_PntMirror && T.shape == gp_TrsfForm.gp_PntMirror)
            {
                scale = 1.0;
                shape = gp_TrsfForm.gp_Translation;
                loc.Add(T.loc.Reversed());
            }
            else if (shape == gp_TrsfForm.gp_Ax1Mirror && T.shape == gp_TrsfForm.gp_Ax1Mirror)
            {
                shape = gp_TrsfForm.gp_Rotation;
                loc.Add(T.loc.Multiplied(matrix));
                matrix.Multiply(T.matrix);
            }
            else if ((shape == gp_TrsfForm.gp_CompoundTrsf || shape == gp_TrsfForm.gp_Rotation ||
                      shape == gp_TrsfForm.gp_Ax1Mirror || shape == gp_TrsfForm.gp_Ax2Mirror)
                     && T.shape == gp_TrsfForm.gp_Translation)
            {
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                Tloc.Multiply(matrix);
                if (scale != 1.0) { Tloc.Multiply(scale); }
                loc.Add(Tloc);
            }
            else if ((shape == gp_TrsfForm.gp_Scale || shape == gp_TrsfForm.gp_PntMirror)
                     && T.shape == gp_TrsfForm.gp_Translation)
            {
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                Tloc.Multiply(scale);
                loc.Add(Tloc);
            }
            else if (shape == gp_TrsfForm.gp_Translation &&
                     (T.shape == gp_TrsfForm.gp_CompoundTrsf || T.shape == gp_TrsfForm.gp_Rotation ||
                      T.shape == gp_TrsfForm.gp_Ax1Mirror || T.shape == gp_TrsfForm.gp_Ax2Mirror))
            {
                shape = gp_TrsfForm.gp_CompoundTrsf;
                scale = T.scale;
                loc.Add(T.loc);
                matrix = T.matrix;
            }
            else if (shape == gp_TrsfForm.gp_Translation &&
                     (T.shape == gp_TrsfForm.gp_Scale || T.shape == gp_TrsfForm.gp_PntMirror))
            {
                shape = T.shape;
                loc.Add(T.loc);
                scale = T.scale;
            }
            else if ((shape == gp_TrsfForm.gp_PntMirror || shape == gp_TrsfForm.gp_Scale) &&
                     (T.shape == gp_TrsfForm.gp_PntMirror || T.shape == gp_TrsfForm.gp_Scale))
            {
                shape = gp_TrsfForm.gp_CompoundTrsf;
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                Tloc.Multiply(scale);
                loc.Add(Tloc);
                scale = scale * T.scale;
            }
            else if ((shape == gp_TrsfForm.gp_CompoundTrsf || shape == gp_TrsfForm.gp_Rotation ||
                      shape == gp_TrsfForm.gp_Ax1Mirror || shape == gp_TrsfForm.gp_Ax2Mirror)
                     && (T.shape == gp_TrsfForm.gp_Scale || T.shape == gp_TrsfForm.gp_PntMirror))
            {
                shape = gp_TrsfForm.gp_CompoundTrsf;
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                if (scale == 1.0)
                {
                    scale = T.scale;
                    Tloc.Multiply(matrix);
                }
                else
                {
                    Tloc.Multiply(matrix);
                    Tloc.Multiply(scale);
                    scale = scale * T.scale;
                }
                loc.Add(Tloc);
            }
            else if ((T.shape == gp_TrsfForm.gp_CompoundTrsf || T.shape == gp_TrsfForm.gp_Rotation ||
                      T.shape == gp_TrsfForm.gp_Ax1Mirror || T.shape == gp_TrsfForm.gp_Ax2Mirror)
                     && (shape == gp_TrsfForm.gp_Scale || shape == gp_TrsfForm.gp_PntMirror))
            {
                shape = gp_TrsfForm.gp_CompoundTrsf;
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                Tloc.Multiply(scale);
                loc.Add(Tloc);
                scale = scale * T.scale;
                matrix = T.matrix;
            }
            else
            {
                shape = gp_TrsfForm.gp_CompoundTrsf;
                gp_XYZ Tloc = new gp_XYZ(T.loc);
                Tloc.Multiply(matrix);
                if (scale != 1.0)
                {
                    Tloc.Multiply(scale);
                    scale = scale * T.scale;
                }
                else { scale = T.scale; }
                loc.Add(Tloc);
                matrix.Multiply(T.matrix);
            }
        }

        public void SetRotation(gp_Ax1 A1,
                             double Ang)
        {
            shape = gp_TrsfForm.gp_Rotation;
            scale = 1.0;
            loc = A1.Location().XYZ();
            matrix.SetRotation(A1.Direction().XYZ(), Ang);
            loc.Reverse();
            loc.Multiply(matrix);
            loc.Add(A1.Location().XYZ());
        }

        //! Returns the scale factor.
        public double ScaleFactor() { return scale; }

        //! Computes the homogeneous vectorial part of the transformation.
        //! It is a 3*3 matrix which doesn't include the scale factor.
        //! In other words, the vectorial part of this transformation is equal
        //! to its homogeneous vectorial part, multiplied by the scale factor.
        //! The coefficients of this matrix must be multiplied by the
        //! scale factor to obtain the coefficients of the transformation.
        public gp_Mat HVectorialPart() { return matrix; }

        internal gp_XYZ TranslationPart()
        {
            return loc;
        }

        internal void Transforms(ref gp_XYZ theCoord)
        {
            theCoord.Multiply(matrix);
            if (scale != 1.0)
            {
                theCoord.Multiply(scale);
            }
            theCoord.Add(loc);
        }

        internal void SetTranslation(gp_Vec theV)
        {
            shape = gp_TrsfForm.gp_Translation;
            scale = 1.0;
            matrix.SetIdentity();
            loc = theV.XYZ();
        }

        //! Returns true if the determinant of the vectorial part of
        //! this transformation is negative.
        public bool IsNegative() { return (scale < 0.0); }


        internal void SetTransformation(gp_Ax3 A3)
        {
            shape = gp_TrsfForm.gp_CompoundTrsf;
            scale = 1.0;
            matrix.SetRows(A3.XDirection().XYZ(),
                            A3.YDirection().XYZ(),
                            A3.Direction().XYZ());
            loc = A3.Location().XYZ();
            loc.Multiply(matrix);
            loc.Reverse();

        }

		internal gp_GTrsf Inverted()
		{
			throw new NotImplementedException();
		}
    }
}