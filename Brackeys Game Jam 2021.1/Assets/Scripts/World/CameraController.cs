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

    public Vector3 cameraOffset;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
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
        if (Mathf.Abs(targetPosition.x) >= cameraConstraints.x){
            targetPosition = new Vector3(cameraConstraints.x * Mathf.Sign(transform.position.x), targetPosition.y);
        }

        if (Mathf.Abs(targetPosition.y) >= cameraConstraints.y){
            targetPosition = new Vector3(targetPosition.x, cameraConstraints.y * Mathf.Sign(transform.position.y));
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
    }
}
