using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    Vector3 currentShipPosition = Vector3.zero;

    public float shipSpeed = 1f;


    void Start()
    {
        currentShipPosition.z = transform.localPosition.z;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, 2560f);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, 1440f);

        Vector2 ratioPos = new Vector2((mousePos.x - 1280f) / 1280f, (mousePos.y - 720f) / 720f);

        Vector3 shipPosition = new Vector3(Mathf.Clamp(ratioPos.x * 111.5f, -100f, 100f), Mathf.Clamp(ratioPos.y * 62f, -55f, 55f), transform.localPosition.z);

        currentShipPosition = Vector3.Lerp(currentShipPosition, shipPosition, shipSpeed * Time.smoothDeltaTime);

        transform.localPosition = currentShipPosition;

        //print("Mouse position: " + ratioPos);
    }
}
