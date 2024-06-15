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


        internal void MakeFace(TopoDS_Face f, Geom_Plane geom_Plane, double v)
        {
            throw new NotImplementedException();
        }

        internal void MakeShell(TopoDS_Shell s)
        {
            throw new NotImplementedException();
        }

        internal void MakeSolid(object value)
        {
            throw new NotImplementedException();
        }
    }
}