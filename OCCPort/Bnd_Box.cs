using System;
using System.Security.Cryptography;

namespace OCCPort
{
    public class Bnd_Box
    {

        //! Returns true if this bounding box is empty (Void flag).
        public bool IsVoid()
        {
            //return (Flags.HasFlag(MaskFlags.VoidMask));
            return (Flags & MaskFlags.VoidMask) != 0;
        }

        //! Returns the lower corner of this bounding box. The gap is included.
        //! If this bounding box is infinite (i.e. "open"), returned values
        //! may be equal to +/- Precision::Infinite().
        //! Standard_ConstructionError exception will be thrown if the box is void.
        //! if IsVoid()
        public gp_Pnt CornerMin()
        {
            gp_Pnt aCornerMin = new gp_Pnt();
            if (IsVoid())
            {
                throw new Standard_ConstructionError("Bnd_Box is void");
            }
            if (IsOpenXmin()) aCornerMin.SetX(-Bnd_Precision_Infinite);
            else aCornerMin.SetX(Xmin - Gap);
            if (IsOpenYmin()) aCornerMin.SetY(-Bnd_Precision_Infinite);
            else aCornerMin.SetY(Ymin - Gap);
            if (IsOpenZmin()) aCornerMin.SetZ(-Bnd_Precision_Infinite);
            else aCornerMin.SetZ(Zmin - Gap);
            return aCornerMin;
        }

        //! Returns the upper corner of this bounding box. The gap is included.
        //! If this bounding box is infinite (i.e. "open"), returned values
        //! may be equal to +/- Precision::Infinite().
        //! Standard_ConstructionError exception will be thrown if the box is void.
        //! if IsVoid()
        public gp_Pnt CornerMax()
        {
            gp_Pnt aCornerMax = new gp_Pnt();
            if (IsVoid())
            {
                throw new Standard_ConstructionError("Bnd_Box is void");
            }
            if (IsOpenXmax()) aCornerMax.SetX(Bnd_Precision_Infinite);
            else aCornerMax.SetX(Xmax + Gap);
            if (IsOpenYmin()) aCornerMax.SetY(Bnd_Precision_Infinite);
            else aCornerMax.SetY(Ymax + Gap);
            if (IsOpenZmin()) aCornerMax.SetZ(Bnd_Precision_Infinite);
            else aCornerMax.SetZ(Zmax + Gap);
            return aCornerMax;
        }
        //! Returns TRUE if this box has finite part.
        public bool HasFinitePart()
        {
            return !IsVoid()
                 && Xmax >= Xmin;
        }

        //=======================================================================
        //function : Enlarge
        //purpose  : 
        //=======================================================================

        public void Enlarge(double Tol)
        {
            Gap = Math.Max(Gap, Math.Abs(Tol));
        }
        public void Add(Bnd_Box Other)
        {
            if (Other.IsVoid())
            {
                return;
            }
            else if (IsVoid())
            {
                //*this = Other;//??
                return;
            }

            if (Xmin > Other.Xmin) Xmin = Other.Xmin;
            if (Xmax < Other.Xmax) Xmax = Other.Xmax;
            if (Ymin > Other.Ymin) Ymin = Other.Ymin;
            if (Ymax < Other.Ymax) Ymax = Other.Ymax;
            if (Zmin > Other.Zmin) Zmin = Other.Zmin;
            if (Zmax < Other.Zmax) Zmax = Other.Zmax;
            Gap = Math.Max(Gap, Other.Gap);

            if (IsWhole())
            {
                return;
            }
            else if (Other.IsWhole())
            {
                SetWhole();
                return;
            }

            if (Other.IsOpenXmin()) OpenXmin();
            if (Other.IsOpenXmax()) OpenXmax();
            if (Other.IsOpenYmin()) OpenYmin();
            if (Other.IsOpenYmax()) OpenYmax();
            if (Other.IsOpenZmin()) OpenZmin();
            if (Other.IsOpenZmax()) OpenZmax();
        }
        //! Sets this bounding box so that it covers the whole of 3D space.
        //! It is infinitely long in all directions.
        public void SetWhole() { Flags = MaskFlags.WholeMask; }


        //! The   Box will be   infinitely   long  in the Xmin
        //! direction.
        public void OpenXmin() { Flags |= MaskFlags.XminMask; }

        //! The   Box will be   infinitely   long  in the Xmax
        //! direction.
        public void OpenXmax() { Flags |= MaskFlags.XmaxMask; }

        //! The   Box will be   infinitely   long  in the Ymin
        //! direction.
        public void OpenYmin() { Flags |= MaskFlags.YminMask; }

        //! The   Box will be   infinitely   long  in the Ymax
        //! direction.
        public void OpenYmax() { Flags |= MaskFlags.YmaxMask; }

        //! The   Box will be   infinitely   long  in the Zmin
        //! direction.
        public void OpenZmin() { Flags |= MaskFlags.ZminMask; }

