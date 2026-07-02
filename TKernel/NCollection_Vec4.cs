using System.Numerics;

namespace TKernel
{   //! Generic 4-components vector.
    //! To be used as RGBA color vector or XYZW 3D-point with special W-component
    //! for operations with projection / model view matrices.
    //! Use this class for 3D-points carefully because declared W-component may
    //! results in incorrect results if used without matrices.
    //   public class NCollection_Vec4<T> where T : struct, INumber<T>, IMultiplyOperators<T, T, T>
    //   {
    //	public NCollection_Vec4()
    //	{
    //		v = new double[4];
    //	}
    //	//! Per-component constructor.
    //	public NCollection_Vec4(double theX,
    //						double theY,
    //						double theZ,
    //						double theW)
    //	{
    //		v = new double[4];
    //		v[0] = theX;
    //		v[1] = theY;
    //		v[2] = theZ;
    //		v[3] = theW;
    //	}


    //	protected double[] v; //!< define the vector as array to avoid structure alignment issues
    //						  //! Alias to 1st component as X coordinate in XYZW.
    //	public double x() { return v[0]; }

    //	//! Alias to 1st component as RED channel in RGBA.
    //	public double r() { return v[0]; }

    //	//! Alias to 2nd component as Y coordinate in XYZW.
    //	public double y() { return v[1]; }

    //	//! Alias to 2nd component as GREEN channel in RGBA.
    //	public double g() { return v[1]; }

    //	//! Alias to 3rd component as Z coordinate in XYZW.
    //	public double z() { return v[2]; }

    //	//! Alias to 3rd component as BLUE channel in RGBA.
    //	public double b() { return v[2]; }

    //	//! Alias to 4th component as W coordinate in XYZW.
    //	public double w() { return v[3]; }

    //	//! Alias to 4th component as ALPHA channel in RGBA.
    //	public double a() { return v[3]; }


    //}
    public struct NCollection_Vec4<Element_t> where Element_t : struct
    {
        public NCollection_Vec4()
        {
            v = new Element_t[4];
        }
        //! Per-component constructor.
        public NCollection_Vec4(Element_t theX,
                            Element_t theY,
                            Element_t theZ,
                            Element_t theW)
        {
            v = new Element_t[4];
            v[0] = theX;
            v[1] = theY;
            v[2] = theZ;
            v[3] = theW;
        }


        public Element_t X { get => v[0]; set => v[0] = value; }
        public Element_t Y { get => v[1]; set => v[1] = value; }
        public Element_t Z { get => v[2]; set => v[2] = value; }
        public Element_t W { get => v[3]; set => v[3] = value; }
        public Element_t[] v; //!< define the vector as array to avoid structure alignment issues
                              //! Alias to 1st component as X coordinate in XYZW.
        public Element_t x() { return v[0]; }
        public void x(Element_t val) { v[0] = val; }
        public void y(Element_t val) { v[1] = val; }
        public void z(Element_t val) { v[2] = val; }

        //! Alias to 1st component as RED channel in RGBA.
        public Element_t r() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XYZW.
        public Element_t y() { return v[1]; }

        //! Alias to 2nd component as GREEN channel in RGBA.
        public Element_t g() { return v[1]; }

        //! Alias to 3rd component as Z coordinate in XYZW.
        public Element_t z() { return v[2]; }

        //! Alias to 3rd component as BLUE channel in RGBA.
        public Element_t b() { return v[2]; }

        //! Alias to 4th component as W coordinate in XYZW.
        public Element_t w() { return v[3]; }

        //! Alias to 4th component as ALPHA channel in RGBA.
        public Element_t a() { return v[3]; }

        public Element_t[] ChangeData()
        {
            return v;
        }
    }
}
