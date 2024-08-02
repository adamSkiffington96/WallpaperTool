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
        explosionActive = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Trail[i].Clear();

            Trail[i].enabled = false;

            transform.GetChild(i).localPosition = piecesOriginList[i];

            transform.GetChild(i).localScale = Vector3.one;
        }

        //Controller.RespawnAsteroid(transform.GetSiblingIndex());
        Controller.SetAsteroidPosition(transform.GetSiblingIndex(), false);

        rotateAsteroidRange = Random.Range(-MaxAsteroidRotation, MaxAsteroidRotation);

        /*
        for (int i = 0; i < transform.childCount; i++)
            Trail[i].enabled = true;*/
    }


    private void MovePieces()
    {
        for(int i = 0; i < transform.childCount; i++) {

            Vector3 awayDir = (piecesOriginList[i] - Vector3.zero).normalized;

            transform.GetChild(i).localPosition += (piecesSpeedList[i] * Time.fixedDeltaTime * awayDir);

            transform.GetChild(i).RotateAround(transform.GetChild(i).position, Camera.main.transform.forward, rotateAsteroidRange * Time.fixedDeltaTime * modifyPieceRotation);

            transform.GetChild(i).localScale = Vector3.MoveTowards(transform.GetChild(i).localScale, Vector3.zero, shrinkPiecesSpeed * Time.fixedDeltaTime);

            Trail[i].time = Mathf.MoveTowards(Trail[i].time, 0f, shrinkTailSpeed * Time.fixedDeltaTime);
        }
    }


}
