using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCamera2D : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;

    public Transform target;
    public Vector3 targetOffset = new Vector3(-0.75f, 0.0f, 4.75f);
    public float smoothTime = 0.5f;
    public Vector2 travelDistance = new Vector2(0.75f, 5.75f);
    public bool disableChase = false;
    public float chaseSpeed = 20.0f;

    void Start()
    {
        if (target)
        {
            transform.position = target.position + targetOffset;
        }
    }

    void Update()
    {
        if (target)
        {
            Vector3 targetPosition = transform.position;

            // X Movement
            targetPosition.x = target.position.x + targetOffset.x;
            targetPosition.x = Mathf.Clamp(targetPosition.x, travelDistance.x, travelDistance.y);

            // Z Movement
            if (target.position.z - (transform.position.z - targetOffset.z) > 0.25f)
            {
                targetPosition.z = target.position.z + targetOffset.z;
            }
            else
            {
                if (!disableChase)
                    targetPosition.z += chaseSpeed * Time.deltaTime;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
