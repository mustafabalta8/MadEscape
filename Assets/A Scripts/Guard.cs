using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;

    [SerializeField] private Transform pathHolder;
    private Vector3[] waypoints;

    [SerializeField] private float speed =7.5f;
    [SerializeField] private float waitTime = 0.6f;
    [SerializeField] private float turnSpeed = 160f;
    [SerializeField] private float viewDistance =10f;
    [SerializeField] private Light spotLight;
    [SerializeField] private LayerMask viewMask;
    [SerializeField] private float timeToSpotPlayer = 0.3f;


    private float viewAngle;
    private Color originalSpotLightColor;

    private float playerVisibleTimer =0;

    private void Start()
    {
        viewAngle = spotLight.spotAngle;
        originalSpotLightColor = spotLight.color;

        waypoints = new Vector3[pathHolder.childCount];
        for(int i=0;i<waypoints.Length;i++)
        {
            waypoints[i] = pathHolder.GetChild(i).transform.position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(Patrol());
    }
    private void Update()
    {
        SpotPlayer();
    }
    private void SpotPlayer()
    {
        if (Player.instance.isImmortal) { return; }
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer = 0;
        }
        spotLight.color = Color.Lerp(originalSpotLightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }
    }
    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, Player.instance.transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (Player.instance.transform.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, Player.instance.transform.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    IEnumerator Patrol()
    {
        transform.position = waypoints[0];
        int n = 1;
        transform.LookAt(waypoints[n]);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[n], speed * Time.deltaTime);            
            if (transform.position == waypoints[n]) {

                n = (n + 1) % waypoints.Length;

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(waypoints[n]));
            }
            yield return null;
        }              
       
    }
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle =  Mathf.Atan2(directionToLookTarget.x, directionToLookTarget.z) * Mathf.Rad2Deg;
        //float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle))  > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
    
    private void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPos = startPos;
        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }
        Gizmos.DrawLine(previousPos, startPos);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

    
}
