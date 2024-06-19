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

        internal bool Find(AIS_InteractiveObject theObject, out Graphic3d_ViewAffinity aResult)
        {
            if (map.ContainsKey(theObject))
        {
            aResult = map[theObject];
                return true;
            }
            aResult = null;
            return false;            
        }
    }
}