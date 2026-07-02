namespace TKV3d
{
    //! Interactive object for displaying the view manipulation cube.
    //!
    //! View cube consists of several parts that are responsible for different camera manipulations:
    //! @li Cube sides represent main views: top, bottom, left, right, front and back.
    //! @li Edges represent rotation of one of main views on 45 degrees.
    //! @li Vertices represent rotation of one of man views in two directions.
    //!
    //! The object is expected to behave like a trihedron in the view corner,
    //! therefore its position should be defined using transformation persistence flags:
    //! @code SetTransformPersistence (new Graphic3d_TransformPers (Graphic3d_TMF_TriedronPers, Aspect_TOTP_LEFT_LOWER, Graphic3d_Vec2i (100, 100)); @endcode
    //!
    //! View Cube parts are sensitive to detection, or dynamic highlighting (but not selection),
    //! and every its owner AIS_ViewCubeOwner corresponds to camera transformation.
    //! @code
    //!   for (aViewCube->StartAnimation (aDetectedOwner); aViewCube->HasAnimation(); )
    //!   {
    //!     aViewCube->UpdateAnimation();
    //!     ... // updating of application window
    //!   }
    //! @endcode
    //! or
    //! @code aViewCube->HandleClick (aDetectedOwner); @endcode
    //! that includes transformation loop.
    //! This loop allows external actions like application updating. For this purpose AIS_ViewCube has virtual interface onAfterAnimation(),
    //! that is to be redefined on application level.
    public class AIS_ViewCube : AIS_InteractiveObject
    {
        public override void Compute(PrsMgr_PresentationManager myPresentationManager, Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        {
            //throw new NotImplementedException();
        }

        public override void ComputeSelection(SelectMgr_Selection theSelection, int theMode)
        {
          //  throw new NotImplementedException();
        }
    }
}

