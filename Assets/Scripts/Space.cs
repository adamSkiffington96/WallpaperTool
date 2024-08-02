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

        /*
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/

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
        _objectParent.localPosition = Vector3.zero;

        foreach (Transform obj in _objectParent)
        {
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

            //float rangeX = Random.Range(-_angleRange.x, _angleRange.x);
            //float rangeY = Random.Range(-_angleRange.y, _angleRange.y);

            float rangeX = Random.Range(-360, 360);
            float rangeY = Random.Range(-360, 360);
            float rangeZ = Random.Range(-360, 360);

            //obj.localPosition = new Vector3(rangeX, rangeY, 0f);

            //Vector3 randomVect = new Vector3(rangeX, rangeY, rangeZ);
            //Vector3 randomVect = Random.insideUnitSphere.normalized;

            //obj.position = _camera.transform.position + randomVect.normalized;


            obj.RotateAround(_camera.transform.position, Vector3.up, rangeX);
            obj.RotateAround(_camera.transform.position, Vector3.right, rangeY);

            
            Vector3 offsetDistance = (obj.position - _camera.transform.position).normalized;
            obj.position = _camera.transform.position + (offsetDistance * _starDistance);

            obj.position += new Vector3(0, 0, 100f);


            Quaternion lookRotation = Quaternion.LookRotation(-offsetDistance);
            obj.rotation = lookRotation;

            obj.GetComponent<Star>().SpawnPointInit(obj.position);

            /*
            Vector3 adjustZRotation = obj.eulerAngles;
            adjustZRotation.z = 45f;
            obj.eulerAngles = adjustZRotation;*/
        }

        //_objectParent.localPosition = Vector3.zero;
        _objectParent.eulerAngles = Vector3.zero;
        
        _objectParent.position = _camera.transform.position;

        _probe.RenderProbe();
    }

    private void ShootingStars()
    {
        if(_currentShootingStarFrequency <= 0) {

            if (_trailRenderer.emitting)
                _trailRenderer.emitting = false;

            //randomDirection = ((_camera.transform.right.normalized * Random.Range(-1f, 1f)) + (_camera.transform.up.normalized * Random.Range(-1f, 1f))).normalized;
            randomDirection = ((_camera.transform.right.normalized * _shootingStarDir.x) + (_camera.transform.up.normalized * _shootingStarDir.y)).normalized;
            randomDirection.z = 0;

            Vector3 offset = new Vector3(_camera.transform.position.x + Random.Range(-5f, 5f), _camera.transform.position.y + Random.Range(-5f, 5f), _camera.transform.position.z) - (randomDirection * 20f);
            offset.z = 10f;
            _shootingStar.localPosition = offset;

            _currentShootingStarFrequency = _createShootingStarFrequency;
        }
        else {
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
        if (Input.GetKeyDown(KeyCode.R)) {
            _movementInput = Vector3.zero;
            _camRotation.x = 0f;
        }

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

        /*
        if (_yaw < 0f)
            _yaw = 360f;
        if(_yaw > 360f)
            _yaw = 0f;*/

        _camRotation = new Vector3(_pitch, _yaw, 0f);

        _debugText.text = "Pitch: " + _pitch + ", Yaw: " + _yaw + "\nInput Y: " + _movementInput.y;

        _camera.transform.eulerAngles = _camRotation;
    }
}
