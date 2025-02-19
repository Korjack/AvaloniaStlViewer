using System.Collections.Generic;
using OpenTK.Mathematics;

namespace AvaloniaStlViewer.STL;

public class StlMesh
{
    public List<Vector3> Vertices { get; } = new();
    public List<Vector3> Normals { get; } = new();
}