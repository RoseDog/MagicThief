using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FOV2DEyes : UnityEngine.MonoBehaviour
{
	public bool raysGizmosEnabled;
	//public float updateRate = 0.02f;	
	public int fovAngle = 90;
	public float fovMaxDistance = 15;    
    public UnityEngine.LayerMask cullingMask;
    public List<UnityEngine.RaycastHit> hits = new List<UnityEngine.RaycastHit>();

    public FOV2DVisionCone visionCone;	

    public Guard guard;
	
	void Update()
	{
		CastRays();

//         if (guard.target != null)
//         {
//             float distFromEnemy = Vector3.Distance(guard.target.transform.position, guard.transform.position);
//             if (!this.IsInvoking("EnemyOutVision"))
//             {
//                 if (distFromEnemy > fovMaxDistance)
//                 {
//                     this.Invoke("EnemyOutVision", 0.5f);
//                 }
//             }
//             else if (distFromEnemy < fovMaxDistance)
//             {
//                 this.CancelInvoke("EnemyOutVision");
//             }
//         }
        
	}
	
	void Start() 
	{
		//InvokeRepeating("CastRays", 0, updateRate);
        guard = GetComponentInParent<Guard>();
        visionCone = GetComponent<FOV2DVisionCone>();
	}
	
	void CastRays()
	{
        int numRays;
        float currentAngle;
        UnityEngine.Vector3 direction;
        UnityEngine.RaycastHit hit;
        float quality = 0.2f;
        numRays = (int)(fovAngle * quality);
		currentAngle = fovAngle / -2;
		
		hits.Clear();
		
		for (int i = 0; i < numRays; i++)
		{
            direction = UnityEngine.Quaternion.AngleAxis(currentAngle, transform.up) * transform.forward;
            hit = new UnityEngine.RaycastHit();

            if (UnityEngine.Physics.Raycast(transform.position, direction, out hit, fovMaxDistance, cullingMask) == false)
			{
				hit.point = transform.position + (direction * fovMaxDistance);
			}
			
			hits.Add(hit);

            currentAngle += 1f / quality;
		}
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
    
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        // 由于FOV的更新频率问题，有时候墙后的也会被看到
        if (!guard.IsBlockByWall(other.gameObject))
        {
            return;
        }
        // 如果是宝石，检查是否被偷
        if (guard.realiseGemLost != null && 
            guard.realiseGemLost != guard.currentAction &&
            guard.spot.target == null && 
            other.gameObject == guard.guardedGemHolder)
        {            
            if (other.gameObject.layer == 24 && other.GetComponentInChildren<Gem>() == null)
            {
                guard.realiseGemLost.Excute();
                return;
            }
        }
        
        if (other.enabled)
        {
            guard.SpotEnemy(other.gameObject);            
        }                
    }    
}
