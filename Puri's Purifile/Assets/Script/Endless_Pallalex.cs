
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Endless_Pallalex : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float pallalaxEffect;
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * pallalaxEffect; // 0 = move with camera, 1 = stay
        float movement = cam.transform.position.x * (1 - pallalaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}