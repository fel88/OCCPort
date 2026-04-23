using OCCPort.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    internal class DMapOfIFacePtrsListOfInteger
    {
        public List<KeyValuePair<IMeshData_Face, ListOfInteger>> map = new List<KeyValuePair<IMeshData_Face, ListOfInteger>>();
        internal void Bind(IMeshData_Face theDFace, ListOfInteger listOfInteger)
        {
            map.Add(new KeyValuePair<IMeshData_Face, ListOfInteger>(theDFace, listOfInteger));
        }

        internal ListOfInteger ChangeFind(IMeshData_Face theDFace)
        {
            return map.First(z => z.Key == theDFace).Value;
        }

        internal ListOfInteger Find(IMeshData_Face theDFace)
        {
            return map.First(z => z.Key == theDFace).Value;
        }

        internal bool IsBound(IMeshData_Face theDFace)
        {
            return map.Any(z => z.Key == theDFace);

        }
    }
}