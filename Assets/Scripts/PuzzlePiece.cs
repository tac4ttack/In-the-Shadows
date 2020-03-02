using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

[SelectionBase]
public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public bool isPuzzlePieceValidated = false;
    [SerializeField] private Quaternion[] _OrientationSolutions = null;
    [SerializeField] private GameObject _RelativePuzzlePiece = null;
    [SerializeField] private Vector3 _RelativeDirectionSolution = Vector3.zero;
    [SerializeField] private float _RelativeDistanceSolution = 0f;
    [SerializeField] private bool _CheckMirroredRelative = false;
    [System.Serializable] private class RotationConstraints { public bool x = false; public bool y = false; public bool z = true; }
    [SerializeField] private RotationConstraints _RotationConstraints = new RotationConstraints();
    [System.Serializable] private class TranslationConstraints { public bool x = true; public bool y = true; public bool z = true; }
    [SerializeField] private TranslationConstraints _TranslationConstraints = new TranslationConstraints();
    [SerializeField] [Range(0.0001f, 100.0f)] private float _RotationSpeed = 42f;
    [SerializeField] [Range(0.0001f, 100.0f)] private float _TranslationSpeed = 10.5f;
    [SerializeField] [Range(0.0f, 1.0f)] private float _OrientationBias = 0.035f;
    [SerializeField] [Range(0.0f, 1.0f)] private float _DirectionBias = 0.035f;
    [SerializeField] [Range(0.0f, 1.0f)] private float _DistanceBias = 0.035f;
    private bool _OrientationOK = false;
    private bool _RelativePositionOK = false;
    private Quaternion _CurrentOrientation;
    private Vector3 _CurrentPosition;
    private Puzzle _PuzzleContainer;
    private MeshRenderer _MeshRenderer;
    private AxisHints _AxisHints;
    private Color _BaseColor;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"PUZZLE PIECE - {this.name} - Awake()");
        #endif

        if (_AxisHints == null)
            _AxisHints = GameObject.FindGameObjectWithTag("Axis Hints").GetComponent<AxisHints>();
        Assert.IsNotNull(_AxisHints, "Axis hints GameObject not found in scene!");

        if (_PuzzleContainer == null)
            _PuzzleContainer = this.GetComponentInParent<Puzzle>();
        Assert.IsNotNull(_PuzzleContainer, "Puzzle script not found in puzzle piece parent!");

        if (_MeshRenderer == null)
            _MeshRenderer = this.GetComponentInChildren<MeshRenderer>();
        Assert.IsNotNull(_MeshRenderer, "Mesh renderer component not found in puzzle piece game object!");

        _BaseColor = _MeshRenderer.materials[0].color;
    }

    void Start()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"PUZZLE PIECE - {this.name} - Start()");
        #endif

        _AxisHints.Enable(false);
        _RotationSpeed = GameManager.GM.Settings.SpeedRotation;
        _TranslationSpeed = GameManager.GM.Settings.SpeedTranslation;
    }

    void Update()
    {
        _CurrentOrientation = this.gameObject.transform.localRotation;
        _CurrentPosition = this.gameObject.transform.localPosition;
        CheckSolutions();
        isPuzzlePieceValidated = (_OrientationOK && _RelativePositionOK);

        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string debugString = $"Name = {this.gameObject.name}\n"
                    +$"LocalRotation = {this.gameObject.transform.localRotation}\n"
                    +$"Rotation = {this.gameObject.transform.rotation}\n";

            if (_RelativePuzzlePiece)
            {
                Vector3 debugRelDir = _RelativePuzzlePiece.transform.position - this.gameObject.transform.position;
                float debugRelDist = debugRelDir.magnitude;
                bool relDirTest = (_RelativeDirectionSolution.x >= debugRelDir.x - _DirectionBias && _RelativeDirectionSolution.x <= debugRelDir.x + _DirectionBias)
                                && (_RelativeDirectionSolution.y >= debugRelDir.y - _DirectionBias && _RelativeDirectionSolution.y <= debugRelDir.y + _DirectionBias)
                                && (_RelativeDirectionSolution.z >= debugRelDir.z - _DirectionBias && _RelativeDirectionSolution.z <= debugRelDir.z + _DirectionBias);
                bool relDistTest = (_RelativeDistanceSolution >= debugRelDist - _DistanceBias && _RelativeDistanceSolution <= debugRelDist + _DistanceBias);

                debugString +=
                $"Relative Position Vector3 = {_RelativePuzzlePiece.transform.position - this.gameObject.transform.position}\n"
                +$"Relative Distance = {Vector3.Magnitude(_RelativePuzzlePiece.transform.position - this.gameObject.transform.position)}\n"
                +$"Orientation check = {_OrientationOK}\nRel.Dir Check = {relDirTest}\nRel.Dist Check = {relDistTest}";
            }
            Debug.Log(debugString);
        }
        #endif
    }

    #region Settings Setters

    public void SetRotationConstraint(bool[] iConstraint)
    {
        if (iConstraint.Length == 3)
        {
            _RotationConstraints.x = iConstraint[0];
            _RotationConstraints.y = iConstraint[1];
            _RotationConstraints.z = iConstraint[2];
        }
    }

    public void SetTranslationConstraint(bool[] iConstraint)
    {
        if (iConstraint.Length == 3)
        {
            _TranslationConstraints.x = iConstraint[0];
            _TranslationConstraints.y = iConstraint[1];
            _TranslationConstraints.z = iConstraint[2];
        }
    }

    public void SetRotationSolutions(Quaternion[] iSolutions)
    {
        if (iSolutions != null && iSolutions.Length > 0)
            _OrientationSolutions = iSolutions;
    }

    public void SetDirectionSolution(Vector3 iDirection)
    {
        if (iDirection != null)
            _RelativeDirectionSolution = iDirection;
    }

    public void SetDistanceSolution(float iDistance)
    {
        _RelativeDistanceSolution = Mathf.Abs(iDistance);
    }

    public void SetRelativePuzzle(GameObject iPuzzle)
    {
        if (iPuzzle != null)
            _RelativePuzzlePiece = iPuzzle;
    }

    public void SetBiases(float iOrientation, float iDirection, float iDistance)
    {
        _OrientationBias = Mathf.Abs(iOrientation);
        _DirectionBias = Mathf.Abs(iDirection);
        _DistanceBias = Mathf.Abs(iDistance);
    }

    public void SetSpeeds(float iRotSpeed, float iTraSpeed)
    {
        _RotationSpeed = Mathf.Abs(iRotSpeed);
        _TranslationSpeed = Mathf.Abs(iTraSpeed);
    }

    #endregion

    #region Solution Check

    private void CheckSolutions()
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
    private void CheckOrientation()
    {
        if (_OrientationSolutions.Length > 0)
        {
            for (int i = 0; i < _OrientationSolutions.Length; i++)
            {
                Quaternion q = (Quaternion)_OrientationSolutions[i];
                if (((q.x >= _CurrentOrientation.x - _OrientationBias && q.x <= _CurrentOrientation.x + _OrientationBias)
                    && (q.y >= _CurrentOrientation.y - _OrientationBias && q.y <= _CurrentOrientation.y + _OrientationBias)
                    && (q.z >= _CurrentOrientation.z - _OrientationBias && q.z <= _CurrentOrientation.z + _OrientationBias)
                    && (q.w >= _CurrentOrientation.w - _OrientationBias && q.w <= _CurrentOrientation.w + _OrientationBias))
                    ||
                        ((q.x >= -_CurrentOrientation.x - _OrientationBias && q.x <= -_CurrentOrientation.x + _OrientationBias)
                    && (q.y >= -_CurrentOrientation.y - _OrientationBias && q.y <= -_CurrentOrientation.y + _OrientationBias)
                    && (q.z >= -_CurrentOrientation.z - _OrientationBias && q.z <= -_CurrentOrientation.z + _OrientationBias)
                    && (q.w >= -_CurrentOrientation.w - _OrientationBias && q.w <= -_CurrentOrientation.w + _OrientationBias)))
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
            _OrientationOK = false;
        }
    }

    /*
        Will check if the puzzle piece is correctly positionned compared to its relative child puzzle piece.
        Will only works if puzzle pieces are at the same depth (same Z coordonate)!
        It's better to set up a circular relationship like:
            A -> B -> C -> D -> A
        
        Will verify if the relative direction AND the relative distance are the planned ones.
    */
    private bool CheckRelativePosition()
    {
        if (!_RelativePuzzlePiece)
        {
            return true;
        }
        else
        {
            Vector3 currentRelativeDirection = _RelativePuzzlePiece.transform.position - this.gameObject.transform.position;
            float currentRelativeDistance = currentRelativeDirection.magnitude;

            if ((_RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && _RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                && (_RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && _RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                && (_RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && _RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                && (_RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && _RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
            {
                return true;
            }
            else if (_CheckMirroredRelative
                && (_RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && _RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                && (-_RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && -_RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                && (_RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && _RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                && (_RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && _RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion

    # region Mesh Manipulation

    public void OnBeginDrag(PointerEventData eventData)
    {
        _MeshRenderer.materials[0].color = Color.green;
        _AxisHints.Enable(true);
        Cursor.visible = false;
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[0]);

        MeshRenderer current = this.gameObject.GetComponentInChildren<MeshRenderer>();
        if (_RelativePuzzlePiece)
        {
            MeshRenderer tmp = _RelativePuzzlePiece.GetComponentInChildren<MeshRenderer>();
            if (current != tmp)
                tmp.enabled = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_PuzzleContainer.CurrentState == Puzzle.PuzzleStates.WinScreen)
            return;
        else
        {
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
            {
                this.gameObject.transform.Translate(ComputeTranslation(0).normalized * Time.deltaTime * _TranslationSpeed, Space.World);
            }
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
            {
                this.gameObject.transform.Rotate(ComputeRotation(1), Time.deltaTime * _RotationSpeed, Space.Self);
            }
            else if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
            {
                this.gameObject.transform.Rotate(ComputeRotation(2), Time.deltaTime * _RotationSpeed, Space.Self);
            }
        }
        _AxisHints.transform.rotation = this.gameObject.transform.rotation;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _MeshRenderer.materials[0].color = _BaseColor;
        _AxisHints.Enable(false);
        Cursor.visible = true;
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[7]);

        MeshRenderer current = this.gameObject.GetComponentInChildren<MeshRenderer>();
        if (_RelativePuzzlePiece != null)
        {
            MeshRenderer tmp = _RelativePuzzlePiece.GetComponentInChildren<MeshRenderer>();
            if (current != tmp)
                tmp.enabled = true;
            
            
        }
    }

    private Vector3 ComputeRotation(int iMod)
    {
        Vector3 newRotation = Vector3.zero;

        if (!_RotationConstraints.y && (iMod == 1 || iMod == 0))
        {
            newRotation.y = Input.GetAxis("Mouse X") * -1f;
        }
        if (!_RotationConstraints.x && (iMod == 2 || iMod == 0))
        {
            newRotation.x = Input.GetAxis("Mouse Y") * -1f;
        }
        return newRotation;
    }

    private Vector3 ComputeTranslation(int iMod)
    {
        Vector3 newTranslation = Vector3.zero;

        if (!_TranslationConstraints.y)
        {
            newTranslation.y = Input.GetAxis("Mouse Y");
        }
        if (!_TranslationConstraints.x)
        {
            newTranslation.x = Input.GetAxis("Mouse X") * 1f;
        }
        return newTranslation;
    }

    #endregion
}
