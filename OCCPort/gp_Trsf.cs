using OCCPort;
using System;
using System.Threading;

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

        //! Computes the following composition of transformations
        //! <me> * <me> * .......* <me>, theN time.
        //! if theN = 0 <me> = Identity
        //! if theN < 0 <me> = <me>.Inverse() *...........* <me>.Inverse().
        //!
        //! Raises if theN < 0 and if the matrix of the transformation not
        //! inversible.
        public gp_Trsf Powered(int theN)
        {
            gp_Trsf aT = new gp_Trsf(this);
            aT.Power(theN);
            return aT;
        }
        public void Power(int N)
        {
            if (shape == gp_TrsfForm.gp_Identity) { }
            else
            {
                if (N == 0)
                {
                    scale = 1.0;
                    shape = gp_TrsfForm.gp_Identity;
                    matrix.SetIdentity();
                    loc = new gp_XYZ(0.0, 0.0, 0.0);
                }
                else if (N == 1) { }
                else if (N == -1) { Invert(); }
                else
                {
                    if (N < 0) { Invert(); }
                    if (shape == gp_TrsfForm.gp_Translation)
                    {
                        int Npower = N;
                        if (Npower < 0) Npower = -Npower;
                        Npower--;
                        gp_XYZ Temploc = loc;
                        for (; ; )
                        {
                            if (IsOdd(Npower)) loc.Add(Temploc);
                            if (Npower == 1) break;
                            Temploc.Add(Temploc);
                            Npower = Npower / 2;
                        }
                    }
                    else if (shape == gp_TrsfForm.gp_Scale)
                    {
                        int Npower = N;
                        if (Npower < 0) Npower = -Npower;
                        Npower--;
                        gp_XYZ Temploc = loc;
                        double Tempscale = scale;
                        for (; ; )
                        {
                            if (IsOdd(Npower))
                            {
                                loc.Add(Temploc.Multiplied(scale));
                                scale = scale * Tempscale;
                            }
                            if (Npower == 1) break;
                            Temploc.Add(Temploc.Multiplied(Tempscale));
                            Tempscale = Tempscale * Tempscale;
                            Npower = Npower / 2;
                        }
                    }
                    else if (shape == gp_TrsfForm.gp_Rotation)
                    {
                        int Npower = N;
                        if (Npower < 0) Npower = -Npower;
                        Npower--;
                        gp_Mat Tempmatrix = new gp_Mat(matrix);
                        if (loc.X() == 0.0 && loc.Y() == 0.0 && loc.Z() == 0.0)
                        {
                            for (; ; )
                            {
                                if (IsOdd(Npower)) matrix.Multiply(Tempmatrix);
                                if (Npower == 1) break;
                                Tempmatrix.Multiply(Tempmatrix);
                                Npower = Npower / 2;
                            }
                        }
                        else
                        {
                            gp_XYZ Temploc = loc;
                            for (; ; )
                            {
                                if (IsOdd(Npower))
                                {
                                    loc.Add(Temploc.Multiplied(matrix));
                                    matrix.Multiply(Tempmatrix);
                                }
                                if (Npower == 1) break;
                                Temploc.Add(Temploc.Multiplied(Tempmatrix));
                                Tempmatrix.Multiply(Tempmatrix);
                                Npower = Npower / 2;
                            }
                        }
                    }
                    else if (shape == gp_TrsfForm.gp_PntMirror || shape == gp_TrsfForm.gp_Ax1Mirror ||
                             shape == gp_TrsfForm.gp_Ax2Mirror)
                    {
                        if (IsEven(N))
                        {
                            shape = gp_TrsfForm.gp_Identity;
                            scale = 1.0;
                            matrix.SetIdentity();
                            loc.SetX(0);
                            loc.SetY(0);
                            loc.SetZ(0);
                        }
                    }
                    else
                    {
                        shape = gp_TrsfForm.gp_CompoundTrsf;
                        int Npower = N;
                        if (Npower < 0) Npower = -Npower;
                        Npower--;
                        gp_XYZ Temploc = loc;
                        double Tempscale = scale;
                        gp_Mat Tempmatrix = new gp_Mat(matrix);
                        for (; ; )
                        {
                            if (IsOdd(Npower))
                            {
                                loc.Add((Temploc.Multiplied(matrix)).Multiplied(scale));
                                scale = scale * Tempscale;
                                matrix.Multiply(Tempmatrix);
                            }
                            if (Npower == 1) break;
                            Tempscale = Tempscale * Tempscale;
                            Temploc.Add((Temploc.Multiplied(Tempmatrix)).Multiplied
                                          (Tempscale)
                                          );
                            Tempmatrix.Multiply(Tempmatrix);
                            Npower = Npower / 2;
                        }
                    }
                }
            }
        }// ------------------------------------------------------------------
         // IsOdd : Returns Standard_True if an integer is odd
         // ------------------------------------------------------------------
        public bool IsOdd(int Value)
        { return Value % 2 == 1; }
        // ------------------------------------------------------------------
        // IsEven : Returns Standard_True if an integer is even
        // ------------------------------------------------------------------
        public  bool IsEven( int Value)
{ return Value % 2 == 0; }


        public gp_Trsf(gp_Trsf t)
        {
            scale = (1.0);
            shape = gp_TrsfForm.gp_Identity;
            matrix = new gp_Mat(t.matrix);
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
            gp_Trsf aTresult = new gp_Trsf(this);
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
        //=======================================================================
        public void SetTranslation(gp_Pnt theP1,
                                      gp_Pnt theP2)
        {
            shape = gp_TrsfForm.gp_Translation;
            scale = 1.0;
            matrix.SetIdentity();
            loc = (theP2.XYZ()).Subtracted(theP1.XYZ());
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

        public gp_Mat VectorialPart()
        {
            if (scale == 1.0)
                return matrix;

            gp_Mat M = matrix;
            if (shape == gp_TrsfForm.gp_Scale || shape == gp_TrsfForm.gp_PntMirror)
                M.SetDiagonal(scale * M.Value(1, 1),
                              scale * M.Value(2, 2),
                              scale * M.Value(3, 3));
            else
                M.Multiply(scale);
            return M;
        }

        internal void Invert()
        {
            //                                    -1
            //  X' = scale * R * X + T  =>  X = (R  / scale)  * ( X' - T)
            //
            // Pour les gp_Trsf puisque le scale est extrait de la gp_Matrice R
            // on a toujours determinant (R) = 1 et R-1 = R transposee.
            if (shape == gp_TrsfForm.gp_Identity) { }
            else if (shape == gp_TrsfForm.gp_Translation || shape == gp_TrsfForm.gp_PntMirror) loc.Reverse();
            else if (shape == gp_TrsfForm.gp_Scale)
            {
                Standard_ConstructionError_Raise_if(Math.Abs(scale) <= gp.Resolution(), "gp_Trsf::Invert() - transformation has zero scale");
                scale = 1.0 / scale;
                loc.Multiply(-scale);
            }
            else
            {
                Standard_ConstructionError_Raise_if(Math.Abs(scale) <= gp.Resolution(), "gp_Trsf::Invert() - transformation has zero scale");
                scale = 1.0 / scale;
                matrix.Transpose();
                loc.Multiply(matrix);
                loc.Multiply(-scale);
            }
        }

        private void Standard_ConstructionError_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        internal void PreMultiply(gp_Trsf T)
        {


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
                    loc.Multiply(T.matrix);
                    loc.Add(T.loc);
                    matrix.PreMultiply(T.matrix);
                }
                else if (shape == gp_TrsfForm.gp_Translation && T.shape == gp_TrsfForm.gp_Translation)
                {
                    loc.Add(T.loc);
                }
                else if (shape == gp_TrsfForm.gp_Scale && T.shape == gp_TrsfForm.gp_Scale)
                {
                    loc.Multiply(T.scale);
                    loc.Add(T.loc);
                    scale = scale * T.scale;
                }
                else if (shape == gp_TrsfForm.gp_PntMirror && T.shape == gp_TrsfForm.gp_PntMirror)
                {
                    scale = 1.0;
                    shape = gp_TrsfForm.gp_Translation;
                    loc.Reverse();
                    loc.Add(T.loc);
                }
                else if (shape == gp_TrsfForm.gp_Ax1Mirror && T.shape == gp_TrsfForm.gp_Ax1Mirror)
                {
                    shape = gp_TrsfForm.gp_Rotation;
                    loc.Multiply(T.matrix);
                    loc.Add(T.loc);
                    matrix.PreMultiply(T.matrix);
                }
                else if ((shape == gp_TrsfForm.gp_CompoundTrsf || shape == gp_TrsfForm.gp_Rotation ||
                          shape == gp_TrsfForm.gp_Ax1Mirror || shape == gp_TrsfForm.gp_Ax2Mirror)
                         && T.shape == gp_TrsfForm.gp_Translation)
                {
                    loc.Add(T.loc);
                }
                else if ((shape == gp_TrsfForm.gp_Scale || shape == gp_TrsfForm.gp_PntMirror)
                         && T.shape == gp_TrsfForm.gp_Translation)
                {
                    loc.Add(T.loc);
                }
                else if (shape == gp_TrsfForm.gp_Translation &&
                         (T.shape == gp_TrsfForm.gp_CompoundTrsf || T.shape == gp_TrsfForm.gp_Rotation
                          || T.shape == gp_TrsfForm.gp_Ax1Mirror || T.shape == gp_TrsfForm.gp_Ax2Mirror))
                {
                    shape = gp_TrsfForm.gp_CompoundTrsf;
                    matrix = T.matrix;
                    if (T.scale == 1.0) loc.Multiply(T.matrix);
                    else
                    {
                        scale = T.scale;
                        loc.Multiply(matrix);
                        loc.Multiply(scale);
                    }
                    loc.Add(T.loc);
                }
                else if ((T.shape == gp_TrsfForm.gp_Scale || T.shape == gp_TrsfForm.gp_PntMirror)
                         && shape == gp_TrsfForm.gp_Translation)
                {
                    loc.Multiply(T.scale);
                    loc.Add(T.loc);
                    scale = T.scale;
                    shape = T.shape;
                }
                else if ((shape == gp_TrsfForm.gp_PntMirror || shape == gp_TrsfForm.gp_Scale) &&
                         (T.shape == gp_TrsfForm.gp_PntMirror || T.shape == gp_TrsfForm.gp_Scale))
                {
                    shape = gp_TrsfForm.gp_CompoundTrsf;
                    loc.Multiply(T.scale);
                    loc.Add(T.loc);
                    scale = scale * T.scale;
                }
                else if ((shape == gp_TrsfForm.gp_CompoundTrsf || shape == gp_TrsfForm.gp_Rotation ||
                          shape == gp_TrsfForm.gp_Ax1Mirror || shape == gp_TrsfForm.gp_Ax2Mirror)
                         && (T.shape == gp_TrsfForm.gp_Scale || T.shape == gp_TrsfForm.gp_PntMirror))
                {
                    shape = gp_TrsfForm.gp_CompoundTrsf;
                    loc.Multiply(T.scale);
                    loc.Add(T.loc);
                    scale = scale * T.scale;
                }
                else if ((T.shape == gp_TrsfForm.gp_CompoundTrsf || T.shape == gp_TrsfForm.gp_Rotation ||
                          T.shape == gp_TrsfForm.gp_Ax1Mirror || T.shape == gp_TrsfForm.gp_Ax2Mirror)
                         && (shape == gp_TrsfForm.gp_Scale || shape == gp_TrsfForm.gp_PntMirror))
                {
                    shape = gp_TrsfForm.gp_CompoundTrsf;
                    matrix = T.matrix;
                    if (T.scale == 1.0) loc.Multiply(T.matrix);
                    else
                    {
                        loc.Multiply(matrix);
                        loc.Multiply(T.scale);
                        scale = T.scale * scale;
                    }
                    loc.Add(T.loc);
                }
                else
                {
                    shape = gp_TrsfForm.gp_CompoundTrsf;
                    loc.Multiply(T.matrix);
                    if (T.scale != 1.0)
                    {
                        loc.Multiply(T.scale); scale = scale * T.scale;
                    }
                    loc.Add(T.loc);
                    matrix.PreMultiply(T.matrix);
                }
            }

        }
    }
}