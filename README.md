
# Desktop Space Simulator

A desktop space sim with an asteroid minigame and a field of stars for effect


## Screenshots

![App Screenshot](https://via.placeholder.com/468x300?text=App+Screenshot+Here)


## Features

- Light/dark mode toggle
- Live previews
- Fullscreen mode
- Cross platform


## Snippets

<details>
<summary><code>Space.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Space : MonoBehaviour
{
    public GameObject[] _prefabBodies;
    
    public Transform _objectParent;
    
    public Vector2 _angleRange;
    
    public Vector2 _scaleRange;
    
    public int totalBodyPopulation = 100;
    
    private Camera _camera;
    
    public float _panCameraSpeed = 100f;
    
    public float _smoothingCamSpeed = 4f;
    
    public float _starDistance = 10f;
    
    private Vector3 _movementInput;
    
    private Vector3 _camRotation;
    
    public Text _debugText;
    
    public ReflectionProbe _probe;
    
    public Vector3 _offsetObjectRotation;
    
    private float _changeColorPercent = 0.2f;
    
    public Color _blueColor;
    public Color _yellowColor;
    public Color _orangeColor;
    public Color _redColor;
    
    public float _colorVariation = 0.5f;
    
    public bool _autoRotateCam = false;
    public float _autoRotateSpeed = 1f;
    
    public Transform _shootingStar;
    
    public float _shootingStarSpeed = 1f;
    
    public float _createShootingStarFrequency = 3f;
    private float _currentShootingStarFrequency = 0f;
    
    public Vector2 _shootingStarDir;
    private Vector3 randomDirection;
    
    private TrailRenderer _trailRenderer;
    
    
    private void Start()
    {
        _trailRenderer = _shootingStar.GetComponent<TrailRenderer>();
    
        _angleRange *= 0.5f;
    
        _camera = Camera.main;
    
        SpawnObjects(totalBodyPopulation);
    
        AddColor();
    
        StartCoroutine(Countdown(0.5f));
    }
    
    IEnumerator Countdown(float time)
    {
        yield return new WaitForSeconds(time);
    
        RegeneratePositions();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegeneratePositions();
        }
    
        PanAround();
    
        ShootingStars();
    }
    
    private void AddColor()
    {
        int populationToChange = (int)(totalBodyPopulation * _changeColorPercent);
    
        int indexNum = totalBodyPopulation / populationToChange;
    
        int currentIndex = 0;
    
        for(int i = 0; i < populationToChange; i++) {
            Material myMat = _objectParent.GetChild(currentIndex).GetComponent<Renderer>().material;
    
            int randomStarColor = Random.Range(0, 4);
    
            float randomDiff = 1 + Random.Range(-_colorVariation, _colorVariation);
    
            Color c = Color.white;
    
            if (randomStarColor == 0)
                c = _blueColor * randomDiff;
            if (randomStarColor == 1)
                c = _yellowColor * randomDiff;
            if (randomStarColor == 2)
                c = _orangeColor * randomDiff;
            if (randomStarColor == 3)
                c = _redColor * randomDiff;
    
            c.a = 1;
            myMat.SetColor("_EmissionColor", c);
    
            currentIndex += indexNum;
        }
    }
    
    private void SpawnObjects(int population)
    {
        float objectsPerCategory = population / _prefabBodies.Length;
    
        foreach(GameObject body in _prefabBodies)
        {
            body.SetActive(true);
    
            for (int j = 0; j < objectsPerCategory; j++)
            {
                GameObject obj = Instantiate(body, _objectParent);
            }
    
            body.SetActive(false);
        }
    
        RegeneratePositions();
    }
    
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
    
    private void ShootingStars()
    {
        // Continue counting down until we fire off a shooting star
    
        if(_currentShootingStarFrequency <= 0) {
    
            // Create and initialize a shooting star
    
            if (_trailRenderer.emitting)
                _trailRenderer.emitting = false;
    
            randomDirection = ((_camera.transform.right.normalized * _shootingStarDir.x) + (_camera.transform.up.normalized * _shootingStarDir.y)).normalized;
            randomDirection.z = 0;
    
            Vector3 offset = new Vector3(_camera.transform.position.x + Random.Range(-5f, 5f), _camera.transform.position.y + Random.Range(-5f, 5f), _camera.transform.position.z) - (randomDirection * 20f);
            offset.z = 10f;
            _shootingStar.localPosition = offset;
    
            _currentShootingStarFrequency = _createShootingStarFrequency;
        }
        else {
    
            // Move the shooting star until reaching edge of screen
            
            _currentShootingStarFrequency -= Time.deltaTime;
    
            if (_shootingStar.localPosition.magnitude < 75f) {
    
                Vector3 movement = _shootingStarSpeed * Time.smoothDeltaTime * randomDirection;
                movement.z = 0;
    
                if (!_trailRenderer.emitting)
                    _trailRenderer.emitting = true;
    
                _shootingStar.localPosition += movement;
            }
            else {
                if (_trailRenderer.emitting)
                    _trailRenderer.emitting = false;
            }
        }
    }
    
    private void PanAround()
    {
        // Rotate camera horizontally
    
        Vector3 input = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
        _movementInput = Vector3.Lerp(_movementInput, input, _smoothingCamSpeed * Time.smoothDeltaTime);
    
        float _pitch = 0f;
        float _yaw;
        if (_autoRotateCam)
        {
            _yaw = _camRotation.y + (_autoRotateSpeed * Time.smoothDeltaTime * 10f);
        }
        else
        {
            _yaw = _camRotation.y + (_movementInput.x * _panCameraSpeed * Time.smoothDeltaTime * 10f);
            _pitch = Mathf.Clamp(_camRotation.x + (-_movementInput.y * _panCameraSpeed * Time.smoothDeltaTime * 10f), -90f, 45f);
        }
    
        _camRotation = new Vector3(_pitch, _yaw, 0f);
    
        _debugText.text = "Pitch: " + _pitch + ", Yaw: " + _yaw + "\nInput Y: " + _movementInput.y;
    
        _camera.transform.eulerAngles = _camRotation;
        }
    }

