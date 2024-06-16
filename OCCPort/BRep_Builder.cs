using System;
using System.Net.Http.Headers;

namespace OCCPort
{


    //! A framework providing advanced tolerance control.
    //! It is used to build Shapes.
    //! If tolerance control is required, you are advised to:
    //! 1. build a default precision for topology, using the
    //! classes provided in the BRepAPI package
    //! 2. update the tolerance of the resulting shape.
    //! Note that only vertices, edges and faces have
    //! meaningful tolerance control. The tolerance value
    //! must always comply with the condition that face
    //! tolerances are more restrictive than edge tolerances
    //! which are more restrictive than vertex tolerances. In
    //! other words: Tol(Vertex) >= Tol(Edge) >= Tol(Face).
    //! Other rules in setting tolerance include:
    //! - you can open up tolerance but should never restrict it
    //! - an edge cannot be included within the fusion of the
    //! tolerance spheres of two vertices
    public class BRep_Builder : TopoDS_Builder

    {


        //! Add the Shape C in the Shape S.
        //! Exceptions
        //! - TopoDS_FrozenShape if S is not free and cannot be modified.
        //! - TopoDS__UnCompatibleShapes if S and C are not compatible.
        public void Add(TopoDS_Shape aShape, TopoDS_Shape aComponent)
        {

            //=======================================================================
            //function : Add
            //purpose  : insert aComponent in aShape
            //=======================================================================



            // From now the Component cannot be edited
            aComponent.TShape().Free(false);

            // Note that freezing aComponent before testing if aShape is free
            // prevents from self-insertion
            // but aShape will be frozen when the Exception is raised
            if (aShape.Free())
            {
                uint[] aTb =                 {
      //COMPOUND to:
      (1<<((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)),
      //COMPSOLID to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)),
      //SOLID to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPSOLID)),
      //SHELL to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)),
      //FACE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SHELL)),
      //WIRE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_FACE)),
      //EDGE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_WIRE)),
      //VERTEX to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_FACE)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_EDGE)),
      //SHAPE to:
      0
                  };
                //
                uint iC = (uint)aComponent.ShapeType();
                int iS = (int)aShape.ShapeType();
                //
                if ((aTb[iC] & (1 << iS)) != 0)
                {
                    TopoDS_ListOfShape L = aShape.TShape().myShapes;
                    L.Append(aComponent);
                    TopoDS_Shape S = L.Last();
                    //
                    // compute the relative Orientation
                    if (aShape.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                        S.Reverse();
                    //
                    // and the Relative Location
                    TopLoc_Location aLoc = aShape.Location();
                    if (!aLoc.IsIdentity())
                        S.Move(aLoc.Inverted(), false);
                    //
                    // Set the TShape as modified.
                    aShape.TShape().Modified(true);
                }
                else
                {
                    throw new TopoDS_UnCompatibleShapes("TopoDS_Builder::Add");
                }
            }
            else
            {
                throw new TopoDS_FrozenShape("TopoDS_Builder::Add");
            }
        }

        internal void Add(TopoDS_Edge e, TopoDS_Vertex vV)
        {
            throw new NotImplementedException();
        }

        internal void Add(TopoDS_Wire w, TopoDS_Edge eE)
        {
            throw new NotImplementedException();
        }

        internal void Add(TopoDS_Face f, TopoDS_Wire w)
        {
            throw new NotImplementedException();
        }

        internal void MakeEdge(TopoDS_Edge E, Geom_Curve C, double Tol)
        {
            throw new NotImplementedException();
        }
        public void MakeEdge(TopoDS_Edge E)
        {
            BRep_TEdge TE = new BRep_TEdge();
            if (E != null && E.Locked())
            {
                throw new TopoDS_LockedShape("BRep_Builder::MakeEdge");
            }
            MakeShape(E, TE);
        }

        private void MakeShape(TopoDS_Edge e, BRep_TEdge tE)
        {
            throw new NotImplementedException();
        }

        internal void MakeFace(TopoDS_Face F, Geom_Surface S,
                    double Tol)
        {
            BRep_TFace TF = new BRep_TFace();
            if (F != null && F.Locked())
            {
                throw new TopoDS_LockedShape("BRep_Builder::MakeFace");
            }
            TF.Surface(S);
            TF.Tolerance(Tol);
            MakeShape(F, TF);
        }

        private void MakeShape(TopoDS_Face f, BRep_TFace tF)
        {
            throw new NotImplementedException();
        }

        public void MakeFace(TopoDS_Face F)
        {
            BRep_TFace TF = new BRep_TFace();
            MakeShape(F, TF);
        }


        internal void UpdateEdge(TopoDS_Edge e, Geom2d_Line geom2d_Line, TopoDS_Face f, double v)
        {
            throw new NotImplementedException();
        }

        internal void UpdateVertex(TopoDS_Vertex V,
            double Par, TopoDS_Edge E, double F, double Tol)
        {
            TopLoc_Location l = new TopLoc_Location();
            //UpdateVertex(V, Par, E, BRep_Tool.Surface(F, l), l, Tol);
        }

        internal void UpdateVertex(TopoDS_Vertex vV, double p, TopoDS_Edge e, double v)
        {
            throw new NotImplementedException();
        }
    }
}