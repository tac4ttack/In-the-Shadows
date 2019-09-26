using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
    public  GameObject  toto;
    public  Light       spot;
    public  float       bias;
    public Quaternion[] solutions;
    private Quaternion current;

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
            Debug.Log(toto.gameObject.transform.localRotation);
            Debug.Log(current);
        }
    }

    /*
        Will check if the Solution quaternion is logically equal to the object quaternion
        Is true if:
            Q1 == Q2
            or
            Q1 == -Q2
     */
    void    CheckSolution()
    {
        current = Quaternion.Normalize(toto.transform.localRotation);
        
        for (int i = 0; i < solutions.Length; i++)
        {
            Quaternion q = (Quaternion)solutions[i];
            if (    ((q.x >= current.x - bias &&  q.x <= current.x + bias)
                &&  (q.y >= current.y - bias &&  q.y <= current.y + bias)
                &&  (q.z >= current.z - bias &&  q.z <= current.z + bias)
                &&  (q.w >= current.w - bias &&  q.w <= current.w + bias))
                ||
                    ((q.x >= -current.x - bias &&  q.x <= -current.x + bias)
                &&  (q.y >= -current.y - bias &&  q.y <= -current.y + bias)
                &&  (q.z >= -current.z - bias &&  q.z <= -current.z + bias)
                &&  (q.w >= -current.w - bias &&  q.w <= -current.w + bias)))
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
