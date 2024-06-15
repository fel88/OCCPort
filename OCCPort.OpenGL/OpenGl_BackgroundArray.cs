using OCCPort;
using OpenTK;
using System;

namespace OCCPort.OpenGL
{
    //! Tool class for generating reusable data for
    //! gradient or texture background rendering.
    public class OpenGl_BackgroundArray : OpenGl_PrimitiveArray
    {
        Graphic3d_TypeOfBackground myType;           //!< Type of background: texture or gradient.
        Aspect_FillMethod myFillMethod;     //!< Texture parameters
        OpenGl_GradientParameters myGradientParams; //!< Gradient parameters
        int myViewWidth;      //!< view width  used for array initialization
        int myViewHeight;     //!< view height used for array initialization
        bool myToUpdate;       //!< Shows if array parameters were changed and data (myAttribs storage) is to be updated

        // =======================================================================
        // method  : Render
        // purpose :
        // =======================================================================
        public void Render(OpenGl_Workspace theWorkspace,
                                     Graphic3d_Camera.Projection theProjection)
        {
            OpenGl_Context aCtx = theWorkspace.GetGlContext();
            int aViewSizeX = aCtx.Viewport()[2];
            int aViewSizeY = aCtx.Viewport()[3];
            NCollection_Vec2i aTileOffset, aTileSize;

            if (aCtx.Camera().Tile().IsValid())
            {
                aViewSizeX = (int)aCtx.Camera().Tile().TotalSize.x();
                aViewSizeY = (int)aCtx.Camera().Tile().TotalSize.y();

                aTileOffset = aCtx.Camera().Tile().OffsetLowerLeft();
                aTileSize = aCtx.Camera().Tile().TileSize;
            }
            if (myToUpdate
             || myViewWidth != aViewSizeX
             || myViewHeight != aViewSizeY
             || myAttribs == null
             || myVboAttribs == null)
            {
                myViewWidth = aViewSizeX;
                myViewHeight = aViewSizeY;
                init(theWorkspace);
            }

            Graphic3d_Mat4 aProjection = aCtx.ProjectionState.Current();
            Graphic3d_Mat4 aWorldView = aCtx.WorldViewState.Current();

            if (myType == Graphic3d_TypeOfBackground.Graphic3d_TOB_CUBEMAP)
            {
                Graphic3d_Camera aCamera = new Graphic3d_Camera(aCtx.Camera());
                aCamera.SetZRange(0.01, 1.0); // is needed to avoid perspective camera exception


                // cancel translation
                aCamera.MoveEyeTo(new gp_Pnt(0.0, 0.0, 0.0));

                // Handle projection matrix:
                // - Cancel any head-to-eye translation for HMD display;
                // - Ignore stereoscopic projection in case of non-HMD 3D display
                //   (ideally, we would need a stereoscopic cubemap image; adding a parallax makes no sense);
                // - Force perspective projection when orthographic camera is active
                //   (orthographic projection makes no sense for cubemap).
                bool isCustomProj = aCamera.IsCustomStereoFrustum()
                                      || aCamera.IsCustomStereoProjection();
                aCamera.SetProjectionType(theProjection == Graphic3d_Camera.Projection.Projection_Orthographic || !isCustomProj
                                         ? Graphic3d_Camera.Projection.Projection_Perspective
                                         : theProjection);

                aProjection = aCamera.ProjectionMatrixF();
                aWorldView = aCamera.OrientationMatrixF();
                if (isCustomProj)
                {
                    // get projection matrix without pre-multiplied stereoscopic head-to-eye translation
                    if (theProjection == Graphic3d_Camera.Projection.Projection_MonoLeftEye)
                    {
                        Graphic3d_Mat4 aMatProjL, aMatHeadToEyeL, aMatProjR, aMatHeadToEyeR;
                     //   aCamera.StereoProjectionF(aMatProjL, aMatHeadToEyeL, aMatProjR, aMatHeadToEyeR);
                        //aProjection = aMatProjL;
                    }
                    else if (theProjection == Graphic3d_Camera.Projection.Projection_MonoRightEye)
                    {
                        Graphic3d_Mat4 aMatProjL, aMatHeadToEyeL, aMatProjR, aMatHeadToEyeR;
                   //     aCamera.StereoProjectionF(aMatProjL, aMatHeadToEyeL, aMatProjR, aMatHeadToEyeR);
                      //  aProjection = aMatProjR;
                    }
                }
            }
            else
            {
                aProjection.InitIdentity();
                aWorldView.InitIdentity();
              // if (aCtx.Camera().Tile().IsValid())
                {
                 //   aWorldView.SetDiagonal(OpenGl_Vec4(2.0f / aTileSize.x(), 2.0f / aTileSize.y(), 1.0f, 1.0f));
                    if (myType == Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT)
                    {
                        //aWorldView.SetColumn(3, OpenGl_Vec4(-1.0f - 2.0f * aTileOffset.x() / aTileSize.x(),
                        //                                      -1.0f - 2.0f * aTileOffset.y() / aTileSize.y(), 0.0f, 1.0f));
                    }
                    else
                    {
                   //     aWorldView.SetColumn(3, OpenGl_Vec4(-1.0f + (float)aViewSizeX / aTileSize.x() - 2.0f * aTileOffset.x() / aTileSize.x(),
                          //                                    -1.0f + (float)aViewSizeY / aTileSize.y() - 2.0f * aTileOffset.y() / aTileSize.y(), 0.0f, 1.0f));
                    }
                }
             //   else
                {
                   // aWorldView.SetDiagonal(OpenGl_Vec4(2.0f / myViewWidth, 2.0f / myViewHeight, 1.0f, 1.0f));
                    if (myType == Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT)
                    {
                      //  aWorldView.SetColumn(3, OpenGl_Vec4(-1.0f, -1.0f, 0.0f, 1.0f));
                    }
                }
            }

            aCtx.ProjectionState.Push();
            aCtx.WorldViewState.Push();
            aCtx.ProjectionState.SetCurrent(aProjection);
            aCtx.WorldViewState.SetCurrent(aWorldView);
            aCtx.ApplyProjectionMatrix();
            aCtx.ApplyModelViewMatrix();

            //OpenGl_PrimitiveArray::Render(theWorkspace);
            Render(theWorkspace);

            aCtx.ProjectionState.Pop();
            aCtx.WorldViewState.Pop();
            aCtx.ApplyProjectionMatrix();
        }

        internal Aspect_GradientFillMethod GradientFillMethod()
        {
            throw new NotImplementedException();
        }

        internal bool IsDefined()
        {
            throw new NotImplementedException();
        }

        internal Aspect_FillMethod TextureFillMethod()
        {
            throw new NotImplementedException();
        }

        private void init(OpenGl_Workspace theWorkspace)
        {
            throw new NotImplementedException();
        }
    }
}