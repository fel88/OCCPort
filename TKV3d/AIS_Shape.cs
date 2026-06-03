using OCCPort;
using TKBRep;
using TKG3d;
using TKMath;
using TKService;

namespace TKV3d
{
    //! A framework to manage presentation and selection of shapes.
    //! AIS_Shape is the interactive object which is used the
    //! most by   applications. There are standard functions
    //! available which allow you to prepare selection
    //! operations on the constituent elements of shapes -
    //! vertices, edges, faces etc - in an open local context.
    //! The selection modes specific to "Shape" type objects
    //! are referred to as Standard Activation Mode. These
    //! modes are only taken into account in open local
    //! context and only act on Interactive Objects which
    //! have redefined the virtual method
    //! AcceptShapeDecomposition so that it returns true.
    //! Several advanced functions are also available. These
    //! include functions to manage deviation angle and
    //! deviation coefficient - both HLR and non-HLR - of
    //! an inheriting shape class. These services allow you to
    //! select one type of shape interactive object for higher
    //! precision drawing. When you do this, the
    //! Prs3d_Drawer::IsOwn... functions corresponding to the
    //! above deviation angle and coefficient functions return
    //! true indicating that there is a local setting available
    //! for the specific object.
    //!
    //! This class allows to map textures on shapes using native UV parametric space of underlying surface of each Face
    //! (this means that texture will be visually duplicated on all Faces).
    //! To generate texture coordinates, appropriate shading attribute should be set before computing presentation in AIS_Shaded display mode:
    //! @code
    //!   Handle(AIS_Shape) aPrs = new AIS_Shape();
    //!   aPrs->Attributes()->SetupOwnShadingAspect();
    //!   aPrs->Attributes()->ShadingAspect()->Aspect()->SetTextureMapOn();
    //!   aPrs->Attributes()->ShadingAspect()->Aspect()->SetTextureMap (new Graphic3d_Texture2Dmanual (Graphic3d_NOT_2D_ALUMINUM));
    //! @endcode
    //! The texture itself is parametrized in (0,1)x(0,1).
    public class AIS_Shape : AIS_InteractiveObject
    {

        public override void Compute(PrsMgr_PresentationManager thePrsMgr,
                                   Prs3d_Presentation thePrs,
                                   int theMode)
        {

            if (myshape.IsNull()
             || (myshape.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND && myshape.NbChildren() == 0))
            {
                return;
            }


            // wire,edge,vertex -> pas de HLR + priorite display superieure
            if (myshape.ShapeType() >= TopAbs_ShapeEnum.TopAbs_WIRE
             && myshape.ShapeType() <= TopAbs_ShapeEnum.TopAbs_VERTEX)
            {
                // TopAbs_WIRE -> 7, TopAbs_EDGE -> 8, TopAbs_VERTEX -> 9 (Graphic3d_DisplayPriority_Highlight)
                /*int aPrior = (int)Graphic3d_DisplayPriority_Above1
                                             + (int)myshape.ShapeType() - TopAbs_ShapeEnum. TopAbs_WIRE;
               thePrs.SetVisual(Graphic3d_TOS_ALL);
               thePrs.SetDisplayPriority((Graphic3d_DisplayPriority)aPrior);*/
            }

            if (IsInfinite())
            {
                thePrs.SetInfiniteState(true); //not taken in account during FITALL
            }

            switch (theMode)
            {

                case (int)AIS_DisplayMode.AIS_WireFrame:
                    {
                        StdPrs_ToolTriangulatedShape.ClearOnOwnDeflectionChange(myshape, myDrawer, true);
                        try
                        {
                            //OCC_CATCH_SIGNALS
                            StdPrs_WFShape.Add(thePrs, myshape, myDrawer);
                        }
                        catch (Standard_Failure anException)
                        {
                            Message.SendFail(("Error: AIS_Shape::Compute() wireframe presentation builder has failed (")
                                             + anException.Message + ")");
                        }
                        break;
                    }
                case (int)AIS_DisplayMode.AIS_Shaded:
                    {
                        StdPrs_ToolTriangulatedShape.ClearOnOwnDeflectionChange(myshape, myDrawer, true);
                        if ((int)myshape.ShapeType() > 4)
                        {
                            StdPrs_WFShape.Add(thePrs, myshape, myDrawer);
                        }
                        else
                        {
                            if (IsInfinite())
                            {
                                StdPrs_WFShape.Add(thePrs, myshape, myDrawer);
                            }
                            else
                            {
                                /* try
                                 {*/
                                //OCC_CATCH_SIGNALS
                                StdPrs_ShadedShape.Add(thePrs, myshape, myDrawer,
                                                         myDrawer.ShadingAspect().Aspect().ToMapTexture()
                                                     && myDrawer.ShadingAspect().Aspect().TextureMap() != null,
                                                         myUVOrigin, myUVRepeat, myUVScale);

                                /*
                                }
                                catch (Standard_Failure const&anException)
          {
                                    Message::SendFail(TCollection_AsciiString("Error: AIS_Shape::Compute() shaded presentation builder has failed (")
                                                     + anException.GetMessageString() + ")");
                                    StdPrs_WFShape::Add(thePrs, myshape, myDrawer);
                                }*/
                            }
                        }
                        break;
                    }

            }
        }



