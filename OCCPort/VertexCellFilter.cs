using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    internal class VertexCellFilter : NCollection_CellFilter<BRepMesh_VertexInspector>
    {
        public VertexCellFilter(double v) : base(v, BRepMesh_VertexInspector.Dimension)
        {
        }
    }
}