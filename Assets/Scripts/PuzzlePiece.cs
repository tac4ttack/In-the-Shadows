using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    [HideInInspector] public      bool            isPuzzlePieceValidated;

    public      Quaternion[]    OrientationSolutions;
    public      GameObject      RelativePuzzlePiece;
    public      Vector3         RelativeDirectionSolution;
    public      float           RelativeDistanceSolution;
    public      bool            CheckMirroredRelative;

    public      bool[]          RotationConstraints = new bool[3];
    public      bool[]          TranslationConstraints = new bool[3];

    private     float           _rotationSpeed = 42f;
    private     float           _translationSpeed = 10.5f;
    private     float           _orientationBias = 0.035f;
    private     float           _directionBias = 0.035f;
    private     float           _distanceBias = 0.035f;
    private     bool            _isOrientationOK;
    private     bool            _isRelativePositionOK;
    private     Quaternion      _currentOrientation;
    private     Vector3         _currentPosition;
    private     Puzzle          _puzzleContainer;

    void Start()
    {
        _puzzleContainer = this.GetComponentInParent<Puzzle>();
        isPuzzlePieceValidated = false;
        _isOrientationOK = false;
        _isRelativePositionOK = false;
        RotationConstraints[2] = true;
        TranslationConstraints[2] = true;
    }

    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.gameObject.name);
            Debug.Log(this.gameObject.transform.localRotation);
            Debug.Log(this.gameObject.transform.rotation);

            
            // Debug.Log(RelativePuzzlePiece.transform.position - this.gameObject.transform.position);
            // Debug.Log(Vector3.Magnitude(RelativePuzzlePiece.transform.position - this.gameObject.transform.position));            
            // Debug.Log(this.gameObject.transform.localRotation);
            // Debug.Log("\n");
        }
    }

    void FixedUpdate()
    {
        _currentOrientation = this.gameObject.transform.localRotation;
        _currentPosition = this.gameObject.transform.localPosition;

        CheckSolutions();
        isPuzzlePieceValidated = (_isOrientationOK && _isRelativePositionOK);
    }

    void OnMouseDrag()
    {
        if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetMouseButton(1))
        {
            if (!RotationConstraints[0])
            {
                this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _rotationSpeed * -1);
                // this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0), Time.deltaTime * _rotationSpeed);
                // this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _rotationSpeed, Space.Self);

            }
            if (!RotationConstraints[1])
            {
                this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0) * Time.deltaTime * _rotationSpeed);
                // this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0), Time.deltaTime * _rotationSpeed);
                // this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0), Time.deltaTime * _rotationSpeed, Space.Self);
            }
        }

        if (Input.GetMouseButton(1) || (!Input.GetMouseButton(1) && Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
        {
            if (!TranslationConstraints[0])
            {
                this.gameObject.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), 0, 0) * Time.deltaTime * _translationSpeed * -1, Space.World);
            }
            if (!TranslationConstraints[1])
            {
                this.gameObject.transform.Translate(new Vector3(0, Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * _translationSpeed, Space.World);
            }
        }
    }

    private void    CheckSolutions()
    {
        CheckOrientation();
        _isRelativePositionOK = CheckRelativePosition();
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
        if (OrientationSolutions.Length > 0)
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
                    _isOrientationOK = true;
                    break;
                }
                else
                {
                    _isOrientationOK = false;
                }
            }        
        }
        else
        {
            _isOrientationOK = true;
        }
    }

    /*
        Will check if the puzzle piece is correctly positionned compared to its relative child puzzle piece.
        Will only works if puzzle pieces are at the same depth (same Z coordonate)!
        It's better to set up a circular relationship like:
            A -> B -> C -> D -> A
        
        Will verify if the relative direction AND the relative distance are the planned ones.
    */
    
    //  DEBUG -> Need to test if the mirror check is good!
    //  DEBUG -> Need to adjust the BIASes!

    private bool    CheckRelativePosition()
    {
        if (!RelativePuzzlePiece)
        {
            return true;
        }
        else
        {
            Vector3 currentRelativeDirection = RelativePuzzlePiece.transform.position - this.gameObject.transform.position;
            float currentRelativeDistance = currentRelativeDirection.magnitude;
            
            if (    (RelativeDistanceSolution >= currentRelativeDistance - _distanceBias && RelativeDistanceSolution <= currentRelativeDistance + _distanceBias)
                &&  (RelativeDirectionSolution.x >= currentRelativeDirection.x - _directionBias && RelativeDirectionSolution.x <= currentRelativeDirection.x + _directionBias)
                &&  (RelativeDirectionSolution.y >= currentRelativeDirection.y - _directionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _directionBias)
                &&  (RelativeDirectionSolution.z >= currentRelativeDirection.z - _directionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _directionBias))
            {
                return true;
            }
            if (    CheckMirroredRelative
                &&  (RelativeDistanceSolution >= currentRelativeDistance - _distanceBias && RelativeDistanceSolution <= currentRelativeDistance + _distanceBias)
                &&  (-RelativeDirectionSolution.x >= currentRelativeDirection.x - _directionBias && -RelativeDirectionSolution.x <= currentRelativeDirection.x + _directionBias)
                &&  (RelativeDirectionSolution.y >= currentRelativeDirection.y - _directionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _directionBias)
                &&  (RelativeDirectionSolution.z >= currentRelativeDirection.z - _directionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _directionBias))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /*
    // OBSOLETE!!!!
    private void    CheckPosition()
    {
        for (int i = 0; i < PositionSolutions.Length; i++)
        {
            Vector3 p = (Vector3)PositionSolutions[i];
            if (    (p.x >=  _currentPosition.x - _positionBias && p.x <=  _currentPosition.x + _positionBias)
                &&  (p.y >=  _currentPosition.y - _positionBias && p.y <=  _currentPosition.y + _positionBias)
                &&  (p.z >=  _currentPosition.z - _positionBias && p.z <=  _currentPosition.z + _positionBias))
            {
                _isRelativePositionOK = true;
                break;
            }
            else
            {
                _isRelativePositionOK = false;
            }
        }
    }
    */
}
