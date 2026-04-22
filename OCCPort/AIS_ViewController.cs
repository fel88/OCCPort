using OCCPort;
using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    public class AIS_ViewController
    {

        //! Handle view redraw.
        //! This method is expected to be called from rendering thread.
        public void handleViewRedraw(AIS_InteractiveContext ctx, V3d_View theView)
        {
            V3d_View aParentView = theView.IsSubview() ? theView.ParentView() : theView;

            // manage animation state
//            if (!myViewAnimation.IsNull()
//             && !myViewAnimation->IsStopped())
//            {
//                myViewAnimation->UpdateTimer();
//                ResetPreviousMoveTo();
//                setAskNextFrame();
//            }
//
/*
            if (!myObjAnimation.IsNull()
             && !myObjAnimation->IsStopped())
            {
                myObjAnimation->UpdateTimer();
                ResetPreviousMoveTo();
                setAskNextFrame();
            }*/

            if (myIsContinuousRedraw)
            {
                myToAskNextFrame = true;
            }
            if (theView.View().IsActiveXR())
            {
                // VR requires continuous rendering
                myToAskNextFrame = true;
            }

            for (int aSubViewPass = 0; aSubViewPass < 2; ++aSubViewPass)
            {
                bool isSubViewPass = (aSubViewPass == 0);
                foreach (var aView in theView.Viewer().ActiveViews())
                {
                    
                    if (isSubViewPass
                    && !aView.IsSubview())
                    {
                        foreach (var aSubviewIter in aView.Subviews())
                        {


                            if (aSubviewIter.Viewer() != theView.Viewer())
                            {
                                if (aSubviewIter.IsInvalidated())
                                {
                                   // if (aSubviewIter.ComputedMode())
                                    {
                                       // aSubviewIter.Update();
                                    }
                                  //  else
                                    {
                                        aSubviewIter.Redraw();
                                    }
                                }
                                else if (aSubviewIter.IsInvalidatedImmediate())
                                {
                                    aSubviewIter.RedrawImmediate();
                                }
                            }
                        }
                        continue;
                    }
                    else if (!isSubViewPass
                           && aView.IsSubview())
                    {
                        continue;
                    }

                    if (aView.IsInvalidated()
                     || (myToAskNextFrame && aView == theView))
                    {
                        if (aView.ComputedMode())
                        {
                            aView.Update();
                        }
                        else
                        {
                            aView.Redraw();
                        }

                        if (aView.IsSubview())
                        {
                            aView.ParentView().InvalidateImmediate();
                        }
                    }
                    else if (aView.IsInvalidatedImmediate())
                    {
                        if (aView.IsSubview())
                        {
                            aView.ParentView().InvalidateImmediate();
                        }

                        aView.RedrawImmediate();
                    }
                }
            }
            if (theView.IsSubview()
             && theView.Viewer() != aParentView.Viewer())
            {
                aParentView.RedrawImmediate();
            }

            if (myToAskNextFrame)
            {
                // ask more frames
                //??aParentView.Window().InvalidateContent(Handle(Aspect_DisplayConnection)());
            }

        }
        //! Reset previous position of MoveTo.
        void ResetPreviousMoveTo() { myPrevMoveTo = new Graphic3d_Vec2i(-1); }

        //! Pick closest point under mouse cursor.
        //! This method is expected to be called from rendering thread.
        //! @param thePnt   [out] result point
        //! @param theCtx    [in] interactive context
        //! @param theView   [in] active view
        //! @param theCursor [in] mouse cursor
        //! @param theToStickToPickRay [in] when TRUE, the result point will lie on picking ray
        //! @return TRUE if result has been found
        public bool PickPoint(ref gp_Pnt thePnt,
                                          AIS_InteractiveContext theCtx,
                                          V3d_View theView,
                                          Graphic3d_Vec2i theCursor,
                                          bool theToStickToPickRay)
        {


            ResetPreviousMoveTo();

            SelectMgr_ViewerSelector aSelector = theCtx.MainSelector();
            aSelector.Pick(theCursor.x(), theCursor.y(), theView);
            if (aSelector.NbPicked() < 1)
            {
                return false;
            }

            SelectMgr_SortCriterion aPicked = aSelector.PickedData(1);
            if (theToStickToPickRay
            && !Precision.IsInfinite(aPicked.Depth))
            {
                thePnt = aSelector.GetManager().DetectedPoint(aPicked.Depth);
            }
            else
            {
                thePnt = aSelector.PickedPoint(1);
            }
            return !Precision.IsInfinite(thePnt.X())
                && !Precision.IsInfinite(thePnt.Y())
                && !Precision.IsInfinite(thePnt.Z());
        }

        //! Compute rotation gravity center point depending on rotation mode.
        //! This method is expected to be called from rendering thread.
        public gp_Pnt GravityPoint(AIS_InteractiveContext theCtx, V3d_View theView)
        {
            switch (myRotationMode)
            {
                case AIS_RotationMode.AIS_RotationMode_PickLast:
                case AIS_RotationMode.AIS_RotationMode_PickCenter:
                    {
                        Graphic3d_Vec2i aCursor = new Graphic3d_Vec2i((int)myGL.OrbitRotation.PointStart.x(), (int)myGL.OrbitRotation.PointStart.y());
                        if (myRotationMode == AIS_RotationMode.AIS_RotationMode_PickCenter)
                        {
                            Graphic3d_Vec2i aViewPort = new Graphic3d_Vec2i();
                            int xx;
                            int yy;
                            //theView.Window().Size(aViewPort.x(), aViewPort.y());
                            theView.Window().Size(out xx, out yy);
                            aViewPort = new Graphic3d_Vec2i(xx, yy);
                            aCursor = aViewPort / 2;
                        }

                        gp_Pnt aPnt = new gp_Pnt();
                        if (PickPoint(ref aPnt, theCtx, theView, aCursor, myToStickToRayOnRotation))
                        {
                            return aPnt;
                        }
                        break;
                    }
                case AIS_RotationMode.AIS_RotationMode_CameraAt:
                    {
                        Graphic3d_Camera aCam = theView.Camera();
                        return aCam.Center();
                    }
                case AIS_RotationMode.AIS_RotationMode_BndBoxScene:
                    {
                        Bnd_Box aBndBox = theView.View().MinMaxValues(false);
                        if (!aBndBox.IsVoid())
                        {
                            var r = ((aBndBox.CornerMin().XYZ() + aBndBox.CornerMax().XYZ()) * 0.5);
                            return new gp_Pnt(r);
                        }
                        break;
                    }
                case AIS_RotationMode.AIS_RotationMode_BndBoxActive:
                    break;
            }

            return theCtx.GravityPoint(theView);
        }

        public void flushBuffers(AIS_InteractiveContext ctx,
                                         V3d_View view)
        {
            myToAskNextFrame = false;

            //myGL.IsNewGesture = myUI.IsNewGesture;
            //    myUI.IsNewGesture = false;

        }
        //! Update buffer for rendering thread.
        //! This method is expected to be called within synchronization barrier between GUI
        //! and Rendering threads (e.g. GUI thread should be locked beforehand to avoid data races).
        //! @param theCtx interactive context
        //! @param theView active view
        //! @param theToHandle if TRUE, the HandleViewEvents() will be called
        public void FlushViewEvents(AIS_InteractiveContext theCtx,
                                                 V3d_View theView,
                                                bool theToHandle = false)
        {
            flushBuffers(theCtx, theView);
            flushGestures(theCtx, theView);

            //if (theView.IsSubview())
            //{
            //    // move input coordinates inside the view
            //    const Graphic3d_Vec2i aDelta = theView->View()->SubviewTopLeft();
            //    if (myGL.MoveTo.ToHilight || myGL.Dragging.ToStart)
            //    {
            //        myGL.MoveTo.Point -= aDelta;
            //    }
            //    if (myGL.Panning.ToStart)
            //    {
            //        myGL.Panning.PointStart -= aDelta;
            //    }
            //    if (myGL.Dragging.ToStart)
            //    {
            //        myGL.Dragging.PointStart -= aDelta;
            //    }
            //    if (myGL.Dragging.ToMove)
            //    {
            //        myGL.Dragging.PointTo -= aDelta;
            //    }
            //    if (myGL.OrbitRotation.ToStart)
            //    {
            //        myGL.OrbitRotation.PointStart -= Graphic3d_Vec2d(aDelta);
            //    }
            //    if (myGL.OrbitRotation.ToRotate)
            //    {
            //        myGL.OrbitRotation.PointTo -= Graphic3d_Vec2d(aDelta);
            //    }
            //    if (myGL.ViewRotation.ToStart)
            //    {
            //        myGL.ViewRotation.PointStart -= Graphic3d_Vec2d(aDelta);
            //    }
            //    if (myGL.ViewRotation.ToRotate)
            //    {
            //        myGL.ViewRotation.PointTo -= Graphic3d_Vec2d(aDelta);
            //    }
            //    for (Graphic3d_Vec2i & aPntIter : myGL.Selection.Points)
            //    {
            //        aPntIter -= aDelta;
            //    }
            //    for (Aspect_ScrollDelta & aZoomIter : myGL.ZoomActions)
            //    {
            //        aZoomIter.Point -= aDelta;
            //    }
            //}

            if (theToHandle)
            {
                HandleViewEvents(theCtx, theView);
            }
        }

        private void flushGestures(AIS_InteractiveContext theCtx, V3d_View theView)
        {
            
        }

        //! Process events within rendering thread.
        public void HandleViewEvents(AIS_InteractiveContext theCtx, V3d_View theView)
        {
            bool wasImmediateUpdate = theView.SetImmediateUpdate(false);


            V3d_View aPickedView;
            //if (theView.IsSubview()
            //   || !theView.Subviews().IsEmpty())
            //{
            //    // activate another subview on mouse click
            //    bool toPickSubview = false;
            //    Graphic3d_Vec2i aClickPoint;
            //    if (myGL.Selection.Tool == AIS_ViewSelectionTool_Picking
            //    && !myGL.Selection.Points.IsEmpty())
            //    {
            //        aClickPoint = myGL.Selection.Points.Last();
            //        toPickSubview = true;
            //    }
            //    else if (!myGL.ZoomActions.IsEmpty())
            //    {
            //        //aClickPoint = myGL.ZoomActions.Last().Point;
            //        //toPickSubview = true;
            //    }

            //    if (toPickSubview)
            //    {
            //        if (theView->IsSubview())
            //        {
            //            aClickPoint += theView->View()->SubviewTopLeft();
            //        }
            //        Handle(V3d_View) aParent = !theView->IsSubview() ? theView : theView->ParentView();
            //        aPickedView = aParent->PickSubview(aClickPoint);
            //    }
            //}

            //handleViewOrientationKeys(theCtx, theView);
            //const AIS_WalkDelta aWalk = handleNavigationKeys(theCtx, theView);
            //handleXRInput(theCtx, theView, aWalk);
            //if (theView->View()->IsActiveXR())
            //{
            //    theView->View()->SetupXRPosedCamera();
            //}
            //handleMoveTo(theCtx, theView);
            //handleCameraActions(theCtx, theView, aWalk);
            //theView->View()->SynchronizeXRPosedToBaseCamera(); // handleCameraActions() may modify posed camera position - copy this modifications also to the base camera
            //handleXRPresentations(theCtx, theView);

            handleViewRedraw(theCtx, theView);
            //theView->View()->UnsetXRPosedCamera();

            theView.SetImmediateUpdate(wasImmediateUpdate);

            //if (!aPickedView.IsNull()
            //  && aPickedView != theView)
            //{
            //    OnSubviewChanged(theCtx, theView, aPickedView);
            //}

            //// make sure to not process the same events twice
            myGL.Reset();
            //myToAskNextFrame = false;
        }

        AIS_ViewInputBuffer myUI=new AIS_ViewInputBuffer ();                       //!< buffer for UI thread
        AIS_ViewInputBuffer myGL=new AIS_ViewInputBuffer ();                       //!< buffer for rendering thread


        double myLastEventsTime;           //!< last fetched events timer value for computing delta/progress
        bool myToAskNextFrame;           //!< flag indicating that another frame should be drawn right after this one
        bool myIsContinuousRedraw;       //!< continuous redrawing (without immediate rendering optimization)

        double myMinCamDistance;           //!< minimal camera distance for zoom operation
        public AIS_RotationMode myRotationMode;             //!< rotation mode
                                                            //  AIS_NavigationMode myNavigationMode;           //!< navigation mode (orbit rotation / first person)
                                                            //  Standard_ShortReal myMouseAccel;               //!< mouse input acceleration ratio in First Person mode
                                                            //  Standard_ShortReal myOrbitAccel;               //!< Orbit rotation acceleration ratio
        bool myToShowPanAnchorPoint;     //!< option displaying panning  anchor point
        bool myToShowRotateCenter;       //!< option displaying rotation center point
        bool myToLockOrbitZUp;           //!< force camera up orientation within AIS_NavigationMode_Orbit rotation mode
        bool myToInvertPitch;            //!< flag inverting pitch direction while processing Aspect_VKey_NavLookUp/Aspect_VKey_NavLookDown
        bool myToAllowTouchZRotation;    //!< enable z-rotation two-touches gesture; FALSE by default
        bool myToAllowRotation;          //!< enable rotation; TRUE by default
        bool myToAllowPanning;           //!< enable panning; TRUE by default
        bool myToAllowZooming;           //!< enable zooming; TRUE by default
        bool myToAllowZFocus;            //!< enable ZFocus change; TRUE by default
        bool myToAllowHighlight;         //!< enable dynamic highlight on mouse move; TRUE by default
        bool myToAllowDragging;          //!< enable dragging object; TRUE by default
        bool myToStickToRayOnZoom;       //!< project picked point to ray while zooming at point, TRUE by default
        bool myToStickToRayOnRotation;   //!< project picked point to ray while rotating around point; TRUE by default

        //Standard_ShortReal myWalkSpeedAbsolute;        //!< normal walking speed, in m/s; 1.5 by default
        //  Standard_ShortReal myWalkSpeedRelative;        //!< walking speed relative to scene bounding box; 0.1 by default
        // Standard_ShortReal myThrustSpeed;              //!< active thrust value
        bool myHasThrust;                //!< flag indicating active thrust

        //AIS_AnimationCamera myViewAnimation;    //!< view animation
        //AIS_Animation myObjAnimation;     //!< objects animation
        bool myToPauseObjAnimation;   //!< flag to pause objects animation on mouse click; FALSE by default
                                      //AIS_RubberBand myRubberBand;            //!< Rubber-band presentation
        SelectMgr_EntityOwner myDragOwner;      //!< detected owner of currently dragged object
        AIS_InteractiveObject myDragObject;     //!< currently dragged object
        Graphic3d_Vec2i myPrevMoveTo;               //!< previous position of MoveTo event in 3D viewer
        bool myHasHlrOnBeforeRotation;   //!< flag for restoring Computed mode after rotation


    }



}
