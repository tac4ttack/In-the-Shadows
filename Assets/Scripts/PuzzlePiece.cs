using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [HideInInspector] public      bool            validated;

    public      Quaternion[]    OrientationSolutions;
    public      Vector3[]       PositionSolutions;

    public      bool[]          RotationConstraints = new bool[3];
    public      bool[]          TranslationConstraints = new bool[3];

    private     float           _rotationSpeed = 42f;
    private     float           _translationSpeed = 10.5f;
    private     float           _orientationBias = 0.035f;
    private     float           _positionBias = 0.035f;
    private     bool            _isQuatOK;
    private     bool            _isPosOK;
    private     Quaternion      _currentOrientation;
    private     Vector3         _currentPosition;

    void Start()
    {
        validated = false;
        _isQuatOK = false;
        _isPosOK = false;
        RotationConstraints[2] = true;
        TranslationConstraints[2] = true;
    }

    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.gameObject.transform.localRotation);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            if (!RotationConstraints[0])
                this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _rotationSpeed * -1);
            if (!RotationConstraints[1])
                this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0) * Time.deltaTime * _rotationSpeed * -1);
        }

        if (Input.GetMouseButton(1) || (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
        {
            if (!TranslationConstraints[0])
                this.gameObject.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), 0, 0) * Time.deltaTime * _translationSpeed * -1, Space.World);
            if (!TranslationConstraints[1])
                this.gameObject.transform.Translate(new Vector3(0, Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * _translationSpeed, Space.World);
        }

        _currentOrientation = this.gameObject.transform.localRotation;
        _currentPosition = this.gameObject.transform.localPosition;

        CheckSolutions();
    }

    private void    CheckSolutions()
    {
        CheckOrientation();
        CheckPosition();

        if (_isQuatOK && _isPosOK)
            validated = true;
        else
            validated = false;
    }

    /*
        Will check if the puzzle piece is correctly oriented.
        Quaternions are logically equal when:
            Q1 == Q2
            or
            Q1 == -Q2
     */
    private void    CheckOrientation()
    {
        for (int i = 0; i < OrientationSolutions.Length; i++)
        {
            Quaternion q = (Quaternion)OrientationSolutions[i];
            if (    ((q.x >=  _currentOrientation.x - _orientationBias && q.x <=  _currentOrientation.x + _orientationBias)
                &&   (q.y >=  _currentOrientation.y - _orientationBias && q.y <=  _currentOrientation.y + _orientationBias)
                &&   (q.z >=  _currentOrientation.z - _orientationBias && q.z <=  _currentOrientation.z + _orientationBias)
                &&   (q.w >=  _currentOrientation.w - _orientationBias && q.w <=  _currentOrientation.w + _orientationBias))
                ||
                    ((q.x >= -_currentOrientation.x - _orientationBias && q.x <= -_currentOrientation.x + _orientationBias)
                &&   (q.y >= -_currentOrientation.y - _orientationBias && q.y <= -_currentOrientation.y + _orientationBias)
                &&   (q.z >= -_currentOrientation.z - _orientationBias && q.z <= -_currentOrientation.z + _orientationBias)
                &&   (q.w >= -_currentOrientation.w - _orientationBias && q.w <= -_currentOrientation.w + _orientationBias)))
            {
                _isQuatOK = true;
                break;
            }
            else
            {
                _isQuatOK = false;
            }
        }
    }

    private void    CheckPosition()
    {
        for (int i = 0; i < PositionSolutions.Length; i++)
        {
            Vector3 p = (Vector3)PositionSolutions[i];
            if (    (p.x >=  _currentPosition.x - _positionBias && p.x <=  _currentPosition.x + _positionBias)
                &&  (p.y >=  _currentPosition.y - _positionBias && p.y <=  _currentPosition.y + _positionBias)
                &&  (p.z >=  _currentPosition.z - _positionBias && p.z <=  _currentPosition.z + _positionBias))
            {
                _isPosOK = true;
                break;
            }
            else
            {
                _isPosOK = false;
            }
        }
    }
}
