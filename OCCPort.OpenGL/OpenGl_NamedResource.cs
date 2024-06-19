using System.Threading;

namespace OCCPort
{
    public class OpenGl_NamedResource : OpenGl_Resource
    {
        public OpenGl_NamedResource()
        {

        }
        public OpenGl_NamedResource(string theId)
        {
            myResourceId = (theId);

        }  //! Return resource name.
        public string ResourceId() { return myResourceId; }

        string myResourceId; //!< resource name

    }
}