        //! The   Box will be   infinitely   long  in the Zmax
        //! direction.
        public void OpenZmax() { Flags |= MaskFlags.ZmaxMask; }
        //! Returns true if this bounding box is infinite in all 6 directions (WholeSpace flag).
        public bool IsWhole() { return (Flags & MaskFlags.WholeMask) == MaskFlags.WholeMask; }
        public void Add(gp_Pnt P)
        {
            double X = 0, Y = 0, Z = 0;
            P.Coord(ref X, ref Y, ref Z);
            Update(X, Y, Z);
        }

        public void Update(double X,

              double Y,

              double Z)
        {
            if (IsVoid())
            {
                Xmin = X;
                Ymin = Y;
                Zmin = Z;
                Xmax = X;
                Ymax = Y;
                Zmax = Z;
                ClearVoidFlag();
            }
            else
            {
                if (X < Xmin) Xmin = X;
                else if (X > Xmax) Xmax = X;
                if (Y < Ymin) Ymin = Y;
                else if (Y > Ymax) Ymax = Y;
                if (Z < Zmin) Zmin = Z;
                else if (Z > Zmax) Zmax = Z;
            }
        }


        //! Returns a finite part of an infinite bounding box (returns self if this is already finite box).
        //! This can be a Void box in case if its sides has been defined as infinite (Open) without adding any finite points.
        //! WARNING! This method relies on Open flags, the infinite points added using Add() method will be returned as is.
        public Bnd_Box FinitePart()
        {
            if (!HasFinitePart())
            {
                return new Bnd_Box();
            }

            Bnd_Box aBox = new Bnd_Box();
            aBox.Update(Xmin, Ymin, Zmin, Xmax, Ymax, Zmax);
            aBox.SetGap(Gap);
            return aBox;
        }

        //=======================================================================
        //function : SetGap
        //purpose  : 
        //=======================================================================

        public void SetGap(double Tol)
        {
            Gap = Tol;
        }

        //=======================================================================
        //function : Update
        //purpose  : 
        //=======================================================================

        public void Update(double x,
              double y,
              double z,
              double X,
              double Y,
              double Z)
        {
            if (IsVoid())
            {
                Xmin = x;
                Ymin = y;
                Zmin = z;
                Xmax = X;
                Ymax = Y;
                Zmax = Z;
                ClearVoidFlag();
            }
            else
            {
                if (x < Xmin) Xmin = x;
                if (X > Xmax) Xmax = X;
                if (y < Ymin) Ymin = y;
                if (Y > Ymax) Ymax = Y;
                if (z < Zmin) Zmin = z;
                if (Z > Zmax) Zmax = Z;
            }
        }

        // set the flag to one
        void ClearVoidFlag() { Flags &= ~MaskFlags.VoidMask; }

        //! Returns true if this bounding box has at least one open direction.
        public bool IsOpen() { return (Flags & MaskFlags.WholeMask) != 0; }

        private double Xmin;
        private double Xmax;
        private double Ymin;
        private double Ymax;
        private double Zmin;
        private double Zmax;
        private double Gap;
        private MaskFlags Flags;

        const double Bnd_Precision_Infinite = 1e+100;
        public void Get(out double theXmin,
                        out double theYmin,
                        out double theZmin,
                       out double theXmax,
                        out double theYmax,
                        out double theZmax)
        {
            if (IsVoid())
            {
                throw new Exception("Bnd_Box is void");
            }

            if (IsOpenXmin()) theXmin = -Bnd_Precision_Infinite;
            else theXmin = Xmin - Gap;
            if (IsOpenXmax()) theXmax = Bnd_Precision_Infinite;
            else theXmax = Xmax + Gap;
            if (IsOpenYmin()) theYmin = -Bnd_Precision_Infinite;
            else theYmin = Ymin - Gap;
            if (IsOpenYmax()) theYmax = Bnd_Precision_Infinite;
            else theYmax = Ymax + Gap;
            if (IsOpenZmin()) theZmin = -Bnd_Precision_Infinite;
            else theZmin = Zmin - Gap;
            if (IsOpenZmax()) theZmax = Bnd_Precision_Infinite;
            else theZmax = Zmax + Gap;
        }


        //! Returns true if this bounding box is open in the  Xmin direction.
        bool IsOpenXmin() { return (Flags & MaskFlags.XminMask) != 0; }

        //! Returns true if this bounding box is open in the  Xmax direction.
        bool IsOpenXmax() { return (Flags & MaskFlags.XmaxMask) != 0; }

        //! Returns true if this bounding box is open in the  Ymix direction.
        bool IsOpenYmin() { return (Flags & MaskFlags.YminMask) != 0; }

        //! Returns true if this bounding box is open in the  Ymax direction.
        bool IsOpenYmax() { return (Flags & MaskFlags.YmaxMask) != 0; }

        //! Returns true if this bounding box is open in the  Zmin direction.
        bool IsOpenZmin() { return (Flags & MaskFlags.ZminMask) != 0; }

        //! Returns true if this bounding box is open in the  Zmax  direction.
        bool IsOpenZmax() { return (Flags & MaskFlags.ZmaxMask) != 0; }



    }//! Bit flags.
}