        public AIS_Shape()
        {

        }
        public AIS_Shape(TopoDS_Shape theShape)
            : base(PrsMgr_TypeOfPresentation3d.PrsMgr_TOP_ProjectorDependent)
        {
            myshape = theShape;
            myUVOrigin = new gp_Pnt2d(0.0, 0.0);
            myUVRepeat = new gp_Pnt2d(1.0, 1.0);
            myUVScale = new gp_Pnt2d(1.0, 1.0);
            myInitAng = 0.0;
            myCompBB = true;
        }

        //! Returns this shape object.
        public TopoDS_Shape Shape() { return myshape; }

        //! Return shape type for specified selection mode.
        public static TopAbs_ShapeEnum SelectionType(int theSelMode)
        {
            switch (theSelMode)
            {
                case 1: return TopAbs_ShapeEnum.TopAbs_VERTEX;
                case 2: return TopAbs_ShapeEnum.TopAbs_EDGE;
                case 3: return TopAbs_ShapeEnum.TopAbs_WIRE;
                case 4: return TopAbs_ShapeEnum.TopAbs_FACE;
                case 5: return TopAbs_ShapeEnum.TopAbs_SHELL;
                case 6: return TopAbs_ShapeEnum.TopAbs_SOLID;
                case 7: return TopAbs_ShapeEnum.TopAbs_COMPSOLID;
                case 8: return TopAbs_ShapeEnum.TopAbs_COMPOUND;
                case 0: return TopAbs_ShapeEnum.TopAbs_SHAPE;
            }
            return TopAbs_ShapeEnum.TopAbs_SHAPE;
        }

        public override void ComputeSelection(SelectMgr_Selection theSelection, int aMode)
        {
            if (myshape.IsNull()) return;
            if (myshape.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND && myshape.NbChildren() == 0)
            {
                // empty Shape -> empty Assembly.
                return;
            }

            TopAbs_ShapeEnum TypOfSel = SelectionType(aMode);
            TopoDS_Shape shape = myshape;

            // POP protection against crash in low layers

            double aDeflection = StdPrs_ToolTriangulatedShape.GetDeflection(shape, myDrawer);
            //try
            //{
            //    //OCC_CATCH_SIGNALS
            //    StdSelect_BRepSelectionTool.Load(aSelection,
            //                                      this,
            //                                      shape,
            //                                      TypOfSel,
            //                                      aDeflection,
            //                                      myDrawer->DeviationAngle(),
            //                                      myDrawer->IsAutoTriangulation());
            //}
            //catch (Standard_Failure anException)
            //{
            //    Message.SendFail(("Error: AIS_Shape::ComputeSelection(") + aMode + ") has failed ("
            //                     + anException.Message + ")");
            //    if (aMode == 0)
            //    {
            //        aSelection.Clear();
            //        Bnd_Box B = BoundingBox();
            //        Handle(StdSelect_BRepOwner) aOwner = new StdSelect_BRepOwner(shape, this);
            //        Handle(Select3D_SensitiveBox) aSensitiveBox = new Select3D_SensitiveBox(aOwner, B);
            //        aSelection->Add(aSensitiveBox);
            //    }
            //}

            //// insert the drawer in the BrepOwners for hilight...
            //StdSelect.SetDrawerForBRepOwner(aSelection, myDrawer);
        }

