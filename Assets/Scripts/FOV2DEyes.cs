using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FOV2DEyes : UnityEngine.MonoBehaviour
{
	public bool raysGizmosEnabled;
	//public float updateRate = 0.02f;	
	public int fovAngle = 90;
	public float fovMaxDistance = 15;    
    public List<UnityEngine.RaycastHit> hits = new List<UnityEngine.RaycastHit>();

    public FOV2DVisionCone[] visionCones;	

    public Guard guard;

    public UnityEngine.Vector3 dirCache;

	void Update()
	{        
        if (guard != null && guard.moving.canMove)
        {
            CastRays(guard.moving.currentDir);            
        }
        else
        {
            CastRays(dirCache);
        }
	}
	
	void Start() 
	{
        guard = GetComponentInParent<Guard>();
        visionCones = GetComponentsInChildren<FOV2DVisionCone>();
	}
    
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesInEye = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEyeTemp;
	public void CastRays(UnityEngine.Vector3 dir, bool needMsg = true)
	{        
        if (guard != null)
        {
            System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEyeTemp
                = new System.Collections.Generic.List<UnityEngine.GameObject>(enemiesInEye.ToArray());

            // 8 ,wall
            // 11,magician
            // 20,Dove
            // 24,GemHolder
            int cullingMask = 1 << 8 | 1 << 11 | 1 << 20 | 1 << 24;

            _castRays(fovAngle, dir, fovMaxDistance, ref enemiesOutEyeTemp, visionCones[0], needMsg, cullingMask);
            _castRays(360 - fovAngle + 10, dir, 0.8f, ref enemiesOutEyeTemp, visionCones[1], needMsg, cullingMask);

            if (needMsg)
            {
                foreach (UnityEngine.GameObject enemy in enemiesOutEyeTemp)
                {
                    if (enemy.GetComponent<Actor>() == null || !enemy.GetComponent<Actor>().inLight)
                    {
                        guard.EnemyOutEye(enemy);
                        enemiesInEye.Remove(enemy);
                    }
                }
            }
        }
        else
        {
            // 8 ,wall
            // 13,guard
            // 20,Dove
            // 24,GemHolder
            int cullingMask = 1 << 8 | 1 << 13 | 1 << 20 | 1 << 24;
            dir = UnityEngine.Vector3.one;
            _castRays(fovAngle, dir, fovMaxDistance, ref enemiesOutEyeTemp, visionCones[0], false, cullingMask);
        }
        
        
        dirCache = dir.normalized;
	}

    public void SetVisionStatus(FOV2DVisionCone.Status status)
    {
        foreach(FOV2DVisionCone cone in visionCones)
        {
            cone.status = status;
            cone.UpdateMeshMaterial();
        }
    }

    void _castRays(float angle, UnityEngine.Vector3 dir, float rayLength,
        ref System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEye,
        FOV2DVisionCone cone,
        bool needMsg,
        int cullingMask)
    {
        hits.Clear();
        float quality = 0.2f;
        int numRays = (int)(angle * quality);        
        float currentAngle = angle / -2;
        for (int i = 0; i < numRays; i++)
        {
            dir = dir.normalized;
            dir = new UnityEngine.Vector3((float)System.Math.Round(dir.x, 3), (float)System.Math.Round(dir.y, 3), (float)System.Math.Round(dir.z, 3));
            UnityEngine.Vector3 direction = UnityEngine.Quaternion.AngleAxis(currentAngle, UnityEngine.Vector3.back) * dir.normalized;
            // 为了让两个fov连成一片
            if (angle > 180)
            {
                direction = -direction;
            }
            UnityEngine.RaycastHit hit = new UnityEngine.RaycastHit();
            
            //direction = new UnityEngine.Vector3(Globals.Floor2(direction.x), Globals.Floor2(direction.y), Globals.Floor2(direction.z));
            rayLength = (float)System.Math.Round(rayLength,3);         
   
            if(i % 30 == 0)
            {
                System.String record_content = gameObject.name + " eye ray:";
                record_content += " " + direction.ToString("F5");
                record_content += " " + rayLength.ToString("F5");
                Globals.record("testReplay", record_content);
            }

            UnityEngine.Vector3 rayOrigin = transform.position;
            if (UnityEngine.Physics.Raycast(rayOrigin, direction, out hit, rayLength, cullingMask) == false)
            {
                hit.point = rayOrigin + (direction * rayLength);
            }
            else if (needMsg)
            {
                if (hit.collider.gameObject.layer == 11 ||
                    hit.collider.gameObject.layer == 20 ||
                    hit.collider.gameObject.layer == 24)
                {
                    if (!enemiesInEye.Contains(hit.collider.gameObject))
                    {
                        enemiesInEye.Add(hit.collider.gameObject);
                        guard.SpotEnemy(hit.collider.gameObject);
                    }
                    else
                    {
                        guard.EnemyStayInEye(hit.collider.gameObject);
                    }
                    enemiesOutEye.Remove(hit.collider.gameObject);
                }
            }
            // mage fov
            if(gameObject.layer == 25)
            {
                hit.point = hit.point + new UnityEngine.Vector3(0, 0.2f, 0);
            }            
            hits.Add(hit);
            currentAngle += 1f / quality;
        }
        cone.UpdateMesh(hits);
    }
	
	void OnDrawGizmosSelected()
	{
        UnityEngine.Gizmos.color = UnityEngine.Color.cyan;
		
		if (raysGizmosEnabled && hits.Count() > 0) 
		{
            foreach (UnityEngine.RaycastHit hit in hits)
			{
                UnityEngine.Gizmos.DrawSphere(hit.point, 0.04f);
                UnityEngine.Gizmos.DrawLine(transform.position, hit.point);
			}
		}
	}

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        visionCones[0].gameObject.layer = layer;
        visionCones[1].gameObject.layer = layer;
    }
}
