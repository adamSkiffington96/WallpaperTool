using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAsteroids : MonoBehaviour
{
    public Transform AsteroidParent;

    private GameObject asteroidPrefab;

    public int asteroidPopulation = 4;

    public float respawnDistance = 120f;

    public int MinSpeed = 15;
    public int MaxSpeed = 40;
    private float mySpeed = 0f;

    private List<float> SpeedList = new List<float>();

    private List<Vector3> MovementList = new List<Vector3>();
    private List<Vector3> SpawnList = new List<Vector3>();

    public float offsetAsteroidTarget = 20f;


    private void Start()
    {
        SpeedList.Add(0f);
        SpawnList.Add(Vector3.zero);
        MovementList.Add(Vector3.zero);

        asteroidPrefab = AsteroidParent.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(asteroidPopulation <= 10)
                asteroidPopulation++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(asteroidPopulation > 1)
                asteroidPopulation--;
        }
    }

    private void FixedUpdate()
    {
        CheckAsteroids();
        MoveAsteroids();
    }

    private void CheckAsteroids()
    {
        if(asteroidPopulation <= 0)
            asteroidPopulation = 1;

        int children = AsteroidParent.childCount;

        if(children != asteroidPopulation)
        {
            if(AsteroidParent.childCount < asteroidPopulation )
            {
                GameObject body = Instantiate(asteroidPrefab, AsteroidParent);

                body.GetComponent<ExplodeAsteroid>().InitController(this);

                SetAsteroidPosition(body.transform.GetSiblingIndex(), true);
            }

            if (AsteroidParent.childCount > asteroidPopulation)
            {
                if(asteroidPopulation > 1)
                {
                    int index = AsteroidParent.childCount - 1;
                    DestructAsteroid(index);
                }
            }
        }
    }

    private void MoveAsteroids()
    {
        for (int i = 0; i < AsteroidParent.childCount; i++)
        {
            Transform current = AsteroidParent.GetChild(i);

            // Only apply movement when movement list matches the # of asteroids
            if (MovementList.Count == AsteroidParent.childCount)
                current.localPosition += (MovementList[i].normalized * SpeedList[i] * Time.fixedDeltaTime);

            if (SpawnList.Count == AsteroidParent.childCount)
            {
                Vector3 distanceTraveled = current.localPosition - SpawnList[i];
                if (distanceTraveled.magnitude > (respawnDistance * 2))
                {
                    print("distance reached");
                    SetAsteroidPosition(i, false);
                }
            }
        }
    }

    public void SetAsteroidPosition(int index, bool createNew)
    {
        Transform body = AsteroidParent.GetChild(index);

        Vector3 myDirection = respawnDistance * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
        myDirection.z = body.transform.localPosition.z;

        body.transform.localPosition = myDirection;

        Vector3 offsetOrigin = AsteroidParent.localPosition + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * offsetAsteroidTarget);
        offsetOrigin.z = body.transform.localPosition.z;

        Vector3 angle = offsetOrigin - body.transform.localPosition;

        if (!createNew) {
            MovementList[index] = angle;

            SpawnList[index] = body.transform.localPosition;

            SpeedList[index] = Random.Range(MinSpeed, MaxSpeed);
        }
        else {
            MovementList.Add(angle);

            SpawnList.Add(body.transform.localPosition);

            SpeedList.Add(Random.Range(MinSpeed, MaxSpeed));
        }
    }

    private void DestructAsteroid(int index)
    {
        GameObject toDestroy = AsteroidParent.GetChild(index).gameObject;

        SpeedList.Remove(SpeedList[SpeedList.Count - 1]);

        SpawnList.Remove(SpawnList[SpawnList.Count - 1]);

        MovementList.Remove(MovementList[MovementList.Count - 1]);

        Destroy(toDestroy);
    }

    

    
}
