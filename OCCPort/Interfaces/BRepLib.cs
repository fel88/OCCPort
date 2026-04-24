using OCCPort.Tester;
using System;
using System.Numerics;
using System.Reflection.Metadata;

namespace OCCPort.Interfaces
{
    internal class BRepLib
    {

        public static void ReverseSortFaces(TopoDS_Shape Sh, TopTools_ListOfShape LF)
        {
            LF.Clear();
            // Use the allocator of the result LF for intermediate results
            TopTools_ListOfShape LTri = new TopTools_ListOfShape(LF.Allocator()),
                LPlan = new TopTools_ListOfShape(LF.Allocator()),
    LCyl = new TopTools_ListOfShape(LF.Allocator()), LCon = new TopTools_ListOfShape(LF.Allocator()), LSphere = new TopTools_ListOfShape(LF.Allocator()),
    LTor = new TopTools_ListOfShape(LF.Allocator()), LOther = new TopTools_ListOfShape(LF.Allocator());
            TopExp_Explorer exp = new TopExp_Explorer(Sh, TopAbs_ShapeEnum.TopAbs_FACE);
            TopLoc_Location l = new TopLoc_Location();

            for (; exp.More(); exp.Next())
            {
                TopoDS_Face F = TopoDS.Face(exp.Current());
                Geom_Surface S = BRep_Tool.Surface(F, l);
                if (S != null)
                {
                    GeomAdaptor_Surface AS = new GeomAdaptor_Surface(S);
                    switch (AS._GetType())
                    {
                        case GeomAbs_SurfaceType.GeomAbs_Plane:
                            {
                                LPlan.Append(F);
                                break;
                            }
                            //case GeomAbs_Cylinder:
                            //    {
                            //        LCyl.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Cone:
                            //    {
                            //        LCon.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Sphere:
                            //    {
                            //        LSphere.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Torus:
                            //    {
                            //        LTor.Append(F);
                            //        break;
                            //    }
                            //default:
                            //    LOther.Append(F);
                    }
                }
                else LTri.Append(F);
            }
            LF.Append(LTri); LF.Append(LOther); LF.Append(LTor); LF.Append(LSphere);
            LF.Append(LCon); LF.Append(LCyl); LF.Append(LPlan);

        }
    }
}