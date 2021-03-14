using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject target;

    [Header("Camera Settings")]
    public float cameraSpeed;
    public Vector2 cameraConstraints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null){
            FollowTarget();
        }
    }

    void FollowTarget(){
        Vector3 targetPosition = target.transform.position;
        if (Mathf.Abs(transform.position.x) >= cameraConstraints.x){
            targetPosition = new Vector3(cameraConstraints.x * Mathf.Sign(transform.position.x), targetPosition.y);
        }

        if (Mathf.Abs(transform.position.y) >= cameraConstraints.y){
            targetPosition = new Vector3(targetPosition.x, cameraConstraints.y * Mathf.Sign(transform.position.y));
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
    }
}
