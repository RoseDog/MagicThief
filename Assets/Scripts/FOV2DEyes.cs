using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FOV2DEyes : UnityEngine.MonoBehaviour
{
	public bool raysGizmosEnabled;
	//public float updateRate = 0.02f;	
	public int fovAngle = 90;
    public float fovMaxDistance = 15;
    public float aural;
    List<UnityEngine.Vector3> hits = new List<UnityEngine.Vector3>();
    public FOV2DVisionCone[] visionCones;	

    public Guard guard;

    public UnityEngine.Vector3 dirCache;

	public void FrameFunc()
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

    void Awake()
    {
        visionCones = GetComponentsInChildren<FOV2DVisionCone>();
    }
	
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesInEye = new System.Collections.Generic.List<UnityEngine.GameObject>();
    public System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEyeTemp;
	public void CastRays(UnityEngine.Vector3 dir, bool needMsg = true)
	{       
        if(visionCones.Length != 0)
        {
            if (guard != null)
            {
                System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEyeTemp
                    = new System.Collections.Generic.List<UnityEngine.GameObject>(enemiesInEye.ToArray());

                // 8 ,wall
                // 11,magician
                // 20,Dove            
                int static_obj_cullingMask = 1 << 8;
                int move_obj_culling_mask = 1 << 11 | 1 << 20;

                List<UnityEngine.Vector3> beginPoints = new List<UnityEngine.Vector3>();
                hits.Clear();

                _castRays(fovAngle, dir, fovMaxDistance, ref enemiesOutEyeTemp, visionCones[0], needMsg, static_obj_cullingMask, ref hits, ref beginPoints, move_obj_culling_mask);
//                 _castRays(360 - fovAngle + 10, dir, aural, ref enemiesOutEyeTemp, visionCones[1], needMsg, static_obj_cullingMask, move_obj_culling_mask);                

                // 14,Chest
                int cullingMask = 1 << 14;
                _castRaysOnChest(fovAngle, dir, fovMaxDistance, ref enemiesOutEyeTemp, cullingMask);

                if (needMsg)
                {
                    foreach (UnityEngine.GameObject enemy in enemiesOutEyeTemp)
                    {
                        if (enemy != null && (enemy.GetComponent<Actor>() == null || !enemy.GetComponent<Actor>().inLight))
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
                List<UnityEngine.Vector3> beginPoints = new List<UnityEngine.Vector3>();
                hits.Clear();

                int static_obj_cullingMask = 1 << 8;
                int move_obj_culling_mask = 1 << 13 | 1 << 20;

                dir = UnityEngine.Vector3.left;
                _castRays(fovAngle, dir, fovMaxDistance, ref enemiesOutEyeTemp, visionCones[0], false, static_obj_cullingMask, ref hits, ref beginPoints, move_obj_culling_mask);
            }
            dirCache = dir.normalized;
        }                              
	}

    public void SetVisionStatus(FOV2DVisionCone.Status status)
    {
        foreach(FOV2DVisionCone cone in visionCones)
        {
            cone.status = status;
            cone.UpdateColor();
        }
    }

    void _castRays(float angle, UnityEngine.Vector3 dir, float rayLength,
        ref System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEye,
        FOV2DVisionCone cone,
        bool needMsg,
        int static_obj_cullingMask,
        ref List<UnityEngine.Vector3> hits,
        ref List<UnityEngine.Vector3> beginPoints,
        int move_obj_culling_mask)
    {
        float quality = 0.2f;
        int numRays = (int)(angle * quality);
        float currentAngle = angle / -2;        
        for (int i = 0; i < numRays; i++)
        {
            dir = dir.normalized;            
            UnityEngine.Vector3 direction = UnityEngine.Quaternion.AngleAxis(currentAngle, UnityEngine.Vector3.back) * dir.normalized;
            // Ϊ��������fov����һƬ
            if (angle > 180)
            {
                direction = -direction;
            }


            if (Globals.DEBUG_REPLAY && i % 30 == 0)
            {
                System.String record_content;
                if (guard)
                {
                    record_content = guard.gameObject.name + " " + guard.idx.ToString() + " eye ray:";
                }
                else
                {
                    record_content = gameObject.name + " eye ray:";
                }
                record_content += " " + direction.ToString("F5");
                record_content += " " + rayLength.ToString("F5");
                Globals.record("testReplay", record_content);
            }


            UnityEngine.Ray ray = new UnityEngine.Ray(transform.position, direction);
            UnityEngine.GameObject collide_obj = null;
            UnityEngine.Vector3 hit_point;
            if (!RayToObjs(ray, rayLength, static_obj_cullingMask, move_obj_culling_mask, out collide_obj, out hit_point))
            {
                hit_point = ray.origin + (direction * rayLength);
            }
            else if (needMsg)
            {
                if (collide_obj.layer == 11 ||
                    collide_obj.layer == 20)
                {
                    if (!enemiesInEye.Contains(collide_obj))
                    {
                        enemiesInEye.Add(collide_obj);
                        guard.SpotEnemy(collide_obj);
                    }
                    else
                    {
                        guard.EnemyStayInEye(collide_obj);
                    }
                    enemiesOutEye.Remove(collide_obj);                   
                }
            }
            // mage fov
            if(gameObject.layer == 25)
            {
                hit_point = hit_point + new UnityEngine.Vector3(0, 0.2f, 0);
            }

            hits.Add(hit_point);
            beginPoints.Add(ray.origin + (direction * 50));
            currentAngle += 1f / quality;
        }

        visionCones[0].UpdateMesh(hits, beginPoints);
    }

    public bool RayToObjs(UnityEngine.Ray ray, 
        float length,
        int static_obj_cullingMask, 
        int move_obj_cullingMask, out UnityEngine.GameObject collide_obj, out UnityEngine.Vector3 hit_point)
    {
        collide_obj = null;
        hit_point = UnityEngine.Vector3.zero;

        UnityEngine.GameObject hitted_actor = null;
        UnityEngine.Vector3 hitted_actor_point = UnityEngine.Vector3.zero;
        foreach (Actor actor in Globals.actors)
        {
            if (actor != null && ((1 << actor.gameObject.layer) & move_obj_cullingMask) != 0)
            {

                float distance = 0.0f;
                if (actor.characterController.bounds.IntersectRay(ray, out distance))
                {
                    if (distance < length)
                    {
                        hitted_actor = actor.gameObject;
                        hitted_actor_point = ray.origin + distance * ray.direction;
                        break;
                    }
                }
            }
        }

        UnityEngine.GameObject hitted_wall = null;
        UnityEngine.Vector3 hitted_wall_point = UnityEngine.Vector3.zero;
        UnityEngine.RaycastHit hit = new UnityEngine.RaycastHit();
        if(UnityEngine.Physics.Raycast(ray, out hit, length, static_obj_cullingMask))
        {
            hitted_wall = hit.collider.gameObject;
            hitted_wall_point = hit.point;            
        }

        if (hitted_actor == null && hitted_wall == null)
        {            
            return false;
        }

        else if (hitted_actor != null && hitted_wall == null)
        {
            collide_obj = hitted_actor;
            hit_point = hitted_actor_point;
        }

        else if (hitted_actor == null && hitted_wall != null)
        {
            collide_obj = hitted_wall;
            hit_point = hitted_wall_point;
        }
        else
        {
            // 
            if(UnityEngine.Vector3.Distance(hitted_wall_point, ray.origin) < UnityEngine.Vector3.Distance(hitted_actor_point, ray.origin) - 10 )
            {
                collide_obj = hitted_wall;
                hit_point = hitted_wall_point;                
            }
            else
            {
                collide_obj = hitted_actor;
                hit_point = hitted_actor_point;
            }
        }

        return true;
    }

   
    void _castRaysOnChest(float angle, UnityEngine.Vector3 dir, float rayLength,
        ref System.Collections.Generic.List<UnityEngine.GameObject> enemiesOutEye,
        int cullingMask)
    {
        float quality = 0.1f;
        int numRays = (int)(angle * quality);
        float currentAngle = angle / -2;
        for (int i = 0; i < numRays; i++)
        {
            dir = dir.normalized;
            dir = new UnityEngine.Vector3((float)System.Math.Round(dir.x, 3), (float)System.Math.Round(dir.y, 3), (float)System.Math.Round(dir.z, 3));
            UnityEngine.Vector3 direction = UnityEngine.Quaternion.AngleAxis(currentAngle, UnityEngine.Vector3.back) * dir.normalized;
            
            UnityEngine.RaycastHit hit = new UnityEngine.RaycastHit();

            
            rayLength = (float)System.Math.Round(rayLength, 3);

            UnityEngine.Vector3 rayOrigin = transform.position;
            if (UnityEngine.Physics.Raycast(rayOrigin, direction, out hit, rayLength, cullingMask) == false)
            {
                hit.point = rayOrigin + (direction * rayLength);
            }
            else
            {
                if (!enemiesInEye.Contains(hit.collider.gameObject))
                {
                    enemiesInEye.Add(hit.collider.gameObject);
                    guard.CheckChest(hit.collider.gameObject);
                }                
            }
            currentAngle += 1f / quality;
        }
    }
	
	void OnDrawGizmosSelected()
	{
        UnityEngine.Gizmos.color = UnityEngine.Color.cyan;
		
		if (raysGizmosEnabled && hits.Count() > 0) 
		{
            foreach (UnityEngine.Vector3 hit in hits)
			{
                UnityEngine.Gizmos.DrawSphere(hit, 0.04f);
                UnityEngine.Gizmos.DrawLine(transform.position, hit);
			}
		}
	}

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        visionCones[0].gameObject.layer = layer;
        visionCones[1].gameObject.layer = layer;
    }

    public void SetVisonConesVisible(bool visible)
    {
        if (visionCones.Length != 0)
        {
            visionCones[0].meshRenderer.enabled = visible;
            visionCones[1].meshRenderer.enabled = visible;
        }
    }
}
