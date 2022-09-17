using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;

    Vector3 offset;

    void Start()
    {
        //create offset for camera to follow player
        player = GameObject.FindGameObjectWithTag("Player");
        offset = player.transform.position - transform.position;
    }

    void Update()
    {
        transform.position = player.transform.position - offset;
    }
}
