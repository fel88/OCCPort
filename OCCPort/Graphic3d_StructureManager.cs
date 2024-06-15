using System;

namespace OCCPort
{
    public class Graphic3d_StructureManager
    {

        public void RegisterObject(object theObject,
                                                  Graphic3d_ViewAffinity theAffinity)
        {
            Graphic3d_ViewAffinity aResult;
            if (myRegisteredObjects.Find(theObject, out aResult)
             && aResult == theAffinity)
            {
                return;
            }

            myRegisteredObjects.Bind(theObject, theAffinity);
        }

        protected Graphic3d_MapOfObject myRegisteredObjects;
    }
}