using System;

namespace OCCPort
{
    //! An Explorer is a Tool to visit  a Topological Data
    //! Structure form the TopoDS package.
    //!
    //! An Explorer is built with :
    //!
    //! * The Shape to explore.
    //!
    //! * The type of Shapes to find : e.g VERTEX, EDGE.
    //! This type cannot be SHAPE.
    //!
    //! * The type of Shapes to avoid. e.g  SHELL, EDGE.
    //! By default   this type is  SHAPE which  means no
    //! restriction on the exploration.
    //!
    //! The Explorer  visits  all the  structure   to find
    //! shapes of the   requested  type  which   are   not
    //! contained in the type to avoid.
    //!
    //! Example to find all the Faces in the Shape S :
    //!
    //! TopExp_Explorer Ex;
    //! for (Ex.Init(S,TopAbs_FACE); Ex.More(); Ex.Next()) {
    //! ProcessFace(Ex.Current());
    //! }
    //!
    //! // an other way
    //! TopExp_Explorer Ex(S,TopAbs_FACE);
    //! while (Ex.More()) {
    //! ProcessFace(Ex.Current());
    //! Ex.Next();
    //! }
    //!
    //! To find all the vertices which are not in an edge :
    //!
    //! for (Ex.Init(S,TopAbs_VERTEX,TopAbs_EDGE); ...)
    //!
    //! To  find all the faces  in   a SHELL, then all the
    //! faces not in a SHELL :
    //!
    //! TopExp_Explorer Ex1, Ex2;
    //!
    //! for (Ex1.Init(S,TopAbs_SHELL),...) {
    //! // visit all shells
    //! for (Ex2.Init(Ex1.Current(),TopAbs_FACE),...) {
    //! // visit all the faces of the current shell
    //! }
    //! }
    //!
    //! for (Ex1.Init(S,TopAbs_FACE,TopAbs_SHELL),...) {
    //! // visit all faces not in a shell
    //! }
    //!
    //! If   the type  to avoid  is   the same  or is less
    //! complex than the type to find it has no effect.
    //!
    //! For example searching edges  not in a vertex  does
    //! not make a difference.

    internal class TopExp_Explorer
    {
        public TopExp_Explorer(TopoDS_Shape theS,
            TopAbs_ShapeEnum theToFind,
            TopAbs_ShapeEnum theToAvoid = TopAbs_ShapeEnum.TopAbs_SHAPE
)
        {
            //myStack = (0L);
            myTop = (-1);
            mySizeOfStack = (theStackSize);
            toFind = (theToFind);
            toAvoid = (theToAvoid);
            hasMore = false;
            myStack = new TopExp_Stack();

            Init(theS, theToFind, theToAvoid);

        }
        const int theStackSize = 20;
        private void Init(TopoDS_Shape S, TopAbs_ShapeEnum ToFind, TopAbs_ShapeEnum ToAvoid)
        {

            Clear();

            myShape = S;
            toFind = ToFind;
            toAvoid = ToAvoid;

            if (S.IsNull())
            {
                hasMore = false;
                return;
            }

            if (toFind == TopAbs_ShapeEnum.TopAbs_SHAPE)
                hasMore = false;

            else
            {
                TopAbs_ShapeEnum ty = S.ShapeType();

                if (LESSCOMPLEX(ty, toFind))
                {
                    // the first Shape is less complex, nothing to find
                    hasMore = false;
                }
                else if (!SAMETYPE(ty, toFind))
                {
                    // type is more complex search inside
                    hasMore = true;
                    Next();
                }
                else
                {
                    // type is found
                    hasMore = true;
                }
            }
        }

        private void Clear()
        {
            hasMore = false;
            myStack.list.Clear();
            /*for (int i = 0; i <= myTop; ++i)
			{
				myStack[i].~TopoDS_Iterator();
			}*/
            myTop = -1;

        }

        internal TopoDS_Shape Current()
        {
            //Standard_NoSuchObject_Raise_if(!hasMore, "TopExp_Explorer::Current");
            if (myTop >= 0)
            {
                //TopoDS_Shape S = myStack[myTop].Value();
                TopoDS_Shape S = myStack[myTop].Value();
                return S;
            }
            else
                return myShape;

        }

        //! Returns True if there are more shapes in the exploration.
        public bool More() { return hasMore; }