        protected TopoDS_Shape myshape;    //!< shape to display

        protected Bnd_Box myBB;       //!< cached bounding box of the shape
        protected gp_Pnt2d myUVOrigin; //!< UV origin vector for generating texture coordinates
        protected gp_Pnt2d myUVRepeat; //!< UV repeat vector for generating texture coordinates
        protected gp_Pnt2d myUVScale;  //!< UV scale  vector for generating texture coordinates
        double myInitAng;
        bool myCompBB;   //!< if TRUE, then bounding box should be recomputed



    }

    //! Defines a class of objects with display and selection services.
    //! Entities which are visualized and selected are Interactive Objects.
    //! Specific attributes of entities such as arrow aspect for dimensions must be loaded in a Prs3d_Drawer.
    //!
    //! You can make use of classes of standard Interactive Objects for which all necessary methods have already been programmed,
    //! or you can implement your own classes of Interactive Objects.
    //! Key interface methods to be implemented by every Interactive Object:
    //! * Presentable Object (PrsMgr_PresentableObject)
    //!   Consider defining an enumeration of supported Display Mode indexes for particular Interactive Object or class of Interactive Objects.
    //!   - AcceptDisplayMode() accepting display modes implemented by this object;
    //!   - Compute() computing presentation for the given display mode index;
    //! * Selectable Object (SelectMgr_SelectableObject)
    //!   Consider defining an enumeration of supported Selection Mode indexes for particular Interactive Object or class of Interactive Objects.
    //!   - ComputeSelection() computing selectable entities for the given selection mode index.
    public abstract class AIS_InteractiveObject : SelectMgr_SelectableObject
    {
        public AIS_InteractiveObject(PrsMgr_TypeOfPresentation3d aTypeOfPresentation3d = PrsMgr_TypeOfPresentation3d.PrsMgr_TOP_AllView)
            : base(aTypeOfPresentation3d)
        {

            myCTXPtr = null;
        }
        AIS_InteractiveContext myCTXPtr; //!< pointer to Interactive Context, where object is currently displayed; @sa SetContext()
        object myOwner;  //!< application-specific owner object

        internal void SetDisplayStatus(PrsMgr_DisplayStatus theStatus)
        {
            myDisplayStatus = theStatus;
        }

        //! Indicates whether the Interactive Object has a pointer to an interactive context.
        public bool HasInteractiveContext() { return myCTXPtr != null; }


        internal void SetContext(AIS_InteractiveContext theCtx)
        {
            if (myCTXPtr == theCtx)
            {
                return;
            }

            myCTXPtr = theCtx;
            if (theCtx != null)
            {
                myDrawer.Link(theCtx.DefaultDrawer());
            }

        }


    }

    public abstract class SelectMgr_SelectableObject : PrsMgr_PresentableObject
    {
        public SelectMgr_SelectableObject(PrsMgr_TypeOfPresentation3d aTypeOfPresentation3d)
            : base(aTypeOfPresentation3d)
        {

            myGlobalSelMode = (0);
            myAutoHilight = (true);
        }

