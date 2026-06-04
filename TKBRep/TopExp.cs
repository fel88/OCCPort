using TKG3d;

namespace OCCPort
{
    //! This package   provides  basic tools  to   explore the
    //! topological data structures.
    //!
    //! * Explorer : A tool to find all sub-shapes of  a given
    //! type. e.g. all faces of a solid.
    //!
    //! * Package methods to map sub-shapes of a shape.
    //!
    //! Level : Public
    //! All methods of all  classes will be public.
    public class TopExp
    {  //! Returns the Vertex of orientation FORWARD in E. If
       //! there is none returns a Null Shape.
       //! CumOri = True : taking account the edge orientation
        public static TopoDS_Vertex FirstVertex(TopoDS_Edge E,
                   bool CumOri = false)
        {
            TopoDS_Iterator ite = new TopoDS_Iterator(E, CumOri);
            while (ite.More())
            {
                if (ite.Value().Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                    return TopoDS.Vertex(ite.Value());
                ite.Next();
            }
            return new TopoDS_Vertex();
        }
        //! Returns the Vertex of orientation REVERSED in E. If
        //! there is none returns a Null Shape.
        //! CumOri = True : taking account the edge orientation
        public static TopoDS_Vertex LastVertex(TopoDS_Edge E,
                  bool CumOri = false)
        {
            TopoDS_Iterator ite = new TopoDS_Iterator(E, CumOri);
            while (ite.More())
            {
                if (ite.Value().Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                    return TopoDS.Vertex(ite.Value());
                ite.Next();
            }
            return new TopoDS_Vertex();
        }


        //! Stores in the map <M> all the subshape of <S> of
        //! type <TS>  for each one append  to  the list all
        //! the ancestors of type <TA>.  For example map all
        //! the edges and bind the list of faces.
        //! Warning: The map is not cleared at first.
        public static void MapShapesAndAncestors(TopoDS_Shape S,
                    TopAbs_ShapeEnum TS,
                    TopAbs_ShapeEnum TA,
                   TopTools_IndexedDataMapOfShapeListOfShape M)
        {
            //TopTools_ListOfShape empty = new TopTools_ListOfShape();

            // visit ancestors
            TopExp_Explorer exa = new TopExp_Explorer(S, TA);
            while (exa.More())
            {
                // visit shapes
                TopoDS_Shape anc = exa.Current();
                TopExp_Explorer exs = new TopExp_Explorer(anc, TS);
                while (exs.More())
                {
                    int index = M.FindIndex(exs.Current());
                    if (index == 0) index = M.Add(exs.Current(), new TopTools_ListOfShape());
                    M[index].Append(anc);
                    exs.Next();
                }
                exa.Next();
            }

            // visit shapes not under ancestors
            TopExp_Explorer ex = new TopExp_Explorer(S, TS, TA);
            while (ex.More())
            {
                int index = M.FindIndex(ex.Current());
                if (index == 0) index = M.Add(ex.Current(), new TopTools_ListOfShape());
                ex.Next();
            }
        }


        //! Returns  in  Vfirst,  Vlast   the first   and last
        //! vertices of the open wire <W>. May be null shapes.
        //! if   <W>  is closed Vfirst and Vlast  are a same
        //! vertex on <W>.
        //! if <W> is no manifold. VFirst and VLast are null
        //! shapes.
        public static void Vertices(TopoDS_Wire W, ref TopoDS_Vertex Vfirst, ref TopoDS_Vertex Vlast)
        {
            Vfirst = Vlast = new TopoDS_Vertex(); // nullify

            TopTools_MapOfShape vmap = new TopTools_MapOfShape();
            TopoDS_Iterator it = new TopoDS_Iterator(W);
            TopoDS_Vertex V1 = null, V2 = null;

            while (it.More())
            {
                TopoDS_Edge E = TopoDS.Edge(it.Value());
                if (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                    TopExp.Vertices(E, ref V2, ref V1);
                else
                    TopExp.Vertices(E, ref V1, ref V2);
                // add or remove in the vertex map
                V1.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                V2.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                if (!vmap.Add(V1)) vmap.Remove(V1);
                if (!vmap.Add(V2)) vmap.Remove(V2);

                it.Next();
            }
            if (vmap.IsEmpty())
            { // closed
                TopoDS_Shape aLocalShape = V2.Oriented(TopAbs_Orientation.TopAbs_FORWARD);
                Vfirst = TopoDS.Vertex(aLocalShape);
                aLocalShape = V2.Oriented(TopAbs_Orientation.TopAbs_REVERSED);
                Vlast = TopoDS.Vertex(aLocalShape);
                //    Vfirst = TopoDS::Vertex(V2.Oriented(TopAbs_FORWARD));
                //    Vlast  = TopoDS::Vertex(V2.Oriented(TopAbs_REVERSED));
            }
            else if (vmap.Extent() == 2)
            { //open
                TopTools_MapIteratorOfMapOfShape ite = new TopTools_MapIteratorOfMapOfShape(vmap);

                while (ite.More() && ite.Key().Orientation() != TopAbs_Orientation.TopAbs_FORWARD)
                    ite.Next();
                if (ite.More()) Vfirst = TopoDS.Vertex(ite.Key());
                ite.Initialize(vmap);
                while (ite.More() && ite.Key().Orientation() != TopAbs_Orientation.TopAbs_REVERSED)
                    ite.Next();
                if (ite.More()) Vlast = TopoDS.Vertex(ite.Key());
            }
        }



        //! Returns in Vfirst, Vlast the  FORWARD and REVERSED
        //! vertices of the edge <E>. May be null shapes.
        //! CumOri = True : taking account the edge orientation
        public static void Vertices(TopoDS_Edge E, ref TopoDS_Vertex Vfirst,
            ref TopoDS_Vertex Vlast, bool CumOri = false)
        {
            // minor optimization for case when Vfirst and Vlast are non-null:
            // at least for VC++ 10, it is faster if we use boolean flags than 
            // if we nullify vertices at that point (see #27021)
            bool isFirstDefined = false;
            bool isLastDefined = false;

            TopoDS_Iterator ite = new TopoDS_Iterator(E, CumOri);
            while (ite.More())
            {
                TopoDS_Shape aV = ite.Value();
                if (aV.Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                {
                    Vfirst = TopoDS.Vertex(aV);
                    isFirstDefined = true;
                }
                else if (aV.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                {
                    Vlast = TopoDS.Vertex(aV);
                    isLastDefined = true;
                }
                ite.Next();
            }

            if (!isFirstDefined)
                Vfirst.Nullify();

            if (!isLastDefined)
                Vlast.Nullify();
        }
    }
}

