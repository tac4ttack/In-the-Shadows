using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [HideInInspector] public      bool            validated;
    public      Quaternion[]    solQua;
    public      Vector3[]       solPos;

    private     float           speed = 21f;
    private     float           quaBias = 0.035f;
    private     float           posBias = 0.035f;
    private     bool            quaOK;
    private     bool            posOK;
    private     Quaternion      currentQua;
    private     Vector3         currentPos;

    void Start()
    {
        validated = false;
        quaOK = false;
        posOK = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.gameObject.transform.localRotation);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
            this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
        
        if (Input.GetKey(KeyCode.I))
            this.gameObject.transform.Rotate(new Vector3(1, 0, 0), 0.5f);

        if (Input.GetKey(KeyCode.K))
            this.gameObject.transform.Rotate(new Vector3(1, 0, 0), -0.5f);

        if (Input.GetKey(KeyCode.J))
            this.gameObject.transform.Rotate(new Vector3(0, 1, 0), 0.5f);

        if (Input.GetKey(KeyCode.L))
            this.gameObject.transform.Rotate(new Vector3(0, 1, 0), -0.5f);

        if (Input.GetKey(KeyCode.U))
            this.gameObject.transform.Rotate(new Vector3(0, 0, 1), 0.5f);

        if (Input.GetKey(KeyCode.O))
            this.gameObject.transform.Rotate(new Vector3(0, 0, 1), -0.5f);


        currentQua = this.gameObject.transform.localRotation;
        currentPos = this.gameObject.transform.localPosition;

        CheckSolutions();
    }

    /*
        Will check if the puzzle piece is correctly rotated and translated.
        Quaternions are logically equal when:
            Q1 == Q2
            or
            Q1 == -Q2
     */
    private void    CheckSolutions()
    {
        for (int i = 0; i < solQua.Length; i++)
        {
            Quaternion q = (Quaternion)solQua[i];
            if (    ((q.x >=  currentQua.x - quaBias && q.x <=  currentQua.x + quaBias)
                &&   (q.y >=  currentQua.y - quaBias && q.y <=  currentQua.y + quaBias)
                &&   (q.z >=  currentQua.z - quaBias && q.z <=  currentQua.z + quaBias)
                &&   (q.w >=  currentQua.w - quaBias && q.w <=  currentQua.w + quaBias))
                ||
                    ((q.x >= -currentQua.x - quaBias && q.x <= -currentQua.x + quaBias)
                &&   (q.y >= -currentQua.y - quaBias && q.y <= -currentQua.y + quaBias)
                &&   (q.z >= -currentQua.z - quaBias && q.z <= -currentQua.z + quaBias)
                &&   (q.w >= -currentQua.w - quaBias && q.w <= -currentQua.w + quaBias)))
            {
                quaOK = true;
                break;
            }
            else
            {
                quaOK = false;
            }
        }

        for (int i = 0; i < solPos.Length; i++)
        {
            Vector3 p = (Vector3)solPos[i];
            if (    (p.x >=  currentPos.x - posBias && p.x <=  currentPos.x + posBias)
                &&  (p.y >=  currentPos.y - posBias && p.y <=  currentPos.y + posBias)
                &&  (p.z >=  currentPos.z - posBias && p.z <=  currentPos.z + posBias))
            {
                posOK = true;
                break;
            }
            else
            {
                posOK = false;
            }
        }

        if (quaOK && posOK)
            validated = true;
        else
            validated = false;
    }
}