        public void ErasePresentations(bool theToRemove)
        {
            if (mySelectionPrs != null)
            {
                mySelectionPrs.Erase();
                if (theToRemove)
                {
                    mySelectionPrs.Clear();
                    mySelectionPrs = null;
                }
            }
            if (myHilightPrs != null)
            {
                myHilightPrs.Erase();
                if (theToRemove)
                {
                    myHilightPrs.Clear();
                    myHilightPrs = null;
                }
            }
        }

        //! If returns True, the old mechanism for highlighting selected objects is used (HilightSelected Method may be empty).
        //! If returns False, the HilightSelected method will be fully responsible for highlighting selected entity owners belonging to this selectable object.
        public virtual bool IsAutoHilight() { return myAutoHilight; }

        //! Method which clear all selected owners belonging
        //! to this selectable object ( for fast presentation draw )
        public virtual void ClearSelected()
        {
            if (mySelectionPrs != null)
            {
                mySelectionPrs.Clear();
            }
        }
        //! Computes sensitive primitives for the given selection mode - key interface method of Selectable Object.
        //! @param theSelection selection to fill
        //! @param theMode selection mode to create sensitive primitives
        public abstract void ComputeSelection(SelectMgr_Selection theSelection,
                                 int theMode);
        //==================================================
        // Function: RecomputePrimitives
        // Purpose : IMPORTANT: Do not use this method to update
        //           selection primitives except implementing custom
        //           selection manager! This method does not take
        //           into account necessary BVH updates, but may
        //           invalidate the pointers it refers to.
        //           TO UPDATE SELECTION properly from outside classes,
        //           use method UpdateSelection.
        //==================================================
        public void RecomputePrimitives(int theMode)
        {
            SelectMgr_SelectableObject aSelParent = (SelectMgr_SelectableObject)(Parent());
            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                if (aSel.Mode() == theMode)
                {
                    aSel.Clear();
                    ComputeSelection(aSel, theMode);
                    aSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
                    aSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_Renew);
                    if (theMode == 0 && aSelParent != null)
                    {
                        SelectMgr_EntityOwner anAsmOwner = aSelParent.GetAssemblyOwner();
                        if (anAsmOwner != null)
                        {
                            SetAssemblyOwner(anAsmOwner, theMode);
                        }
                    }
                    return;
                }
            }

            SelectMgr_Selection aNewSel = new SelectMgr_Selection(theMode);
            ComputeSelection(aNewSel, theMode);

            if (theMode == 0 && aSelParent != null)
            {
                SelectMgr_EntityOwner anAsmOwner = aSelParent.GetAssemblyOwner();
                if (anAsmOwner != null)
                {
                    SetAssemblyOwner(anAsmOwner, theMode);
                }
            }

            aNewSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
            aNewSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_Add);

            myselections.Append(aNewSel);
        }

        public virtual SelectMgr_EntityOwner GetAssemblyOwner()
        {
            throw null;
        }

        //=======================================================================
        //function : SetAssemblyOwner
        //purpose  : Sets common entity owner for assembly sensitive object entities
        //=======================================================================
        public void SetAssemblyOwner(SelectMgr_EntityOwner theOwner,
                                                   int theMode)
        {
            if (theMode == -1)
            {
                foreach (var item in myselections)
                {
                    SelectMgr_Selection aSel = item;
                    foreach (var ent in aSel.Entities())
                    {

                        ent.BaseSensitive().Set(theOwner);
                    }
                }
                return;
            }

            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                if (aSel.Mode() == theMode)
                {
                    foreach (var ent in aSel.Entities())
                    {
                        ent.BaseSensitive().Set(theOwner);
                    }
                    return;
                }
            }
        }
        //! Return the sequence of selections.
        public SelectMgr_SequenceOfSelection Selections() { return myselections; }

        public override void ResetTransformation()
        {
            foreach (var item in myselections)
            {
                SelectMgr_Selection aSel = item;
                aSel.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial);
                aSel.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_None);
            }

            base.ResetTransformation();
        }

        SelectMgr_SequenceOfSelection myselections;    //!< list of selections
        Prs3d_Presentation mySelectionPrs;  //!< optional presentation for highlighting selected object
        Prs3d_Presentation myHilightPrs;    //!< optional presentation for highlighting detected object
        int myGlobalSelMode; //!< global selection mode
        bool myAutoHilight;   //!< auto-highlighting flag defining

        //! Returns the mode for selection of object as a whole; 0 by default.
        public int GlobalSelectionMode() { return myGlobalSelMode; }


    }

    //! Tool for computing wireframe presentation of a TopoDS_Shape.
    class StdPrs_WFShape : Prs3d_Root
    {


        public static void Add(Prs3d_Presentation thePresentation,
                              TopoDS_Shape theShape,
                              Prs3d_Drawer theDrawer,
                              bool theIsParallel = false)
        {
            if (theShape.IsNull())
            {
                return;
            }

            if (theDrawer.IsAutoTriangulation())
            {
                StdPrs_ToolTriangulatedShape.Tessellate(theShape, theDrawer);
            }

            // draw triangulation-only edges
            Graphic3d_ArrayOfPrimitives aTriFreeEdges = AddEdgesOnTriangulation(theShape, true);
            if (aTriFreeEdges != null)
            {
                Graphic3d_Group aGroup = thePresentation.NewGroup();
                //aGroup.SetPrimitivesAspect(theDrawer.FreeBoundaryAspect().Aspect());
                aGroup.AddPrimitiveArray(aTriFreeEdges);
            }

            Prs3d_NListOfSequenceOfPnt aCommonPolylines = new Prs3d_NListOfSequenceOfPnt();
            Prs3d_LineAspect aWireAspect = theDrawer.WireAspect();
            double aShapeDeflection = StdPrs_ToolTriangulatedShape.GetDeflection(theShape, theDrawer);


            // Draw isolines
            {
                Prs3d_NListOfSequenceOfPnt aUPolylines = new Prs3d_NListOfSequenceOfPnt(), aVPolylines = new Prs3d_NListOfSequenceOfPnt();
                Prs3d_NListOfSequenceOfPnt aUPolylinesPtr = aUPolylines;
                Prs3d_NListOfSequenceOfPnt aVPolylinesPtr = aVPolylines;

                Prs3d_LineAspect anIsoAspectU = theDrawer.UIsoAspect();
                Prs3d_LineAspect anIsoAspectV = theDrawer.VIsoAspect();

                if (anIsoAspectV.Aspect().IsEqual(anIsoAspectU.Aspect()))
                {
                    aVPolylinesPtr = aUPolylinesPtr;  // put both U and V isolines into single group
                }
                if (anIsoAspectU.Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aUPolylinesPtr = aCommonPolylines; // put U isolines into single group with common edges
                }
                if (anIsoAspectV.Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aVPolylinesPtr = aCommonPolylines; // put V isolines into single group with common edges
                }

                bool isParallelIso = false;
                if (theIsParallel)
                {
                    int aNbFaces = 0;
                    for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                    {
                        ++aNbFaces;
                    }
                    if (aNbFaces > 1)
                    {
                        isParallelIso = true;
                        List<TopoDS_Face> aFaces = new List<TopoDS_Face>(aNbFaces);
                        aNbFaces = 0;
                        for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                        {
                            TopoDS_Face aFace = TopoDS.Face(aFaceExplorer.Current());
                            if (theDrawer.IsoOnPlane() || !StdPrs_ShapeTool.IsPlanarFace(aFace))
                            {
                                aFaces[aNbFaces++] = aFace;
                            }
                        }

                        //StdPrs_WFShape_IsoFunctor anIsoFunctor(*aUPolylinesPtr, *aVPolylinesPtr, aFaces, theDrawer, aShapeDeflection);
                        //  OSD_Parallel::For(0, aNbFaces, anIsoFunctor, aNbFaces < 2);
                    }
                }

                if (!isParallelIso)
                {
                    for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                    {
                        TopoDS_Face aFace = TopoDS.Face(aFaceExplorer.Current());
                        if (theDrawer.IsoOnPlane() || !StdPrs_ShapeTool.IsPlanarFace(aFace))
                        {
                            StdPrs_Isolines.Add(aFace, theDrawer, aShapeDeflection, aUPolylinesPtr, aVPolylinesPtr);
                        }
                    }
                }

                Prs3d.AddPrimitivesGroup(thePresentation, anIsoAspectU, aUPolylines);
                Prs3d.AddPrimitivesGroup(thePresentation, anIsoAspectV, aVPolylines);
            }

            {
                Prs3d_NListOfSequenceOfPnt anUnfree = new Prs3d_NListOfSequenceOfPnt(), aFree = new Prs3d_NListOfSequenceOfPnt();
                Prs3d_NListOfSequenceOfPnt anUnfreePtr = anUnfree;
                Prs3d_NListOfSequenceOfPnt aFreePtr = aFree;
                /*
                if (!theDrawer.UnFreeBoundaryDraw())
                {
                    anUnfreePtr = null;
                }
                else if (theDrawer.UnFreeBoundaryAspect()->Aspect()->IsEqual(aWireAspect.Aspect()))
                {
                    anUnfreePtr = &aCommonPolylines; // put unfree edges into single group with common edges
                }

                if (!theDrawer.FreeBoundaryDraw())
                {
                    aFreePtr = null;
                }
                else if (theDrawer.FreeBoundaryAspect().Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aFreePtr = &aCommonPolylines; // put free edges into single group with common edges
                }*/

                addEdges(theShape,
                          theDrawer,
                          aShapeDeflection,
                          theDrawer.WireDraw() ? aCommonPolylines : null,
                          aFreePtr,
                          anUnfreePtr);

                Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.UnFreeBoundaryAspect(), anUnfree);
                Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.FreeBoundaryAspect(), aFree);
            }


            Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.WireAspect(), aCommonPolylines);

            Graphic3d_ArrayOfPoints aVertexArray = AddVertexes(theShape, theDrawer.VertexDrawMode());
            if (aVertexArray != null)
            {
                Graphic3d_Group aGroup = thePresentation.NewGroup();
                //aGroup.SetPrimitivesAspect(theDrawer.PointAspect()->Aspect());
                aGroup.AddPrimitiveArray(aVertexArray);
            }
        }

        // =========================================================================
        // function : addEdges
        // purpose  :
        // =========================================================================
        public static void addEdges(TopoDS_Shape theShape,
                                Prs3d_Drawer theDrawer,
                                double theShapeDeflection,
                                Prs3d_NListOfSequenceOfPnt theWire,
                                Prs3d_NListOfSequenceOfPnt theFree,
                                Prs3d_NListOfSequenceOfPnt theUnFree)
        {
            if (theShape.IsNull())
            {
                return;
            }

            TopTools_ListOfShape aLWire = new TopTools_ListOfShape();
            TopTools_ListOfShape aLFree = new TopTools_ListOfShape();
            TopTools_ListOfShape aLUnFree = new TopTools_ListOfShape();

            TopTools_IndexedDataMapOfShapeListOfShape anEdgeMap = new TopTools_IndexedDataMapOfShapeListOfShape();
            TopExp.MapShapesAndAncestors(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, anEdgeMap);

            for (TopTools_IndexedDataMapOfShapeListOfShape.Iterator anEdgeIter = new TopTools_IndexedDataMapOfShapeListOfShape.Iterator(anEdgeMap); anEdgeIter.More(); anEdgeIter.Next())
            {
                TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.Key());
                int aNbNeighbours = anEdgeIter.Value().Extent();
                switch (aNbNeighbours)
                {
                    case 0:
                        {
                            if (theWire != null)
                            {
                                aLWire.Append(anEdge);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (theFree != null)
                            {
                                aLFree.Append(anEdge);
                            }
                            break;
                        }
                    default:
                        {
                            if (theUnFree != null)
                            {
                                aLUnFree.Append(anEdge);
                            }
                            break;
                        }
                }
            }

            if (!aLWire.IsEmpty())
            {
                addEdges(aLWire, theDrawer, theShapeDeflection, theWire);
            }
            if (!aLFree.IsEmpty())
            {
                addEdges(aLFree, theDrawer, theShapeDeflection, theFree);
            }
            if (!aLUnFree.IsEmpty())
            {
                addEdges(aLUnFree, theDrawer, theShapeDeflection, theUnFree);
            }
        }

        static void addEdges(TopTools_ListOfShape theEdges,
                                   Prs3d_Drawer theDrawer,
                                   double theShapeDeflection,
                                  Prs3d_NListOfSequenceOfPnt thePolylines)
        {
            TopTools_ListIteratorOfListOfShape anEdgesIter = new TopTools_ListIteratorOfListOfShape();
            for (anEdgesIter.Initialize(theEdges); anEdgesIter.More(); anEdgesIter.Next())
            {
                TopoDS_Edge anEdge = TopoDS.Edge(anEdgesIter.Value());
                if (BRep_Tool.Degenerated(anEdge))
                {
                    continue;
                }

                TColgp_SequenceOfPnt aPoints = new TColgp_SequenceOfPnt();

                TopLoc_Location aLocation = new TopLoc_Location();
                Poly_Triangulation aTriangulation = null;
                Poly_PolygonOnTriangulation anEdgeIndicies = null;
                BRep_Tool.PolygonOnTriangulation(anEdge, ref anEdgeIndicies, ref aTriangulation, aLocation);
                Poly_Polygon3D aPolygon;

                if (anEdgeIndicies != null)
                {
                    // Presentation based on triangulation of a face.
                    TColStd_Array1OfInteger anIndices = anEdgeIndicies.Nodes();

                    int anIndex = anIndices.Lower();
                    if (aLocation.IsIdentity())
                    {
                        for (; anIndex <= anIndices.Upper(); ++anIndex)
                        {
                            aPoints.Append(aTriangulation.Node(anIndices[anIndex]));
                        }
                    }
                    else
                    {
                        for (; anIndex <= anIndices.Upper(); ++anIndex)
                        {
                            aPoints.Append(aTriangulation.Node(anIndices[anIndex]).Transformed(aLocation));
                        }
                    }
                }
                else if ((aPolygon = BRep_Tool.Polygon3D(anEdge, ref aLocation)) != null)
                {
                    // Presentation based on triangulation of the free edge on a surface.
                    TColgp_Array1OfPnt aNodes = aPolygon.Nodes();
                    int anIndex = aNodes.Lower();
                    if (aLocation.IsIdentity())
                    {
                        for (; anIndex <= aNodes.Upper(); ++anIndex)
                        {
                            aPoints.Append(aNodes.Value(anIndex));
                        }
                    }
                    else
                    {
                        for (; anIndex <= aNodes.Upper(); ++anIndex)
                        {
                            aPoints.Append(aNodes.Value(anIndex).Transformed(aLocation));
                        }
                    }
                }
                else if (BRep_Tool.IsGeometric(anEdge))
                {
                    // Default presentation for edges without triangulation.
                    BRepAdaptor_Curve aCurve = new BRepAdaptor_Curve(anEdge);
                    StdPrs_DeflectionCurve.Add(null,
                                                 aCurve,
                                                 theShapeDeflection,
                                                 theDrawer,
                                                 aPoints.ChangeSequence(),
                                                 false);
                }

                if (!aPoints.IsEmpty())
                {
                    thePolylines.Append(aPoints);
                }
            }
        }


        // =========================================================================
        // function : AddVertexes
        // purpose  :
        // =========================================================================
        public static Graphic3d_ArrayOfPoints AddVertexes(TopoDS_Shape theShape,
                                                             Prs3d_VertexDrawMode theVertexMode)
        {
            TColgp_SequenceOfPnt aShapeVertices = new TColgp_SequenceOfPnt();
            if (theVertexMode == Prs3d_VertexDrawMode.Prs3d_VDM_All)
            {
                for (TopExp_Explorer aVertIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX); aVertIter.More(); aVertIter.Next())
                {
                    TopoDS_Vertex aVert = TopoDS.Vertex(aVertIter.Current());
                    aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                }
            }
            else
            {
                // isolated vertices
                for (TopExp_Explorer aVertIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX, TopAbs_ShapeEnum.TopAbs_EDGE); aVertIter.More(); aVertIter.Next())
                {
                    TopoDS_Vertex aVert = TopoDS.Vertex(aVertIter.Current());
                    aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                }

                // internal vertices
                for (TopExp_Explorer anEdgeIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_EDGE); anEdgeIter.More(); anEdgeIter.Next())
                {
                    for (TopoDS_Iterator aVertIter = new TopoDS_Iterator(anEdgeIter.Current(), false, true); aVertIter.More(); aVertIter.Next())
                    {
                        TopoDS_Shape aVertSh = aVertIter.Value();
                        if (aVertSh.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL
                            && aVertSh.ShapeType() == TopAbs_ShapeEnum.TopAbs_VERTEX)
                        {
                            TopoDS_Vertex aVert = TopoDS.Vertex(aVertSh);
                            aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                        }
                    }
                }
            }

            if (aShapeVertices.IsEmpty())
            {
                return null;
            }

            int aNbVertices = aShapeVertices.Length();
            Graphic3d_ArrayOfPoints aVertexArray = new Graphic3d_ArrayOfPoints(aNbVertices);
            for (int aVertIter = 1; aVertIter <= aNbVertices; ++aVertIter)
            {
                aVertexArray.AddVertex(aShapeVertices.Value(aVertIter));
            }
            return aVertexArray;
        }

        public static void AddEdgesOnTriangulation(TColgp_SequenceOfPnt theSegments,

                                              TopoDS_Shape theShape,
                                              bool theToExcludeGeometric = true)
        {
            TopLoc_Location aLocation = new TopLoc_Location(), aDummyLoc = new TopLoc_Location();
            for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
                if (theToExcludeGeometric)
                {
                    Geom_Surface aSurf = BRep_Tool.Surface(aFace, out aDummyLoc);
                    if (aSurf != null)
                    {
                        continue;
                    }
                }
                Poly_Triangulation aPolyTri = BRep_Tool.Triangulation(aFace, ref aLocation);
                if (aPolyTri != null)
                {
                    Prs3d.AddFreeEdges(theSegments, aPolyTri, aLocation);
                }
            }
        }

        public static Graphic3d_ArrayOfPrimitives AddEdgesOnTriangulation(TopoDS_Shape theShape,
                                                                             bool theToExcludeGeometric)
        {
            TColgp_SequenceOfPnt aSeqPnts = new TColgp_SequenceOfPnt();
            AddEdgesOnTriangulation(aSeqPnts, theShape, theToExcludeGeometric);
            if (aSeqPnts.Size() < 2)
            {
                return null;
            }

            int aNbVertices = aSeqPnts.Size();
            Graphic3d_ArrayOfSegments aSurfArray = new Graphic3d_ArrayOfSegments(aNbVertices);
            for (int anI = 1; anI <= aNbVertices; anI += 2)
            {
                aSurfArray.AddVertex(aSeqPnts.Value(anI));
                aSurfArray.AddVertex(aSeqPnts.Value(anI + 1));
            }
            return aSurfArray;
        }

    }
    public class Prs3d_Root
    {
    }
}

