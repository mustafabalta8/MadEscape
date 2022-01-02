using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test01 : MonoBehaviour
{    
    [SerializeField] float angleInDegrees;
    void Update()
    {
        Vector3 direction = new Vector3( Mathf.Cos((90-angleInDegrees) * Mathf.Deg2Rad), 0, Mathf.Sin( (90-angleInDegrees) * Mathf.Deg2Rad));
        Vector3 direction01 = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos( angleInDegrees * Mathf.Deg2Rad));
        Debug.DrawRay(transform.position, direction01 * 3, Color.green);
    }
}
