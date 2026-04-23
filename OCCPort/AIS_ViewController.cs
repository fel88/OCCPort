using OCCPort;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static OCCPort.Graphic3d_Camera;

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
        double myMouseClickThreshold;      //!< mouse click threshold in pixels; 3 by default
        double myMouseDoubleClickInt;      //!< double click interval in seconds; 0.4 by default
        float myScrollZoomRatio;          //!< distance ratio for mapping mouse scroll event to zoom; 15.0 by default

        public bool UpdateMouseScroll(Aspect_ScrollDelta theDelta)
        {
            Aspect_ScrollDelta aDelta = theDelta;
            aDelta.Delta *= myScrollZoomRatio;
            return UpdateZoom(aDelta);
        }


        public bool UpdateZoom(Aspect_ScrollDelta theDelta)
        {
            if (!myUI.ZoomActions.IsEmpty())
            {
                var last = myUI.ZoomActions.Last();
                //if (myUI.ZoomActions.ChangeLast().Point == theDelta.Point)
                if (last.Point == theDelta.Point)
                {
                    //myUI.ZoomActions.ChangeLast().Delta += theDelta.Delta;
                    last.Delta += theDelta.Delta;
                    return false;
                }
            }

            myUI.ZoomActions.Append(theDelta);
            return true;
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
            AIS_WalkDelta aWalk = handleNavigationKeys(theCtx, theView);
            //handleXRInput(theCtx, theView, aWalk);
            //if (theView->View()->IsActiveXR())
            //{
            //    theView->View()->SetupXRPosedCamera();
            //}
            //handleMoveTo(theCtx, theView);
            handleCameraActions(theCtx, theView, aWalk);
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

        AIS_Point myAnchorPointPrs1;          //!< anchor point presentation (Graphic3d_ZLayerId_Top)
        AIS_Point myAnchorPointPrs2;          //!< anchor point presentation (Graphic3d_ZLayerId_Topmost)
        private AIS_WalkDelta handleNavigationKeys(AIS_InteractiveContext theCtx, V3d_View theView)
        {
            // navigation keys
            double aCrouchRatio = 1.0, aRunRatio = 1.0;
            if (myNavigationMode == AIS_NavigationMode.AIS_NavigationMode_FirstPersonFlight)
            {
                aRunRatio = 3.0;
            }

            double aRotSpeed = 0.5;
            double aWalkSpeedCoef = WalkSpeedRelative();
            AIS_WalkDelta aWalk = FetchNavigationKeys(aCrouchRatio, aRunRatio);
            /*
             ...............
             */

            return new AIS_WalkDelta();
        }

        AIS_WalkDelta FetchNavigationKeys(double theCrouchRatio,
                                                       double theRunRatio)
        {
            AIS_WalkDelta aWalk = new AIS_WalkDelta();
            return aWalk;

        }
        //! Return walking speed relative to scene bounding box; 0.1 by default.
        public float WalkSpeedRelative() { return myWalkSpeedRelative; }
        float myWalkSpeedAbsolute;        //!< normal walking speed, in m/s; 1.5 by default
        float myWalkSpeedRelative;        //!< walking speed relative to scene bounding box; 0.1 by default
        float myThrustSpeed;              //!< active thrust value


        AIS_NavigationMode myNavigationMode;           //!< navigation mode (orbit rotation / first person)
        private void handleCameraActions(AIS_InteractiveContext theCtx, V3d_View theView, AIS_WalkDelta theWalk)
        {
            // apply view actions
            if (myGL.Orientation.ToSetViewOrient)
            {
                theView.SetProj(myGL.Orientation.ViewOrient);
                myGL.Orientation.ToFitAll = true;
            }

            // apply fit all
            if (myGL.Orientation.ToFitAll)
            {
                const double aFitMargin = 0.01;
                theView.FitAll(aFitMargin, false);
                theView.Invalidate();
                myGL.Orientation.ToFitAll = false;
            }

            //if (myGL.IsNewGesture)
            //{
            //    if (myAnchorPointPrs1.HasInteractiveContext())
            //    {
            //        theCtx.Remove(myAnchorPointPrs1, false);
            //        if (!theView.Viewer().ZLayerSettings(myAnchorPointPrs1.ZLayer()).IsImmediate())
            //        {
            //            theView.Invalidate();
            //        }
            //        else
            //        {
            //            theView.InvalidateImmediate();
            //        }
            //    }
            //    if (myAnchorPointPrs2.HasInteractiveContext())
            //    {
            //        theCtx.Remove(myAnchorPointPrs2, false);
            //        if (!theView.Viewer().ZLayerSettings(myAnchorPointPrs2.ZLayer()).IsImmediate())
            //        {
            //            theView.Invalidate();
            //        }
            //        else
            //        {
            //            theView.InvalidateImmediate();
            //        }
            //    }

            //    if (myHasHlrOnBeforeRotation)
            //    {
            //        myHasHlrOnBeforeRotation = false;
            //        theView.SetComputedMode(true);
            //        theView.Invalidate();
            //    }
            //}

            //if (myNavigationMode != AIS_NavigationMode.AIS_NavigationMode_FirstPersonWalk)
            //{
            //    if (myGL.Panning.ToStart
            //     && myToAllowPanning)
            //    {
            //        gp_Pnt aPanPnt = new gp_Pnt(Precision.Infinite(), 0.0, 0.0);
            //        if (!theView.Camera().IsOrthographic())
            //        {
            //            bool toStickToRay = false;
            //            if (myGL.Panning.PointStart.x() >= 0
            //             && myGL.Panning.PointStart.y() >= 0)
            //            {
            //                PickPoint(aPanPnt, theCtx, theView, myGL.Panning.PointStart, toStickToRay);
            //            }
            //            if (Precision.IsInfinite(aPanPnt.X()))
            //            {
            //                Graphic3d_Vec2i aWinSize;
            //                theView.Window().Size(aWinSize.x(), aWinSize.y());
            //                PickPoint(aPanPnt, theCtx, theView, aWinSize / 2, toStickToRay);
            //            }
            //            if (!Precision.IsInfinite(aPanPnt.X())
            //              && myToShowPanAnchorPoint)
            //            {
            //                gp_Trsf aPntTrsf;
            //                aPntTrsf.SetTranslation(new gp_Vec(aPanPnt.XYZ()));
            //                theCtx.SetLocation(myAnchorPointPrs2, aPntTrsf);
            //            }
            //        }
            //        setPanningAnchorPoint(aPanPnt);
            //    }

            //    if (myToShowPanAnchorPoint
            //    && hasPanningAnchorPoint()
            //    && myGL.Panning.ToPan
            //    && !myGL.IsNewGesture
            //    && !myAnchorPointPrs2.HasInteractiveContext())
            //    {
            //        theCtx.Display(myAnchorPointPrs2, 0, -1, false, PrsMgr_DisplayStatus.AIS_DS_Displayed);
            //    }

            //    handlePanning(theView);
            //    handleZRotate(theView);
            //}

            if ((myNavigationMode == AIS_NavigationMode.AIS_NavigationMode_Orbit
              || myGL.OrbitRotation.ToStart
              || myGL.OrbitRotation.ToRotate)
             && myToAllowRotation)
            {
                if (myGL.OrbitRotation.ToStart
                && !myHasHlrOnBeforeRotation)
                {
                    myHasHlrOnBeforeRotation = theView.ComputedMode();
                    if (myHasHlrOnBeforeRotation)
                    {
                        theView.SetComputedMode(false);
                    }
                }

                gp_Pnt aGravPnt = new gp_Pnt();
                if (myGL.OrbitRotation.ToStart)
                {
                    aGravPnt = GravityPoint(theCtx, theView);
                    if (myToShowRotateCenter)
                    {
                        gp_Trsf aPntTrsf = new gp_Trsf();
                        aPntTrsf.SetTranslation(new gp_Vec(aGravPnt.XYZ()));
                        theCtx.SetLocation(myAnchorPointPrs1, new TopLoc_Location(aPntTrsf));
                        theCtx.SetLocation(myAnchorPointPrs2, new TopLoc_Location(aPntTrsf));
                    }
                }

                if (myToShowRotateCenter
                && myGL.OrbitRotation.ToRotate
                && !myGL.IsNewGesture
                && !myAnchorPointPrs1.HasInteractiveContext())
                {
                    theCtx.Display(myAnchorPointPrs1, 0, -1, false, PrsMgr_DisplayStatus.AIS_DS_Displayed);
                    theCtx.Display(myAnchorPointPrs2, 0, -1, false, PrsMgr_DisplayStatus.AIS_DS_Displayed);
                }
                handleOrbitRotation(theView, aGravPnt,
                                     myToLockOrbitZUp || myNavigationMode != AIS_NavigationMode.AIS_NavigationMode_Orbit);
            }

            //if ((myNavigationMode != AIS_NavigationMode.AIS_NavigationMode_Orbit
            //  || myGL.ViewRotation.ToStart
            //  || myGL.ViewRotation.ToRotate)
            // && myToAllowRotation)
            //{
            //    if (myGL.ViewRotation.ToStart
            //    && !myHasHlrOnBeforeRotation)
            //    {
            //        myHasHlrOnBeforeRotation = theView.ComputedMode();
            //        if (myHasHlrOnBeforeRotation)
            //        {
            //            theView.SetComputedMode(false);
            //        }
            //    }

            //    double aRoll = 0.0;
            //    if (!theWalk[AIS_WalkRotation_Roll].IsEmpty()
            //     && !myToLockOrbitZUp)
            //    {
            //        aRoll = (M_PI / 12.0) * theWalk[AIS_WalkRotation_Roll].Pressure;
            //        aRoll *= Min(1000.0 * theWalk[AIS_WalkRotation_Roll].Duration, 100.0) / 100.0;
            //        if (theWalk[AIS_WalkRotation_Roll].Value < 0.0)
            //        {
            //            aRoll = -aRoll;
            //        }
            //    }

            //    handleViewRotation(theView, theWalk[AIS_WalkRotation_Yaw].Value, theWalk[AIS_WalkRotation_Pitch].Value, aRoll,
            //                        myNavigationMode == AIS_NavigationMode.AIS_NavigationMode_FirstPersonFlight);
            //}

            //if (!myGL.ZoomActions.IsEmpty())
            //{
            //    foreach (var aZoomParams in myGL.ZoomActions)
            //    {
            //        if (myToAllowZFocus
            //         && (aZoomParams.Flags & Aspect_VKeyFlags_CTRL) != 0
            //         && theView.Camera().IsStereo())
            //        {
            //            handleZFocusScroll(theView, aZoomParams);
            //            continue;
            //        }

            //        if (!myToAllowZooming)
            //        {
            //            continue;
            //        }

            //        if (!theView.Camera().IsOrthographic())
            //        {
            //            gp_Pnt aPnt;
            //            if (aZoomParams.HasPoint()
            //             && PickPoint(aPnt, theCtx, theView, aZoomParams.Point, myToStickToRayOnZoom))
            //            {
            //                handleZoom(theView, aZoomParams, &aPnt);
            //                continue;
            //            }

            //            Graphic3d_Vec2i aWinSize;
            //            theView->Window()->Size(aWinSize.x(), aWinSize.y());
            //            if (PickPoint(aPnt, theCtx, theView, aWinSize / 2, myToStickToRayOnZoom))
            //            {
            //                aZoomParams.ResetPoint(); // do not pretend to zoom at 'nothing'
            //                handleZoom(theView, aZoomParams, &aPnt);
            //                continue;
            //            }
            //        }
            //        handleZoom(theView, aZoomParams, null);
            //    }
            //    myGL.ZoomActions.Clear();
            //}
        }

        public void handleOrbitRotation(V3d_View theView,
                                              gp_Pnt thePnt,
                                              bool theToLockZUp)
        {
            if (!myToAllowRotation)
            {
                return;
            }

            Graphic3d_Camera aCam = theView.View().IsActiveXR()
                                                 ? theView.View().BaseXRCamera()
                                                 : theView.Camera();


            /*
             ....
             */
        }

        public void handleZFocusScroll(V3d_View theView,
                                                Aspect_ScrollDelta theParams)
        {
            if (!myToAllowZFocus
             || !theView.Camera().IsStereo())
            {
                return;
            }

            double aFocus = theView.Camera().ZFocus() + (theParams.Delta > 0.0 ? 0.05 : -0.05);
            if (aFocus > 0.2
             && aFocus < 2.0)
            {
                theView.Camera().SetZFocus(theView.Camera().ZFocusType(), aFocus);
                theView.Invalidate();
            }
        }

        AIS_ViewInputBuffer myUI = new AIS_ViewInputBuffer();                       //!< buffer for UI thread
        AIS_ViewInputBuffer myGL = new AIS_ViewInputBuffer();                       //!< buffer for rendering thread


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