```
</details>

<details>
<summary><code>GenerateAsteroids.cs</code></summary>

```
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
        // Create asteroid speed list, spawnPosition list, and initial mov

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
        // Spawn initial asteroid count

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
        // Move our asteroids a specified distance, until respawning them in a new position

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

    private void DestructAsteroid(int index)
    {
        // Only used for testing, changing num of asteroids in runtime

        GameObject toDestroy = AsteroidParent.GetChild(index).gameObject;

        SpeedList.Remove(SpeedList[SpeedList.Count - 1]);

        SpawnList.Remove(SpawnList[SpawnList.Count - 1]);

        MovementList.Remove(MovementList[MovementList.Count - 1]);

        Destroy(toDestroy);
    }
}
```
</details>

</details>

<details>
<summary><code>ExplodeAsteroid.cs</code></summary>

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeAsteroid : MonoBehaviour
{
    private bool explosionActive = false;

    public float minimumForce = 1f;

    private float respawnTimer = 0f;

    private GenerateAsteroids Controller;

    private List<Vector3> piecesOriginList = new List<Vector3>();

    private List<float> piecesSpeedList = new List<float>();

    public float MaxAsteroidRotation = 1f;
    private float rotateAsteroidRange = 1f;

    public float modifyPieceRotation = 1f;

    public float MaxPieceFlyawaySpeed = 5f;

    private List<TrailRenderer> Trail = new List<TrailRenderer>();

    public float shrinkPiecesSpeed = 1f;
    public float shrinkTailSpeed = 1f;


    private void Start()
    {
        // Init

        rotateAsteroidRange = Random.Range(-MaxAsteroidRotation, MaxAsteroidRotation);

        for (int i = 0; i < transform.childCount; i++) {

            piecesOriginList.Add(transform.GetChild(i).localPosition);

            piecesSpeedList.Add(Random.Range(minimumForce, MaxPieceFlyawaySpeed));

            Trail.Add(transform.GetChild(i).GetComponent<TrailRenderer>());
        }
    }


    public void InitController(GenerateAsteroids cont)
    {
        Controller = cont;
    }


    public void Explode()
    {
        // Explosion triggered

        for (int i = 0; i < transform.childCount; i++)
        {
            Trail[i].enabled = true;

            Trail[i].Clear();

            Trail[i].time = 5f;
        }

        respawnTimer = 8f;

        explosionActive = true;
    }


    private void FixedUpdate()
    {
        // If we are exploding, move the pieces, otherwise rotate

        if(explosionActive) {
            if(respawnTimer > 0f) {
                MovePieces();

                respawnTimer -= Time.fixedDeltaTime;
            }
            else {
                Respawn();
            }
        }
        else{
            transform.RotateAround(transform.position, Camera.main.transform.forward, rotateAsteroidRange * Time.fixedDeltaTime);
        }
    }


    private void Respawn()
    {
        // Reset asteroid position and rotations values

        explosionActive = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Trail[i].Clear();

            Trail[i].enabled = false;

            transform.GetChild(i).localPosition = piecesOriginList[i];

            transform.GetChild(i).localScale = Vector3.one;
        }

        Controller.SetAsteroidPosition(transform.GetSiblingIndex(), false);

        rotateAsteroidRange = Random.Range(-MaxAsteroidRotation, MaxAsteroidRotation);
    }


    private void MovePieces()
    {
        // If we've exploded, move all our pieces in our last direction of travel

        for(int i = 0; i < transform.childCount; i++) {

            Vector3 awayDir = (piecesOriginList[i] - Vector3.zero).normalized;

            transform.GetChild(i).localPosition += (piecesSpeedList[i] * Time.fixedDeltaTime * awayDir);

            transform.GetChild(i).RotateAround(transform.GetChild(i).position, Camera.main.transform.forward, rotateAsteroidRange * Time.fixedDeltaTime * modifyPieceRotation);

            transform.GetChild(i).localScale = Vector3.MoveTowards(transform.GetChild(i).localScale, Vector3.zero, shrinkPiecesSpeed * Time.fixedDeltaTime);

            Trail[i].time = Mathf.MoveTowards(Trail[i].time, 0f, shrinkTailSpeed * Time.fixedDeltaTime);
        }
    }
}

```
</details>

