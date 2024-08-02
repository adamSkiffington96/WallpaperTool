using UnityEngine;
using System.Collections;

public class Wiggle : MonoBehaviour
{
    public float speed = 1;
    public float amplitude = 2;
    public int octaves = 4;

    private Vector3 destination;
    private int currentTime = 0;

    private Vector3 currentVel;

    private float startingDistance = 0;

    private void Start()
    {
        startingDistance = transform.localPosition.z;
    }

    private void FixedUpdate()
    {
        // if number of frames played since last change of direction > octaves create a new destination
        if (currentTime > octaves)
        {
            currentTime = 0;
            destination = generateRandomVector(amplitude);

            //print("new Vector Generated: " + destination);
        }

        // smoothly moves the object to the random destination

        Vector3 offsetDest = Vector3.SmoothDamp(transform.localPosition, destination, ref currentVel, speed);
        offsetDest.z = startingDistance;

        transform.localPosition = offsetDest;

        currentTime++;
    }

    // generates a random vector based on a single amplitude for x y and z
    private Vector3 generateRandomVector(float amp)
    {
        Vector3 result = new Vector3(Random.Range(-amp, amp), Random.Range(-amp, amp), startingDistance);

        return result;
    }
}