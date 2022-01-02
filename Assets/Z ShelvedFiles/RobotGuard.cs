using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGuard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;

    Animator animator;
    [SerializeField] bool isWalking;
    [SerializeField] bool isTurning;


    

    [SerializeField] Transform pathHolder;

    Vector3[] waypoints;
    [SerializeField] float speed = 10f, waitTime = 1f, turnSpeed = 90f;

    [SerializeField] Light spotLight;
    [SerializeField] float viewDistance;
    [SerializeField] LayerMask viewMask;

    float viewAngle;
    Transform player;
    Color originalSpotLightColor;

    float playerVisibleTimer = 0;
    [SerializeField] float timeToSpotPlayer = 0.5f;
    private void Start()
    {
        animator = GetComponent<Animator>();

        viewAngle = spotLight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalSpotLightColor = spotLight.color;

        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).transform.position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(Patrol());
    }
    private void Update()
    {
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
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
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
            transform.position = Vector3.MoveTowards(transform.position, waypoints[n], speed * Time.deltaTime);//???baþka bir objeyi o noktaya götür ve tam o naktaya geldiðinde bu iþlemi baþlat
            //isWalking = true;
            //animator.SetBool("walk", true);
            if (transform.position == waypoints[n])
            {
                Debug.Log("asdas");
                n = (n + 1) % waypoints.Length;

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(waypoints[n]));
            }
            yield return null;
        }

    }
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        //animator.SetBool("walk", false);
        Vector3 directionToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;
        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
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
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }
        Gizmos.DrawLine(previousPos, startPos);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }


}

