using OCCPort;
using TKernel;

namespace TKService
{
    public class Graphic3d_MapOfObject : NCollection_DataMap<object, Graphic3d_ViewAffinity, NCollection_DefaultHasher<object>>
    {
        //public Dictionary<object, Graphic3d_ViewAffinity> map = new Dictionary<object, Graphic3d_ViewAffinity>();
        //internal void Bind(object theObject, Graphic3d_ViewAffinity theAffinity)
        //{
        //    map.Add(theObject, theAffinity);
        //}

       
    }

}
