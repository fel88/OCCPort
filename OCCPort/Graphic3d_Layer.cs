using System;

namespace OCCPort
{
    public class Graphic3d_Layer
	{
        public void Add(Graphic3d_CStructure theStruct, Graphic3d_DisplayPriority thePriority, bool isForChangePriority)
        {
            throw new NotImplementedException();
        }

        public bool IsImmediate()
        {
            throw new NotImplementedException();
        }

        public Graphic3d_IndexedMapOfStructure Structures(Graphic3d_DisplayPriority aPriorityIter)
		{
			throw new NotImplementedException();
		}

		internal Bnd_Box BoundingBox(object v, object aCamera, object value1, object value2, bool theToIncludeAuxiliary)
		{
			throw new NotImplementedException();
		}

		internal void InvalidateBoundingBox()
		{
			throw new NotImplementedException();
		}

		internal int NbOfTransformPersistenceObjects()
		{
			throw new NotImplementedException();
		}

	}
    public class Graphic3d_IndexedMapOfStructure
    {
    }

}