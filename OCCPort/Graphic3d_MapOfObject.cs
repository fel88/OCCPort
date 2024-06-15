using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class Graphic3d_MapOfObject
    {
        public Dictionary<object, Graphic3d_ViewAffinity> map = new Dictionary<object, Graphic3d_ViewAffinity>();
        internal void Bind(object theObject, Graphic3d_ViewAffinity theAffinity)
        {
            map.Add(theObject, theAffinity);
        }

        internal bool Find(object theObject, out Graphic3d_ViewAffinity aResult)
        {
            aResult = map[theObject];
            return map.ContainsKey(theObject);
        }
    }
}