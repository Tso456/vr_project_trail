using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer = null;
    public SplineContainer SplineContainer
    {
        get
        {
            if (this.splineContainer == null)
                this.splineContainer = this.GetComponent<SplineContainer>();
            return this.splineContainer;
        }
    }

    public Spline Spline
    {
        get
        {
            if (this.SplineContainer == null)
                return null;
            return this.SplineContainer.Spline;
        }
    }

    [Header("Behaviour - Speed-based")]
    public bool useSpeed = true;
    public float speed = 1.0f;

    [Header("Behaviour - Time-based")]
    public float timeToReachEnd = 0.0f; // Time in seconds to reach the end of the spline, if greater than 0 speed is ignored

    [Header("Behaviour - Smoothing")]
    public bool loop = true;
    public bool pingPong = false;
    public bool easeInAndOut = false;  // Ease in and out functionality

    [Header("Progress")]
    [SerializeField] private float progress = 0.0f;
    [SerializeField] private bool isForward = true;

    private void Start()
    {
        //// Optional, uncomment this or apply it another way so that time is used to determine the speed we need to travel at.
        //if (timeToReachEnd > 0)
        //{
        //    speed = Spline.GetLength() / timeToReachEnd;
        //}
    }

    void Update()
    {
        if (Spline != null)
        {
            // Velocity
            float velocity = Time.deltaTime * speed;

            // Motion direction
            float motionDirection = (isForward == true ? 1f : -1f);

            // Determine progress here
            if (useSpeed == true || this.timeToReachEnd < 0f)
            {
                progress += (motionDirection * (velocity / Spline.GetLength()));
            }
            else 
            {
                progress += (motionDirection * (Time.deltaTime / this.timeToReachEnd));
            }

            // Handle looping & pingpong
            if (progress > 1.0f || progress < 0.0f)
            {
                if (loop)
                {
                    progress = Mathf.Repeat(progress, 1.0f);
                }
                else if (pingPong)
                {
                    isForward = !isForward;
                    progress = Mathf.Clamp01(progress);
                }
                else
                {
                    progress = Mathf.Clamp01(progress);
                    enabled = false; // stop moving if not looping or ping-ponging
                }
            }

            // If easeInAndOut is true, use the Smoothstep function to ease the motion
            float easedProgress = easeInAndOut ? Smoothstep(progress) : progress;

            // Calculate position
            Vector3 estimationPosition = this.SplineContainer.EvaluatePosition(easedProgress);

            // Apply position
            transform.position = Vector3.Lerp(transform.position, estimationPosition, Time.deltaTime * speed);

            //Vector3 Dir = new Vector3(velocity.x, velocity.y, velocity.z).normalized;
            Vector3 RightDir = Vector3.up;//new Vector3().Cross(Vector3.up, velocity);
                
            float dist = velocity * Time.deltaTime;
            float alpha = (dist * 180.0f) / (Mathf.PI * 0.37f);
            transform.Rotate(RightDir, alpha, Space.World);
        }
    }

    // The Smoothstep function for easing
    float Smoothstep(float x)
    {
        return x * x * (3 - 2 * x);
    }
}