        //typedef TopoDS_Iterator* TopExp_Stack;
        TopExp_Stack myStack = new TopExp_Stack();
        TopoDS_Shape myShape;
        int myTop;
        int mySizeOfStack;
        TopAbs_ShapeEnum toFind;
        TopAbs_ShapeEnum toAvoid;
        bool hasMore;
        // macro to compare two types of shapes
        // always True if the first one is SHAPE
        bool SAMETYPE(TopAbs_ShapeEnum x, TopAbs_ShapeEnum y)
        {
            return (x) == (y);
        }
        bool AVOID(TopAbs_ShapeEnum x, TopAbs_ShapeEnum y)
        {
            return ((x) == TopAbs_ShapeEnum.TopAbs_SHAPE) ? false : (x) == (y);
        }

        bool LESSCOMPLEX(TopAbs_ShapeEnum x, TopAbs_ShapeEnum y)
        {
            return (x) > (y);
        }

        internal void Next()
        {
            int NewSize;
            TopoDS_Shape ShapTop;
            TopAbs_ShapeEnum ty;
            if (!hasMore)
            {
                throw new Standard_NoMoreObject_Exception("TopExp_Explorer::Next");
            }
            //Standard_NoMoreObject_Raise_if(!hasMore, "TopExp_Explorer::Next");

            if (myTop < 0)
            {
                // empty stack. Entering the initial shape.
                ty = myShape.ShapeType();

                if (SAMETYPE(toFind, ty))
                {
                    // already visited once
                    hasMore = false;
                    return;
                }
                else if (AVOID(toAvoid, ty))
                {
                    // avoid the top-level
                    hasMore = false;
                    return;
                }
                else
                {
                    // push and try to find
                    if (++myTop >= mySizeOfStack)
                    {
                        NewSize = mySizeOfStack + theStackSize;
                        //TopExp_Stack newStack = (TopoDS_Iterator*)Standard::Allocate(NewSize * sizeof(TopoDS_Iterator));
                        TopExp_Stack newStack = new TopExp_Stack();
                        int i;
                        for (i = 0; i < myTop; i++)
                        {
                            var iter = new TopoDS_Iterator(myStack[i].Value());
                            newStack[i] = iter;
                            //new(&newStack[i]) TopoDS_Iterator(myStack[i]);
                            //myStack[i].~TopoDS_Iterator();
                        }
                        //Standard::Free(myStack);
                        mySizeOfStack = NewSize;
                        myStack = newStack;
                    }
                    myStack.Add(new TopoDS_Iterator(myShape));
                    //myStack[myTop] = new TopoDS_Iterator(myShape);
                    //new(&myStack[myTop]) TopoDS_Iterator(myShape);
                }
            }
            else myStack[myTop].Next();


            for (; ; )
            {
                if (myStack[myTop].More())
                {
                    ShapTop = myStack[myTop].Value();
                    ty = ShapTop.ShapeType();
                    if (SAMETYPE(toFind, ty))
                    {
                        hasMore = true;
                        return;
                    }
                    else if (LESSCOMPLEX(toFind, ty) && !AVOID(toAvoid, ty))
                    {
                        if (++myTop >= mySizeOfStack)
                        {
                            NewSize = mySizeOfStack + theStackSize;
                            //TopExp_Stack newStack = (TopoDS_Iterator*)Standard::Allocate(NewSize * sizeof(TopoDS_Iterator));
                            TopExp_Stack newStack = new TopExp_Stack();
                            int i;
                            for (i = 0; i < myTop; i++)
                            {
                                //new(&newStack[i]) TopoDS_Iterator(myStack[i]);
                                newStack.Add(new TopoDS_Iterator(myStack[i]));

                                //myStack[i].~TopoDS_Iterator();
                            }
                            //Standard::Free(myStack);
                            mySizeOfStack = NewSize;
                            myStack = newStack;
                        }
                        //new(&myStack[myTop]) TopoDS_Iterator(ShapTop);
                        myStack.Add(new TopoDS_Iterator(ShapTop));

                    }
                    else
                    {
                        myStack[myTop].Next();
                    }
                }
                else
                {
                    //myStack[myTop].~TopoDS_Iterator();
                    myTop--;
                    if (myTop < 0) break;
                    myStack[myTop].Next();
                }
            }
            hasMore = false;
        }
    }
}