public class PathMesh : UnityEngine.MonoBehaviour 
{
    UnityEngine.Vector3[] newVertices;
    UnityEngine.Vector2[] newUV;
    int[] newTriangles;
    UnityEngine.Mesh mesh;

    public void Construct(System.Collections.Generic.List<UnityEngine.Vector3> pathPoints)
    {
        mesh = GetComponent<UnityEngine.MeshFilter>().mesh;

        int i, v;
        if (mesh.vertices.Length != pathPoints.Count + 1)
        {
            mesh.Clear();
            newVertices = new UnityEngine.Vector3[pathPoints.Count + 1];
            newTriangles = new int[(pathPoints.Count - 1) * 3];

            i = 0;
            v = 0;
            while (i < newTriangles.Length - 6)
            {
                if ((i % 6) == 0)
                {
                    newTriangles[i] = v;
                    newTriangles[i + 1] = v+1;
                    newTriangles[i + 2] = v + 3;
                    

                    newTriangles[i + 3] = v;
                    newTriangles[i + 4] = v + 3;
                    newTriangles[i + 5] = v + 2;

                    v += 2;

                    //1,2,4,
                    //1,4,3
                }
                i++;
            }
        }


        for (i = 0; i < pathPoints.Count; i++)
        {
            newVertices[i] = transform.InverseTransformPoint(pathPoints[i]);
        }

        newUV = new UnityEngine.Vector2[newVertices.Length];
        i = 0;
        while (i < newUV.Length)
        {
            newUV[i] = new UnityEngine.Vector2(newVertices[i].x, newVertices[i].z);
            i++;
        }

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUV;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
