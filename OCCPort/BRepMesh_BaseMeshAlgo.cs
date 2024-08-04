namespace OCCPort
{
    //! Class provides base functionality for algorithms building face triangulation.
    //! Performs initialization of BRepMesh_DataStructureOfDelaun and nodes map structures.
    public abstract class BRepMesh_BaseMeshAlgo : IMeshTools_MeshAlgo
    {
        //! Gets mesh structure.
        public BRepMesh_DataStructureOfDelaun getStructure()
        {
            return myStructure;
        }
        Poly_Triangulation collectTriangles()
        {
            MapOfInteger aTriangles = myStructure.ElementsOfDomain();
            //if (aTriangles.IsEmpty())
            //{
            //    return new Poly_Triangulation();
            //}

            Poly_Triangulation aRes = new Poly_Triangulation();
            //aRes.ResizeTriangles(aTriangles.Extent(), false);
            IteratorOfMapOfInteger aTriIt = new IteratorOfMapOfInteger(aTriangles);
            for (int aTriangeId = 1; aTriIt.More(); aTriIt.Next(), ++aTriangeId)
            {
                //     BRepMesh_Triangle aCurElem = myStructure.GetElement(aTriIt.Key());

                int[] aNode = new int[3];
                //    myStructure.ElementNodes(aCurElem, aNode);

                //    for (int i = 0; i < 3; ++i)
                //    {
                //        if (!myUsedNodes.IsBound(aNode[i]))
                //        {
                //            myUsedNodes.Bind(aNode[i], myUsedNodes.Size() + 1);
                //        }

                //        aNode[i] = myUsedNodes.Find(aNode[i]);
                //    }

                aRes.SetTriangle(aTriangeId, new Poly_Triangle(aNode[0], aNode[1], aNode[2]));
            }
            //aRes.ResizeNodes(myUsedNodes.Extent(), false);
            //aRes.AddUVNodes();
            return aRes;
        }

        //! Commits generated triangulation to TopoDS face.
        private void commitSurfaceTriangulation()
        {
            Poly_Triangulation aTriangulation = collectTriangles();
            if (aTriangulation == null)
            {
                //myDFace.SetStatus(IMeshData_Failure);
                return;
            }

            // collectNodes(aTriangulation);

            //BRepMesh_ShapeTool.AddInFace(myDFace.GetFace(), aTriangulation);
        }


        //! Generates mesh for the contour stored in data structure.
        public abstract void generateMesh(Message_ProgressRange theRange);
        public void Perform(IMeshData_Face theDFace, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            try
            {
                //myDFace = theDFace;
                //myParameters = theParameters;
                //myAllocator = new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE);
                myStructure = new BRepMesh_DataStructureOfDelaun();
                //myNodesMap = new VectorOfPnt(256, myAllocator);
                //myUsedNodes = new DMapOfIntegerInteger(1, myAllocator);

                if (initDataStructure())
                {
                    // if (!theRange.More())
                    {
                        //   return;
                    }
                    generateMesh(theRange);
                    commitSurfaceTriangulation();
                }
            }
            catch (Standard_Failure  /*theException*/)
            {
            }

            //myDFace.Nullify(); // Do not hold link to face.
            myStructure = null;
            //myNodesMap.Nullify();
            //myUsedNodes.Nullify();
            //myAllocator.Nullify();
        }
        BRepMeshData_Face myDFace;
        private bool initDataStructure()
        {

            for (int aWireIt = 0; aWireIt < myDFace.WiresNb(); ++aWireIt)
            {
            }



            return true;
        }

        BRepMesh_DataStructureOfDelaun myStructure;
    }
}