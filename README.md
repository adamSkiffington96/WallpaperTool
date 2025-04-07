
# Desktop Space Simulator

A space sim you can run as a desktop background. Use the windows marquee tool to blast apart asteroids!

![App Screenshot](https://i.imgur.com/MQMl44B.jpeg)
![App Screenshot](https://i.imgur.com/8itZUVd.png)
![App Screenshot](https://i.imgur.com/sY9lrTR.png)


## Features

- Stars are attracted to your mouse
- Break asteroids by highlighting them
- Shooting stars
- Randomized star color and size
- Nebula


## Snippets

<details>
<summary><code>Generate Positioning</code></summary>

```
private void RegeneratePositions()
    {
        // This essentially creates a random sphere of stars situated around the camera

        _objectParent.localPosition = Vector3.zero;
    
        // For every star
    
        foreach (Transform obj in _objectParent)
        {
            // Randomize scale (within range)
    
            float randomScaleVal = Random.Range(_scaleRange.x, _scaleRange.y);
    
            if(randomScaleVal > (_scaleRange.y / 2f))
            {
                float largeStarChance = Random.Range(0f, 100f);
                if(largeStarChance > 33f)
                {
                    randomScaleVal = Random.Range(_scaleRange.x, _scaleRange.y / 2f);
                }
            }
    
            obj.localScale = new Vector3(randomScaleVal, randomScaleVal, randomScaleVal);
    
            float rangeX = Random.Range(-360, 360);
            float rangeY = Random.Range(-360, 360);
            float rangeZ = Random.Range(-360, 360);
    
            // Rotate star randomly around the origin (camera position)
    
            obj.RotateAround(_camera.transform.position, Vector3.up, rangeX);
            obj.RotateAround(_camera.transform.position, Vector3.right, rangeY);
    
            // Reset distance and orientation from camera
            
            Vector3 offsetDistance = (obj.position - _camera.transform.position).normalized;
            obj.position = _camera.transform.position + (offsetDistance * _starDistance);
    
            obj.position += new Vector3(0, 0, 100f);
    
            Quaternion lookRotation = Quaternion.LookRotation(-offsetDistance);
            obj.rotation = lookRotation;
    
            // Send the star's new position data to its script
    
            obj.GetComponent<Star>().SpawnPointInit(obj.position);
        }
    
        _objectParent.eulerAngles = Vector3.zero;
        
        _objectParent.position = _camera.transform.position;
    
        _probe.RenderProbe();
    }

```
</details>

<details>
<summary><code>Set Asteroid Positions</code></summary>

```
public void SetAsteroidPosition(int index, bool createNew)
    {
        //  Upon respawn, set out asteroid's new position and trajectory
        //      - store this asteroids trajectory, spawn position, and a randomized speed in lists

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
```
</details>

<details>
<summary><code>BlackHole</code></summary>

```
private void GravityWell()
    {
        // Move our black hole depending on the mouse position

        Vector3 mousePos = Input.mousePosition;

        float ratioX = ((mousePos.x - (Screen.width / 2)) * 2) / Screen.width;
        float ratioY = ((mousePos.y - (Screen.height / 2)) * 2) / Screen.height;

        print("Ratio pos: " + ratioX + ", " + ratioY + "\n Input pos: " + Input.mousePosition + "\n ");


        Vector3 myPosition = new Vector3(ratioX * 3000, ratioY * 1700, 3000f);


        Vector3 adjustedLength = (Camera.main.transform.position - myPosition).normalized * 3000f;

        transform.localPosition = -adjustedLength;
    }

```
</details>

<details>
<summary><code>SelectorGame.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectorGame : MonoBehaviour
{
    public GameObject UICorner0;
    public GameObject UICorner1;

    public Vector2 cornerPosition0 = Vector2.zero;
    public Vector2 cornerPosition1 = Vector2.zero;

    public GameObject alienParent;

    public GameObject AlienObject0;
    public GameObject AlienObject1;
    public GameObject AlienObject2;

    private Material alienMat0;
    private Material alienMat1;
    private Material alienMat2;
    //public Material AlienMaterial;

    public Transform AsteroidParent;

    private GenerateAsteroids AsteroidSystem;

    public bool showDebugCircles = false;


    private void Start()
    {
        AsteroidSystem = GetComponent<GenerateAsteroids>();

        alienMat0 = AlienObject0.GetComponent<Renderer>().material;
        alienMat1 = AlienObject1.GetComponent<Renderer>().material;
        alienMat2 = AlienObject2.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if(Input.GetMouseButtonDown(0)) {
            cornerPosition0 = new Vector2(mousePos.x, mousePos.y);

            if(showDebugCircles ) {
                UICorner0.transform.position = cornerPosition0;
            }
        }
        if(Input.GetMouseButton(0)) {
            cornerPosition1 = new Vector2(mousePos.x, mousePos.y);

            if(showDebugCircles) {
                UICorner1.transform.position = cornerPosition1;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            cornerPosition1 = new Vector2(mousePos.x, mousePos.y);

            CheckBounds();
        }
    }


    private void CheckBounds()
    {
        // Check if any asteroid is within our marquee bounds, and explode it if so

        for(int i = 0; i < AsteroidParent.childCount; i++)
        {
            Vector2 target = Camera.main.WorldToScreenPoint(AsteroidParent.GetChild(i).position);

            bool insideX = false;
            bool insideY = false;

            if (cornerPosition1.x < cornerPosition0.x) {
                if (target.x < cornerPosition0.x && target.x > cornerPosition1.x)
                    insideX = true;
            }
            else {
                if (target.x > cornerPosition0.x && target.x < cornerPosition1.x)
                    insideX = true;
            }

            if (cornerPosition1.y < cornerPosition0.y) {
                if (target.y < cornerPosition0.y && target.y > cornerPosition1.y)
                    insideY = true;
            }
            else {
                if (target.y > cornerPosition0.y && target.y < cornerPosition1.y)
                    insideY = true;
            }

            if (insideX == true && insideY == true) {
                AsteroidParent.GetChild(i).GetComponent<ExplodeAsteroid>().Explode();
            }
        }
    }
}

```
</details>

## Optimizations

- Stars are release once they travel too far away from their origin
- Code is generally optimized to be able to run on a persons desktop without becoming resource-intensive too quick