</details>

<details>
<summary><code>BlackHole.cs</code></summary>

```
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BlackHole : MonoBehaviour
{
    public List<Transform> CapturedStars = new List<Transform>();

    public List<Transform> ReleasedStars = new List<Transform>();

    public float gravityForce = 1f;
    public float releaseForce = 1f;

    private float starLeaveDistance = 700f;

    public float starRevolveSpeed = 4f;

    private Vector3 lastPosition = Vector3.zero;


    private void Start()
    {
        transform.position = Camera.main.transform.position + (Camera.main.transform.forward.normalized * 3000f);
    }

    private void Update()
    {
        //GravityWell();

        //MoveStars();
    }

    private void FixedUpdate()
    {
        GravityWell();

        MoveStars();
    }

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

    private void MoveStars()
    {
        // Rotate captured stars towards center of our black hole

        for (int i = 0; i < CapturedStars.Count; i++)
        {
            float distFromOrigin = (CapturedStars[i].position - CapturedStars[i].GetComponent<Star>().OriginPoint()).magnitude;

            float wellDist = 250f / (CapturedStars[i].position - transform.position).magnitude;

            bool starRelease = false;

            if(wellDist > 5)
                starRelease = true;
            if (distFromOrigin >= starLeaveDistance)
                starRelease = true;

            if (starRelease)
            {
                //print("Star left @ dist: " + distFromOrigin);

                Transform holding = CapturedStars[i];

                CapturedStars.Remove(holding);

                ReleasedStars.Add(holding);
            }
            else
            {
                lastPosition = transform.position;

                CapturedStars[i].position = Vector3.Lerp(CapturedStars[i].position,
                    lastPosition, gravityForce * Time.fixedDeltaTime * wellDist);

                CapturedStars[i].RotateAround(lastPosition, transform.forward,
                        starRevolveSpeed * Time.fixedDeltaTime * wellDist);
            }
        }

        // Move released stars back to their spawn point

        for (int i = 0; i < ReleasedStars.Count; i++)
        {
            Vector3 spawn = ReleasedStars[i].GetComponent<Star>().OriginPoint();

            float distFromHome = (spawn - ReleasedStars[i].position).magnitude;

            if (distFromHome > 0.01f)
            {
                ReleasedStars[i].position = Vector3.MoveTowards(ReleasedStars[i].position, spawn, releaseForce * Time.fixedDeltaTime);
            }
            else
            {
                ReleasedStars[i].position = spawn;

                ReleasedStars.Remove(ReleasedStars[i]);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            CapturedStars.Add(other.transform);
        }
    }
}

```
</details>

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

What optimizations did you make in your code? E.g. refactors, performance improvements, accessibility


## Demo

Insert gif or link to demo

