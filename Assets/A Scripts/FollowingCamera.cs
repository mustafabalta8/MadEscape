using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 cameraDistance;
    

    private void Update()
    {
        transform.position = target.transform.position + cameraDistance;
    }
}
