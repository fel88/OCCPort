using System;

namespace OCCPort
{
    public class TopoDS_Builder
    {
        public void MakeWire(TopoDS_Wire W)
        {
            TopoDS_TWire TW = new TopoDS_TWire();
            MakeShape(W, TW);
        }
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
            //if (aShape.Free())
            if (true)
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

        //! Make a Solid covering the whole 3D space.
        public void MakeSolid(TopoDS_Solid S)
        {
            TopoDS_TSolid TS = new TopoDS_TSolid();
            MakeShape(S, TS);
        }

        //=======================================================================
        public void MakeShape(TopoDS_Shape S,
                                   TopoDS_TShape T)
        {
            S.TShape(T);
            S.Location(new TopLoc_Location());
            S.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
        }


        public void MakeShell(TopoDS_Shell S)
        {
            TopoDS_TShell TS = new TopoDS_TShell();
            MakeShape(S, TS);
        }

    }
}