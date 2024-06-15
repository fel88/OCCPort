using System;

namespace OCCPort
{
    public class Bnd_Box
    {
        //! Bit flags.
        [Flags]
        enum MaskFlags
        {
            VoidMask = 0x01,
            XminMask = 0x02,
            XmaxMask = 0x04,
            YminMask = 0x08,
            YmaxMask = 0x10,
            ZminMask = 0x20,
            ZmaxMask = 0x40,
            WholeMask = 0x7e
        };


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


        //! Returns true if this bounding box is empty (Void flag).
        public bool IsVoid()
        {
            return (Flags.HasFlag(MaskFlags.VoidMask));
        }
    }
}