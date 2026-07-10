global using OpenGl_Vec2 = TKernel.NCollection_Vec2<float>;


using OCCPort.Common;
using OpenTK;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using TKernel;
using TKMath;
using TKOpenGl;
using TKService;

namespace OCCPort.OpenGL
{
    //! Tool class for generating reusable data for
    //! gradient or texture background rendering.
    public class OpenGl_BackgroundArray : OpenGl_PrimitiveArray
    {
        Graphic3d_TypeOfBackground myType;           //!< Type of background: texture or gradient.
        Aspect_FillMethod myFillMethod;     //!< Texture parameters
        OpenGl_GradientParameters myGradientParams = new OpenGl_GradientParameters(); //!< Gradient parameters
        int myViewWidth;      //!< view width  used for array initialization
        int myViewHeight;     //!< view height used for array initialization
        bool myToUpdate;       //!< Shows if array parameters were changed and data (myAttribs storage) is to be updated

        public OpenGl_BackgroundArray(OpenGl_GraphicDriver aDriver, Graphic3d_TypeOfPrimitiveArray theType, Graphic3d_IndexBuffer theIndices, Graphic3d_Buffer theAttribs, Graphic3d_BoundBuffer theBounds) : base(aDriver, theType, theIndices, theAttribs, theBounds)
        {
        }

        public OpenGl_BackgroundArray(Graphic3d_TypeOfBackground theType) :
            base(null, Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES, null, null, null)
        {

            myType = (theType);
            myFillMethod = Aspect_FillMethod.Aspect_FM_NONE;
            myViewWidth = (0);
            myViewHeight = (0);
            myToUpdate = (false);

            myDrawMode = (int)All.Triangles;
            myIsFillType = true;

            myGradientParams.color1 = new OpenGl_Vec4(0.0f, 0.0f, 0.0f, 1.0f);
            myGradientParams.color2 = new OpenGl_Vec4(0.0f, 0.0f, 0.0f, 1.0f);
            myGradientParams.type = Aspect_GradientFillMethod.Aspect_GradientFillMethod_None;
        }

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
            Graphic3d_Vec2i aTileOffset = new NCollection_Vec2<int>(), aTileSize = new NCollection_Vec2<int>();

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
                if (aCtx.Camera().Tile().IsValid())
                {
                    aWorldView.SetDiagonal(new OpenGl_Vec4(2.0f / aTileSize.x(), 2.0f / aTileSize.y(), 1.0f, 1.0f));
                    if (myType == Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT)
                    {
                        aWorldView.SetColumn(3, new OpenGl_Vec4(-1.0f - 2.0f * aTileOffset.x() / aTileSize.x(),
                                                              -1.0f - 2.0f * aTileOffset.y() / aTileSize.y(), 0.0f, 1.0f));
                    }
                    else
                    {
                        aWorldView.SetColumn(3, new OpenGl_Vec4(-1.0f + (float)aViewSizeX / aTileSize.x() - 2.0f * aTileOffset.x() / aTileSize.x(),
                                                       -1.0f + (float)aViewSizeY / aTileSize.y() - 2.0f * aTileOffset.y() / aTileSize.y(), 0.0f, 1.0f));
                    }
                }
                else
                {
                    aWorldView.SetDiagonal(new OpenGl_Vec4(2.0f / myViewWidth, 2.0f / myViewHeight, 1.0f, 1.0f));
                    if (myType == Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT)
                    {
                        aWorldView.SetColumn(3, new OpenGl_Vec4(-1.0f, -1.0f, 0.0f, 1.0f));
                    }
                }
            }

            aCtx.ProjectionState.Push();
            aCtx.WorldViewState.Push();
            aCtx.ProjectionState.SetCurrent(aProjection);
            aCtx.WorldViewState.SetCurrent(aWorldView);
            aCtx.ApplyProjectionMatrix();
            aCtx.ApplyModelViewMatrix();

            //OpenGl_PrimitiveArray.Render(theWorkspace);
            Render(theWorkspace);

