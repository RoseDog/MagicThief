using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FOV2DEyes : UnityEngine.MonoBehaviour
{
	public bool raysGizmosEnabled;
	//public float updateRate = 0.02f;
	public int quality = 4;
	public int fovAngle = 90;
	public float fovMaxDistance = 15;
    public float enemyOutVisionTime = 1.0f;
    public UnityEngine.LayerMask cullingMask;
    public List<UnityEngine.RaycastHit> hits = new List<UnityEngine.RaycastHit>();
	
	int numRays;
	float currentAngle;
    UnityEngine.Vector3 direction;
    UnityEngine.RaycastHit hit;

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
	}
	
	void CastRays()
	{
		numRays = fovAngle * quality;
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
        guard.spot.SpotMagician(other.gameObject);

        if (this.IsInvoking("EnemyOutVision"))
        {
            this.CancelInvoke("EnemyOutVision");            
        }
        this.Invoke("EnemyOutVision", enemyOutVisionTime);
    }

    void OnTriggerStay(UnityEngine.Collider other)
    {
        if (this.IsInvoking("EnemyOutVision"))
        {
            //this.CancelInvoke("EnemyOutVision");
        }
    }

    void EnemyOutVision()
    {
        UnityEngine.Debug.Log("magician out vision");
        guard.wandering.Excute();
    }    
}
