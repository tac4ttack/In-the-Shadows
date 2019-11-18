
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

    private     float           _RotationSpeed = 42f;
    private     float           _TranslationSpeed = 10.5f;
    private     float           _OrientationBias = 0.035f;
    private     float           _DirectionBias = 0.035f;
    private     float           _DistanceBias = 0.035f;
    private     bool            _OrientationOK;
    private     bool            _RelativePositionOK;
    private     Quaternion      _CurrentOrientation;
    private     Vector3         _CurrentPosition;
    private     Puzzle          _PuzzleContainer;

    void Start()
    {
        _PuzzleContainer = this.GetComponentInParent<Puzzle>();
        isPuzzlePieceValidated = false;
        _OrientationOK = false;
        _RelativePositionOK = false;
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
        _CurrentOrientation = this.gameObject.transform.localRotation;
        _CurrentPosition = this.gameObject.transform.localPosition;

        CheckSolutions();
        isPuzzlePieceValidated = (_OrientationOK && _RelativePositionOK);
    }

    void OnMouseDrag()
    {
        // if (!Utility.IsPointerOverUIObject())
        // {
            if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && !Input.GetMouseButton(1))
            {
                if (!RotationConstraints[0])
                {
                    this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _RotationSpeed * -1);
                    // this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0), Time.deltaTime * _RotationSpeed);
                    // this.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _RotationSpeed, Space.Self);

                }
                if (!RotationConstraints[1])
                {
                    this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0) * Time.deltaTime * _RotationSpeed);
                    // this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0), Time.deltaTime * _RotationSpeed);
                    // this.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, 0), Time.deltaTime * _RotationSpeed, Space.Self);
                }
            }

            if (Input.GetMouseButton(1) || (!Input.GetMouseButton(1) && Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
            {
                if (!TranslationConstraints[0])
                {
                    this.gameObject.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), 0, 0) * Time.deltaTime * _TranslationSpeed * -1, Space.World);
                }
                if (!TranslationConstraints[1])
                {
                    this.gameObject.transform.Translate(new Vector3(0, Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * _TranslationSpeed, Space.World);
                }
            }
        // }
    }

    private void    CheckSolutions()
    {
        CheckOrientation();
        _RelativePositionOK = CheckRelativePosition();
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
                if (    ((q.x >=  _CurrentOrientation.x - _OrientationBias && q.x <=  _CurrentOrientation.x + _OrientationBias)
                    &&   (q.y >=  _CurrentOrientation.y - _OrientationBias && q.y <=  _CurrentOrientation.y + _OrientationBias)
                    &&   (q.z >=  _CurrentOrientation.z - _OrientationBias && q.z <=  _CurrentOrientation.z + _OrientationBias)
                    &&   (q.w >=  _CurrentOrientation.w - _OrientationBias && q.w <=  _CurrentOrientation.w + _OrientationBias))
                    ||
                        ((q.x >= -_CurrentOrientation.x - _OrientationBias && q.x <= -_CurrentOrientation.x + _OrientationBias)
                    &&   (q.y >= -_CurrentOrientation.y - _OrientationBias && q.y <= -_CurrentOrientation.y + _OrientationBias)
                    &&   (q.z >= -_CurrentOrientation.z - _OrientationBias && q.z <= -_CurrentOrientation.z + _OrientationBias)
                    &&   (q.w >= -_CurrentOrientation.w - _OrientationBias && q.w <= -_CurrentOrientation.w + _OrientationBias)))
                {
                    _OrientationOK = true;
                    break;
                }
                else
                {
                    _OrientationOK = false;
                }
            }        
        }
        else
        {
            _OrientationOK = true;
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
            
            if (    (RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                &&  (RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                &&  (RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                &&  (RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
            {
                return true;
            }
            if (    CheckMirroredRelative
                &&  (RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                &&  (-RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && -RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                &&  (RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                &&  (RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
