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
    public struct NCollection_Vec4<Element_t> where Element_t : struct, INumber<Element_t>, IMultiplyOperators<Element_t, Element_t, Element_t>
    {
        public NCollection_Vec4()
        {
            v = new Element_t[4];
        }


        public NCollection_Vec4(NCollection_Vec3<Element_t> theVec3, Element_t theW)
        {
            v = new Element_t[4];

            v[0] = theVec3.x();
            v[1] = theVec3.y();
            v[2] = theVec3.z();
            v[3] = theW;
        }

        //! Initialize ALL components of vector within specified value.
        public NCollection_Vec4(Element_t theValue)
        {
            v = new Element_t[4];
            v[0] = v[1] = v[2] = v[3] = theValue;
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

        //! Assign new values as 3-component vector and a 4-th value.
        public void SetValues(NCollection_Vec3<Element_t> theVec3, Element_t theW)
        {
            v[0] = theVec3.x();
            v[1] = theVec3.y();
            v[2] = theVec3.z();
            v[3] = theW;
        }
        public void SetValues(Element_t x, Element_t y, Element_t z, Element_t w)
        {
            v[0] = x;
            v[1] = y;
            v[2] = z;
            v[3] = w;
        }
        public NCollection_Vec4(Element_t[] array)
        {
            v = new Element_t[4];
            for (int i = 0; i < array.Length; i++)
            {
                v[i] = array[i];
            }
        }
        public Element_t[] GetData()
        {
            return v;
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
        public void w(Element_t val) { v[3] = val; }

        //! Alias to 1st component as RED channel in RGBA.
        public Element_t r() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XYZW.
        public Element_t y() { return v[1]; }

        //! Alias to 2nd component as GREEN channel in RGBA.
        public Element_t g() { return v[1]; }
        public void g(Element_t val) { v[1] = val; }

        //! Alias to 3rd component as Z coordinate in XYZW.
        public Element_t z() { return v[2]; }

        //! Alias to 3rd component as BLUE channel in RGBA.
        public Element_t b() { return v[2]; }
        public void b(Element_t val) { v[2] = val; }

        //! Alias to 4th component as W coordinate in XYZW.
        public Element_t w() { return v[3]; }

        //! Alias to 4th component as ALPHA channel in RGBA.
        public Element_t a() { return v[3]; }
        public void a(Element_t val) { v[3] = val; }

        public Element_t[] ChangeData()
        {
            return v;
        }
        public NCollection_Vec3<Element_t> xyz()
        {
            return new NCollection_Vec3<Element_t>(x(), y(), z());
        }

        public static NCollection_Vec4<Element_t> operator +(NCollection_Vec4<Element_t> temp, NCollection_Vec4<Element_t> temp2)
        {
            return new NCollection_Vec4<Element_t>(temp.x() + temp2.x(), temp.y() + temp2.y(), temp.z() + temp2.z(), temp.w() + temp2.w());
        }
        public static NCollection_Vec4<Element_t> operator -(NCollection_Vec4<Element_t> temp)
        {
            return new NCollection_Vec4<Element_t>(-temp.x(), -temp.y(), -temp.z(), -temp.w());
        }

        public static NCollection_Vec4<Element_t> operator *(NCollection_Vec4<Element_t> temp, Element_t temp2)
        {
            return new NCollection_Vec4<Element_t>(temp.x() * temp2, temp.y() * temp2, temp.z() * temp2, temp.w() * temp2);
        }

    }
}
