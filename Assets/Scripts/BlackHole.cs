using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public List<Transform> CapturedStars = new List<Transform>();

    public List<Transform> ReleasedStars = new List<Transform>();

    public float gravityForce = 1f;
    public float releaseForce = 1f;

    public float starLeaveDistance = 50f;

    public float starRevolveSpeed = 4f;


    private void Start()
    {
        transform.position = Camera.main.transform.position + (Camera.main.transform.forward.normalized * 3000f);
    }

    private void Update()
    {
        //GravityWell();

        //ReleaseStars();
    }

    private void FixedUpdate()
    {
        GravityWell();

        ReleaseStars();
    }

    private void GravityWell()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3 ratio = new Vector3(Mathf.Clamp((mousePos.x - 1280) / 1280, -1, 1), Mathf.Clamp((mousePos.y - 720) / 720, -1, 1), 0f);

        Vector3 myPosition = new Vector3(ratio.x * 3000, ratio.y * 1700, 3000f);

        Vector3 adjustedLength = (Camera.main.transform.position - myPosition).normalized * 3000f;

        transform.localPosition = -adjustedLength;

        print(ratio);
    }

    private void ReleaseStars()
    {
        for (int i = 0; i < CapturedStars.Count; i++)
        {
            float wellDist = 250f / (CapturedStars[i].position - transform.position).magnitude;

            CapturedStars[i].position = Vector3.Lerp(CapturedStars[i].position, transform.position, gravityForce * Time.fixedDeltaTime * wellDist);
            CapturedStars[i].RotateAround(transform.position, transform.forward, starRevolveSpeed * Time.fixedDeltaTime * wellDist);
        }

        for (int i = 0; i < ReleasedStars.Count; i++)
        {
            Vector3 spawn = ReleasedStars[i].GetComponent<Star>().OriginPoint();

            if(spawn != Vector3.zero)
            {
                if((spawn - ReleasedStars[i].position).magnitude > 0.5f)
                {
                    ReleasedStars[i].position = Vector3.MoveTowards(ReleasedStars[i].position, spawn, releaseForce * Time.deltaTime);
                }
                else
                {
                    ReleasedStars[i].position = spawn;

                    ReleasedStars.Remove(ReleasedStars[i]);
                }
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        //other.transform.position = Vector3.Lerp(other.transform.position, transform.position, gravityForce * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            if (ReleasedStars.Contains(other.transform))
                ReleasedStars.Remove(other.transform);

            CapturedStars.Add(other.transform);
        }
    }

    private void RemoveStar(Transform target)
    {
        ReleasedStars.Add(target);

        CapturedStars.Remove(target);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            RemoveStar(other.transform);

            //OriginPositions.Remove(other.transform.position);
        }
    }
}
