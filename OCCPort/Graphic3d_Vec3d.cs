global using Graphic3d_Vec3d = OCCPort.NCollection_Vec3<double>;
global using Graphic3d_Vec3 = OCCPort.NCollection_Vec3<float>;
global using Graphic3d_Vec3i = OCCPort.NCollection_Vec3<int>;
global using Graphic3d_Vec4d = OCCPort.NCollection_Vec4<double>;
global using Graphic3d_Vec4 = OCCPort.NCollection_Vec4<float>;
global using BVH_Vec3d  = OCCPort.NCollection_Vec3<double>;
global using IFacePtr = OCCPort.Interfaces.IMeshData_Face;
global using IWireHandle = OCCPort.Interfaces.IMeshData_Wire;



using OpenTK.Mathematics;

namespace OCCPort
{
    
    //public class Graphic3d_Vec3d : NCollection_Vec3<double>, IVectorType
    //{

    //    public Graphic3d_Vec3d() { }

    //    //! Compute maximum component of the vector.
    //    public double maxComp()
    //    {
    //        return v[0] > v[1] ? (v[0] > v[2] ? v[0] : v[2])
    //                           : (v[1] > v[2] ? v[1] : v[2]);
    //    }
    //    public Graphic3d_Vec3d(double v1, double v2, double v3) : base(v1, v2, v3)
    //    {

    //    }
    //    public Graphic3d_Vec3d(Graphic3d_Vec3d v1)
    //    {
    //        v[0] = v1.x();
    //        v[1] = v1.y();
    //        v[2] = v1.z();
    //    }
    //    //! Computes the square of vector modulus (magnitude, length).
    //    //! This method may be used for performance tricks.
    //    public double SquareModulus()
    //    {
    //        return x() * x() + y() * y() + z() * z();
    //    }

    //    public Graphic3d_Vec3d(double v1) : base(v1)
    //    {

    //    }
    //    public static Graphic3d_Vec3d operator -(Graphic3d_Vec3d theLeft, Graphic3d_Vec3d theRight)
    //    {
    //        return new Graphic3d_Vec3d(theLeft.X - theRight.X,
    //            theLeft.Y - theRight.Y,
    //            theLeft.Z - theRight.Z);
    //    }

    //    public static Graphic3d_Vec3d operator +(Graphic3d_Vec3d theLeft, Graphic3d_Vec3d theRight)
    //    {
    //        return new Graphic3d_Vec3d(theLeft.X + theRight.X,
    //         theLeft.Y + theRight.Y,
    //         theLeft.Z + theRight.Z);
    //    }
    //    public static Graphic3d_Vec3d operator *(Graphic3d_Vec3d theLeft, Graphic3d_Vec3d theRight)
    //    {
    //        return new Graphic3d_Vec3d(theLeft.X * theRight.X,
    //        theLeft.Y * theRight.Y,
    //        theLeft.Z * theRight.Z);
    //    }
    //    public static Graphic3d_Vec3d operator *(Graphic3d_Vec3d theLeft, double theRight)
    //    {
    //        return new Graphic3d_Vec3d(theLeft.X * theRight,
    //          theLeft.Y * theRight,
    //          theLeft.Z * theRight);
    //    }

    //    public double X { get => x(); set => v[0] = value; }
    //    public double Y { get => v[1]; set => v[1] = value; }
    //    public double Z { get => v[2]; set => v[2] = value; }

    //    public Graphic3d_Vec3d cwiseMax(Graphic3d_Vec3d theVec) 
    //    {
    //        return new Graphic3d_Vec3d(v[0] > theVec.v[0] ? v[0] : theVec.v[0],
    //                        v[1] > theVec.v[1] ? v[1] : theVec.v[1],
    //                        v[2] > theVec.v[2] ? v[2] : theVec.v[2]);
    //    }

    //    public Graphic3d_Vec3d cwiseMin(Graphic3d_Vec3d theVec) 
    //    {
    //        return new Graphic3d_Vec3d(v[0] < theVec.v[0] ? v[0] : theVec.v[0],
    //                        v[1] < theVec.v[1] ? v[1] : theVec.v[1],
    //                        v[2] < theVec.v[2] ? v[2] : theVec.v[2]);
    //    }

    //    public IVectorType cwiseMax(IVectorType thePoint)
    //    {            

    //        throw new System.NotImplementedException();
    //    }

    //    public IVectorType cwiseMin(IVectorType thePoint)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
