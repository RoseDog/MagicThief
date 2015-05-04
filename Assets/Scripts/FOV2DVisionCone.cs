using System.Collections;
using System.Collections.Generic;

[UnityEngine.RequireComponent(typeof(UnityEngine.MeshFilter))]
[UnityEngine.RequireComponent(typeof(UnityEngine.MeshRenderer))]

public class FOV2DVisionCone : UnityEngine.MonoBehaviour 
{
	public enum Status
	{
		Idle,
		Suspicious,  
		Alert
	}
	public Status status;
    public List<UnityEngine.Material> materials;

    UnityEngine.Vector3[] newVertices;
    UnityEngine.Vector2[] newUV;
    int[] newTriangles;
    UnityEngine.Mesh mesh;
    public UnityEngine.MeshRenderer meshRenderer;
    
	int i;
	int v;
	
    void Awake() 
	{
        mesh = GetComponent<UnityEngine.MeshFilter>().mesh;
        meshRenderer = GetComponent<UnityEngine.MeshRenderer>();
		
		meshRenderer.material = materials[0];
    }

	public void UpdateMesh(List<UnityEngine.RaycastHit> hits)
	{		
		if (hits == null || hits.Count == 0)
			return;
		
		if (mesh.vertices.Length != hits.Count + 1)
		{
			mesh.Clear();
            newVertices = new UnityEngine.Vector3[hits.Count + 1];
			newTriangles = new int[(hits.Count - 1) * 3];
			
			i = 0;
			v = 1;
			while (i < newTriangles.Length)
			{
				if ((i % 3) == 0)
				{
					newTriangles[i] = 0;
					newTriangles[i + 1] = v;
					newTriangles[i + 2] = v + 1;
					v++;
				}
				i++;
			}
		}

        newVertices[0] = UnityEngine.Vector3.zero;
		for (i = 1; i <= hits.Count; i++)
		{
			newVertices[i] = transform.InverseTransformPoint(hits[i-1].point);
		}
        
        newUV = new UnityEngine.Vector2[newVertices.Length];
		i = 0;
        while (i < newUV.Length) {
            newUV[i] = new UnityEngine.Vector2(newVertices[i].x, newVertices[i].z);
            i++;
        }
		
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
		mesh.uv = newUV;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

    public void UpdateMeshMaterial()
	{
        meshRenderer.material = materials[(int)status];
	}
}
