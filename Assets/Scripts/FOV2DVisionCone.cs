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
    public UnityEngine.Material material;

    public UnityEngine.Color idle;
    public UnityEngine.Color suspicious;
    public UnityEngine.Color alert;
    UnityEngine.Color color;

    UnityEngine.Vector3[] newVertices;
    UnityEngine.Vector2[] newUV;
    int[] newTriangles;
    UnityEngine.Mesh mesh;
    public UnityEngine.MeshRenderer meshRenderer;   
	
    void Awake() 
	{
        mesh = GetComponent<UnityEngine.MeshFilter>().mesh;
        meshRenderer = GetComponent<UnityEngine.MeshRenderer>();

        if (material != null)
        {
            meshRenderer.material = new UnityEngine.Material(material);
        }        
        status = Status.Idle;
        UpdateColor();
    }

    public void UpdateMesh(List<UnityEngine.Vector3> hits, List<UnityEngine.Vector3> beginPoints)
	{		
		if (hits == null || hits.Count == 0)
			return;

        //if (mesh.vertices.Length != hits.Count + beginPoints.Count)
		{
			mesh.Clear();
            newVertices = new UnityEngine.Vector3[hits.Count + beginPoints.Count];
            int i = 0;
            int v = 0;
            for (i = 0; i < hits.Count; ++i)
            {
                newVertices[v++] = transform.InverseTransformPoint(hits[i]);
                newVertices[v++] = transform.InverseTransformPoint(beginPoints[i]);
            }

            newTriangles = new int[(hits.Count-1) * 2 * 3];
           
			i = 0;
			v = 0;
			while (i < newTriangles.Length)
			{
                newTriangles[i] = v;
                newTriangles[i + 1] = v + 2;
                newTriangles[i + 2] = v + 1;

                newTriangles[i + 3] = v + 2;
                newTriangles[i + 4] = v + 3;
                newTriangles[i + 5] = v + 1;

                v += 2;
				i += 6;
			}
		}

        UnityEngine.Color[] colors = new UnityEngine.Color[newVertices.Length];       
        for (int idx = 0; idx < newVertices.Length; idx++)
        {
            if(idx % 2 == 0)
            {
                colors[idx] = new UnityEngine.Color(color.r, color.g, color.b, color.a);
            }
            else
            {
                colors[idx] = new UnityEngine.Color(color.r, color.g, color.b, color.a * 0.15f);
            }
        }
		
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.colors = colors;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
    
    public void UpdateColor()
	{
        if (status == Status.Idle)
        {
            color = idle;
        }
        else if (status == Status.Alert)
        {
            color = alert;
        }
        else if (status == Status.Suspicious)
        {
            color = suspicious;
        }
	}
}
