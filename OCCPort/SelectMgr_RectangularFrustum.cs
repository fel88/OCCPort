using OCCPort;

namespace OCCPort
{//! This class contains representation of rectangular selecting frustum, created in case
 //! of point and box selection, and algorithms for overlap detection between selecting
 //! frustum and sensitive entities. The principle of frustum calculation:
 //! - for point selection: on a near view frustum plane rectangular neighborhood of
 //!                        user-picked point is created according to the pixel tolerance
 //!                        given and then this rectangle is projected onto far view frustum
 //!                        plane. This rectangles define the parallel bases of selecting frustum;
 //! - for box selection: box points are projected onto near and far view frustum planes.
 //!                      These 2 projected rectangles define parallel bases of selecting frustum.
 //! Overlap detection tests are implemented according to the terms of separating axis
 //! theorem (SAT).
    public class SelectMgr_RectangularFrustum : SelectMgr_Frustum //<4>

    {
        //! Auxiliary structure to define selection primitive (point or box)
        //! In case of point selection min and max points are identical.
        public struct SelectionRectangle
        {
            public SelectionRectangle()
            {
                //: myMinPnt(gp_Pnt2d(RealLast(), RealLast())),
                //   myMaxPnt(gp_Pnt2d(RealLast(), RealLast())) 
            }

            public gp_Pnt2d MousePos() { return myMinPnt; }
            public void SetMousePos(gp_Pnt2d thePos) { myMinPnt = thePos; myMaxPnt = thePos; }

            public gp_Pnt2d MinPnt() { return myMinPnt; }
            public void SetMinPnt(gp_Pnt2d theMinPnt) { myMinPnt = theMinPnt; }

            public gp_Pnt2d MaxPnt() { return myMaxPnt; }
            public void SetMaxPnt(gp_Pnt2d theMaxPnt) { myMaxPnt = theMaxPnt; }


            gp_Pnt2d myMinPnt;
            gp_Pnt2d myMaxPnt;
        }

        public void Init(gp_Pnt2d theMinPnt,
                                                 gp_Pnt2d theMaxPnt)
        {
            mySelectionType = SelectMgr_SelectionType.SelectMgr_SelectionType_Box;
            mySelRectangle.SetMinPnt(theMinPnt);
            mySelRectangle.SetMaxPnt(theMaxPnt);
        }
        SelectionRectangle mySelRectangle;              //!< parameters for selection by point or box (it is used to build frustum)
        gp_Pnt myNearPickedPnt;             //!< 3d projection of user-picked selection point onto near view plane
        gp_Pnt myFarPickedPnt;              //!< 3d projection of user-picked selection point onto far view plane
        gp_Dir myViewRayDir;                //!< view ray direction
        double myScale;                     //!< Scale factor of applied transformation, if there was any


    }
}