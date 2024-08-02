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

        //Hover(isInside);
    }


    private void CheckBounds()
    {
        for(int i = 0; i < AsteroidParent.childCount; i++)
        {
            Vector2 target = Camera.main.WorldToScreenPoint(AsteroidParent.GetChild(i).position);

            //bool isInside = false;

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
                //isInside = true;
                //AsteroidSystem.RespawnAsteroid(i, true);
                AsteroidParent.GetChild(i).GetComponent<ExplodeAsteroid>().Explode();
            }
        }

        
    }

    private void Hover(bool state)
    {
        if(state == true)
        {
            if(alienMat0.GetColor("_EmissionColor") != Color.red)
            {
                alienMat0.SetColor("_EmissionColor", Color.red);
                alienMat1.SetColor("_EmissionColor", Color.red);
                alienMat2.SetColor("_EmissionColor", Color.red);
            }
        }
        else
        {
            if (alienMat0.GetColor("_EmissionColor") != Color.white)
            {
                alienMat0.SetColor("_EmissionColor", Color.white);
                alienMat1.SetColor("_EmissionColor", Color.white);
                alienMat2.SetColor("_EmissionColor", Color.white);
            }
        }
    }


}
