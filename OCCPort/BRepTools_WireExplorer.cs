using System;

namespace OCCPort
{
    internal class BRepTools_WireExplorer
    {
        private TopoDS_Wire wire;

        public BRepTools_WireExplorer(TopoDS_Wire wire)
        {
            this.wire = wire;
        }

        internal TopoDS_Shape Current()
        {
            throw new NotImplementedException();
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal object Next()
        {
            throw new NotImplementedException();
        }
    }
}