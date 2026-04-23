using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    public class Graphic3d_TransformPers
    {
        internal bool IsTrihedronOr2d(Graphic3d_TransModeFlags theMode)
        {
            return (theMode & (Graphic3d_TransModeFlags.Graphic3d_TMF_TriedronPers | Graphic3d_TransModeFlags.Graphic3d_TMF_2d)) != 0;

        }
        //! Return true for Graphic3d_TMF_ZoomPers, Graphic3d_TMF_ZoomRotatePers or Graphic3d_TMF_RotatePers modes.
        public bool IsZoomOrRotate() { return IsZoomOrRotate(myMode); }
        //! Return true if specified mode is zoom/rotate transformation persistence.
        public static bool IsZoomOrRotate(Graphic3d_TransModeFlags theMode)
        {
            return (theMode & (Graphic3d_TransModeFlags.Graphic3d_TMF_ZoomPers | Graphic3d_TransModeFlags.Graphic3d_TMF_RotatePers)) != 0;
        }
        //template<class T>
        public void Apply(Graphic3d_Camera theCamera,
                                             NCollection_Mat4 theProjection,
                                             NCollection_Mat4 theWorldView,
                                             int theViewportWidth,
                                             int theViewportHeight,
                                           ref  BVH_Box theBoundingBox)
        {
            //NCollection_Mat4<T> aTPers = Compute(theCamera, theProjection, theWorldView, theViewportWidth, theViewportHeight);
            //if (aTPers.IsIdentity()
            //|| !theBoundingBox.IsValid())
            //{
            //    return;
            //}

            //const typename BVH_Box<T, 3 >::BVH_VecNt & aMin = theBoundingBox.CornerMin();
            //const typename BVH_Box<T, 3 >::BVH_VecNt & aMax = theBoundingBox.CornerMax();

            //typename BVH_Box<T, 4 >::BVH_VecNt anArrayOfCorners[8];
            //anArrayOfCorners[0] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMin.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[1] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMin.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[2] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMax.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[3] = typename BVH_Box < T, 4 >::BVH_VecNt(aMin.x(), aMax.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[4] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMin.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[5] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMin.y(), aMax.z(), static_cast<T>(1.0));
            //anArrayOfCorners[6] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMax.y(), aMin.z(), static_cast<T>(1.0));
            //anArrayOfCorners[7] = typename BVH_Box < T, 4 >::BVH_VecNt(aMax.x(), aMax.y(), aMax.z(), static_cast<T>(1.0));

            //theBoundingBox.Clear();
            //for (int anIt = 0; anIt < 8; ++anIt)
            //{
            //    typename BVH_Box<T, 4 >::BVH_VecNt & aCorner = anArrayOfCorners[anIt];
            //    aCorner = aTPers * aCorner;
            //    aCorner = aCorner / aCorner.w();
            //    theBoundingBox.Add(typename BVH_Box < T, 3 >::BVH_VecNt(aCorner.x(), aCorner.y(), aCorner.z()));
            //}
        }

        //! Return the anchor point for zoom/rotate transformation persistence.
        public gp_Pnt AnchorPoint()
        {
            if (!IsZoomOrRotate())
            {
                throw new Standard_ProgramError("Graphic3d_TransformPers::AnchorPoint(), wrong persistence mode.");
            }

            return new gp_Pnt(myParams.Params3d.PntX, myParams.Params3d.PntY, myParams.Params3d.PntZ);
        }

        //! 3D anchor point for zoom/rotate transformation persistence.
        public struct PersParams3d
        {
            public double PntX;
            public double PntY;
            public double PntZ;

        }
        public struct PersParams2d
        {
            public int OffsetX;
            public int OffsetY;
            public Aspect_TypeOfTriedronPosition Corner;

        }
        //union
        public class MyParamsUnion
        {
            //  {
            public PersParams3d Params3d;
            public PersParams2d Params2d;
            //  }
        }
        MyParamsUnion myParams = new MyParamsUnion();
        Graphic3d_TransModeFlags myMode;  //!< Transformation persistence mode flags

        //! Return true for Graphic3d_TMF_TriedronPers and Graphic3d_TMF_2d modes.
        public bool IsTrihedronOr2d() { return IsTrihedronOr2d(myMode); }
    }


}