            aCtx.ProjectionState.Pop();
            aCtx.WorldViewState.Pop();
            aCtx.ApplyProjectionMatrix();
        }


        //! Gets background gradient fill method
        public Aspect_GradientFillMethod GradientFillMethod() { return myGradientParams.type; }


        internal bool IsDefined()
        {
            switch (myType)
            {
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT: return myGradientParams.type != Aspect_GradientFillMethod.Aspect_GradientFillMethod_None;
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE: return myFillMethod != Aspect_FillMethod.Aspect_FM_NONE;
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_CUBEMAP: return true;
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_NONE: return false;
            }
            return false;
        }
        void invalidateData()
        {
            myToUpdate = true;
        }


        //! Sets background gradient parameters
        public void SetGradientParameters(Quantity_Color theColor1,
                                               Quantity_Color theColor2,
                                               Aspect_GradientFillMethod theType)
        {
            if (myType != Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT)
            {
                return;
            }

            double anR = 0, aG = 0, aB = 0;
            theColor1.Values(ref anR, ref aG, ref aB, Quantity_TypeOfColor.Quantity_TOC_RGB);
            myGradientParams.color1 = new OpenGl_Vec4((float)anR, (float)aG, (float)aB, 0.0f);

            theColor2.Values(ref anR, ref aG, ref aB, Quantity_TypeOfColor.Quantity_TOC_RGB);
            myGradientParams.color2 = new OpenGl_Vec4((float)anR, (float)aG, (float)aB, 0.0f);

            myGradientParams.type = theType;
            invalidateData();
        }

        internal Aspect_FillMethod TextureFillMethod()
        {
            throw new NotImplementedException();
        }

        bool createGradientArray(OpenGl_Context theCtx)
        {

            // Initialize data for primitive array
            Graphic3d_Attribute[] aGragientAttribInfo =
            [
                new(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS,   Graphic3d_TypeOfData.Graphic3d_TOD_VEC2 ),
   new (Graphic3d_TypeOfAttribute. Graphic3d_TOA_COLOR,Graphic3d_TypeOfData. Graphic3d_TOD_VEC3 )
  ];

            if (!myAttribs.Init(4, aGragientAttribInfo, 2))
            {
                return false;
            }
            if (!myIndices.Init<ushort>(6))
            {
                return false;
            }
            ushort[] THE_FS_QUAD_TRIS = { 0, 1, 3, 1, 2, 3 };
            for (int aVertIter = 0; aVertIter < 6; ++aVertIter)
            {
                myIndices.SetIndex(aVertIter, THE_FS_QUAD_TRIS[aVertIter]);
            }

            OpenGl_Vec2[] aVertices =
            {
   new  OpenGl_Vec2((float)(myViewWidth), 0.0f),
   new OpenGl_Vec2((float)(myViewWidth), (float)(myViewHeight)),
   new  OpenGl_Vec2(0.0f,               (float)(myViewHeight)),
  new  OpenGl_Vec2(0.0f,               0.0f)
  };

            float[][] aCorners = new float[4][];
            float[] aDiagCorner1 = new float[3];
            float[] aDiagCorner2 = new float[3];

            switch (myGradientParams.type)
            {
                case Aspect_GradientFillMethod.Aspect_GradientFillMethod_Horizontal:
                    {
                        aCorners[0] = myGradientParams.color2.ChangeData();
                        aCorners[1] = myGradientParams.color2.ChangeData();
                        aCorners[2] = myGradientParams.color1.ChangeData();
                        aCorners[3] = myGradientParams.color1.ChangeData();
                        break;
                    }
                case Aspect_GradientFillMethod.Aspect_GradientFillMethod_Vertical:
                    {
                        aCorners[0] = myGradientParams.color2.ChangeData();
                        aCorners[1] = myGradientParams.color1.ChangeData();
                        aCorners[2] = myGradientParams.color1.ChangeData();
                        aCorners[3] = myGradientParams.color2.ChangeData();
                        break;
                    }
                //            case Aspect_GradientFillMethod.Aspect_GradientFillMethod_Diagonal1:
                //{
                //  aCorners[0] = myGradientParams.color2.ChangeData();
                //  aCorners[2] = myGradientParams.color1.ChangeData();
                //  aDiagCorner1[0] = aDiagCorner2[0] = 0.5f * (aCorners[0][0] + aCorners[2][0]);
                //  aDiagCorner1[1] = aDiagCorner2[1] = 0.5f * (aCorners[0][1] + aCorners[2][1]);
                //  aDiagCorner1[2] = aDiagCorner2[2] = 0.5f * (aCorners[0][2] + aCorners[2][2]);
                //  aCorners[1] = aDiagCorner1;
                //  aCorners[3] = aDiagCorner2;
                //  break;
                //}
                case Aspect_GradientFillMethod.Aspect_GradientFillMethod_Diagonal2:
                    {
                        aCorners[1] = myGradientParams.color1.ChangeData();
                        aCorners[3] = myGradientParams.color2.ChangeData();
                        aDiagCorner1[0] = aDiagCorner2[0] = 0.5f * (aCorners[1][0] + aCorners[3][0]);
                        aDiagCorner1[1] = aDiagCorner2[1] = 0.5f * (aCorners[1][1] + aCorners[3][1]);
                        aDiagCorner1[2] = aDiagCorner2[2] = 0.5f * (aCorners[1][2] + aCorners[3][2]);
                        aCorners[0] = aDiagCorner1;
                        aCorners[2] = aDiagCorner2;
                        break;
                    }
                //case Aspect_GradientFillMethod_Corner1:
                //case Aspect_GradientFillMethod_Corner2:
                //case Aspect_GradientFillMethod_Corner3:
                //case Aspect_GradientFillMethod_Corner4:
                //{
                //  Graphic3d_Attribute aCornerAttribInfo[] =
                //  {
                //    { Graphic3d_TOA_POS,   Graphic3d_TOD_VEC2 },
                //    { Graphic3d_TOA_UV,    Graphic3d_TOD_VEC2 }
                //  };

                //  OpenGl_Vec2 []anUVs=new [4] 
                //  {
                //    OpenGl_Vec2 (1.0f, 0.0f),
                //    OpenGl_Vec2 (1.0f, 1.0f),
                //    OpenGl_Vec2 (0.0f, 1.0f),
                //    OpenGl_Vec2 (0.0f, 0.0f)
                //  };

                //  if (!myAttribs->Init (4, aCornerAttribInfo, 2))
                //  {
                //    return Standard_False;
                //  }
                //  for (Standard_Integer anIt = 0; anIt < 4; ++anIt)
                //  {
                //    OpenGl_Vec2* aVertData = reinterpret_cast<OpenGl_Vec2*>(myAttribs->changeValue (anIt));
                //    *aVertData = aVertices[anIt];

                //    OpenGl_Vec2* anUvData = reinterpret_cast<OpenGl_Vec2*>(myAttribs->changeValue (anIt) + myAttribs->AttributeOffset (1));
                //    // cyclically move highlighted corner depending on myGradientParams.type
                //    *anUvData = anUVs[(anIt + myGradientParams.type - Aspect_GradientFillMethod_Corner1) % 4];
                //  }
                //  return true;
                //}
                //case Aspect_GradientFillMethod_Elliptical:
                //{
                //  // construction of a circle circumscribed about a view rectangle
                //  // using parametric equation (scaled by aspect ratio and centered)
                //  const Standard_Integer aSubdiv = 64;
                //  if (!myAttribs->Init (aSubdiv + 2, aGragientAttribInfo, 2))
                //  {
                //    return Standard_False;
                //  }

                //  OpenGl_Vec2 anEllipVerts[aSubdiv + 2];
                //  anEllipVerts[0] = OpenGl_Vec2 (float (myViewWidth) / 2.0f, float (myViewHeight) / 2.0f);
                //  Standard_Real aTetta = (M_PI * 2.0) / aSubdiv;
                //  Standard_Real aParam = 0.0;
                //  for (Standard_Integer anIt = 1; anIt < aSubdiv + 2; ++anIt)
                //  {
                //    anEllipVerts[anIt] = OpenGl_Vec2 (float (Cos (aParam) * Sqrt (2.0) * myViewWidth  / 2.0 + myViewWidth  / 2.0f),
                //                                      float (Sin (aParam) * Sqrt (2.0) * myViewHeight / 2.0 + myViewHeight / 2.0f));

                //    aParam += aTetta;
                //  }
                //  for (Standard_Integer anIt = 0; anIt < aSubdiv + 2; ++anIt)
                //  {
                //    OpenGl_Vec2* aVertData = reinterpret_cast<OpenGl_Vec2*>(myAttribs->changeValue (anIt));
                //    *aVertData = anEllipVerts[anIt];

                //    OpenGl_Vec3* aColorData = reinterpret_cast<OpenGl_Vec3*>(myAttribs->changeValue (anIt) + myAttribs->AttributeOffset (1));
                //    *aColorData = myGradientParams.color2.rgb();
                //  }
                //  // the central vertex is colored in different way
                //  OpenGl_Vec3* aColorData = reinterpret_cast<OpenGl_Vec3*>(myAttribs->changeValue (0) + myAttribs->AttributeOffset (1));
                //  *aColorData = myGradientParams.color1.rgb();

                //  if (!myIndices->Init<unsigned short> (3 * aSubdiv))
                //  {
                //    return Standard_False;
                //  }
                //  for (Standard_Integer aCurTri = 0; aCurTri < aSubdiv; aCurTri++)
                //  {
                //    myIndices->SetIndex (aCurTri * 3 + 0, 0);
                //    myIndices->SetIndex (aCurTri * 3 + 1, aCurTri + 1);
                //    myIndices->SetIndex (aCurTri * 3 + 2, aCurTri + 2);
                //  }

                //  return Standard_True;
                //}
                case Aspect_GradientFillMethod.Aspect_GradientFillMethod_None:
                    {
                        break;
                    }
            }

            for (int anIt = 0; anIt < 4; ++anIt)
            {
                var b1 = BitConverter.GetBytes(aVertices[anIt][0]);
                var b2 = BitConverter.GetBytes(aVertices[anIt][1]);
                myAttribs.changeValue(anIt, b1.Concat(b2).ToArray());

                var color = theCtx.Vec4FromQuantityColor(new OpenGl_Vec4(aCorners[anIt][0], aCorners[anIt][1], aCorners[anIt][2], 1.0f)).rgb();
                List<byte> data = new List<byte>();
                for (int i = 0; i < color.v.Length; i++)
                {
                    data.AddRange(BitConverter.GetBytes(color.v[i]));
                }
                myAttribs.changeValue(anIt, data.ToArray(), myAttribs.AttributeOffset(1));
                //OpenGl_Vec3* aColorData = reinterpret_cast<OpenGl_Vec3*>(myAttribs->changeValue(anIt) + myAttribs.AttributeOffset(1));

            }

            return true;
        }

        private bool init(OpenGl_Workspace theWorkspace)
        {
            OpenGl_Context aCtx = theWorkspace.GetGlContext();

            if (myIndices == null)
            {
                myIndices = new Graphic3d_IndexBuffer(Graphic3d_Buffer.DefaultAllocator());
            }
            if (myAttribs == null)
            {
                myAttribs = new Graphic3d_Buffer(Graphic3d_Buffer.DefaultAllocator());
            }

            switch (myType)
            {
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT:
                    {
                        if (!createGradientArray(aCtx))
                        {
                            return false;
                        }
                        break;
                    }
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE:
                    {
                        /**if (!createTextureArray(theWorkspace))
                        {
                            return false;
                        }*/
                        break;
                    }
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_CUBEMAP:
                    {
                        /* if (!createCubeMapArray())
                         {
                             return false;
                         }*/
                        break;
                    }
                case Graphic3d_TypeOfBackground.Graphic3d_TOB_NONE:
                default:
                    {
                        return false;
                    }
            }

            // Init VBO
            if (myIsVboInit)
            {
                clearMemoryGL(aCtx);
            }
            buildVBO(aCtx, true);
            myIsVboInit = true;

            // Data is up-to-date
            myToUpdate = false;
            return true;
        }
    }
}