﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
    public  GameObject  toto;
    public  Light       spot;
    public  float       bias;
    public Quaternion[] solutions;

    // Start is called before the first frame update
    void Start()
    {
        spot.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.I))
            toto.gameObject.transform.Rotate(new Vector3(1, 0, 0), 0.5f);

        if (Input.GetKey(KeyCode.K))
            toto.gameObject.transform.Rotate(new Vector3(1, 0, 0), -0.5f);

        if (Input.GetKey(KeyCode.J))
            toto.gameObject.transform.Rotate(new Vector3(0, 1, 0), 0.5f);

        if (Input.GetKey(KeyCode.L))
            toto.gameObject.transform.Rotate(new Vector3(0, 1, 0), -0.5f);

        if (Input.GetKey(KeyCode.U))
            toto.gameObject.transform.Rotate(new Vector3(0, 0, 1), 0.5f);

        if (Input.GetKey(KeyCode.O))
            toto.gameObject.transform.Rotate(new Vector3(0, 0, 1), -0.5f);

        if (Input.GetKey(KeyCode.Backspace))
            toto.gameObject.transform.localRotation = new Quaternion(0, 0 ,0 , 1);


        CheckSolution();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(toto.gameObject.transform.rotation.x);
            Debug.Log(toto.gameObject.transform.rotation.y);
            Debug.Log(toto.gameObject.transform.rotation.z);
            Debug.Log(toto.gameObject.transform.rotation.w);

        }
    }

    void    CheckSolution()
    {
        for (int i = 0; i < solutions.Length; i++)
        {
            Quaternion q = (Quaternion)solutions[i];
            if (    q.x >= toto.transform.rotation.x - bias
                &&  q.x <= toto.transform.rotation.x + bias
                &&  q.y >= toto.transform.rotation.y - bias
                &&  q.y <= toto.transform.rotation.y + bias
                &&  q.z >= toto.transform.rotation.z - bias
                &&  q.z <= toto.transform.rotation.z + bias
                &&  q.w >= toto.transform.rotation.w - bias
                &&  q.w <= toto.transform.rotation.w + bias)
            {
                spot.color = Color.green;
            }
            else
            {
                spot.color = Color.white;
            }
        }
    }
}
