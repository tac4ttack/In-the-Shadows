using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public bool isPuzzlePieceValidated = false;

    public Quaternion[] OrientationSolutions;
    public GameObject RelativePuzzlePiece;
    public Vector3 RelativeDirectionSolution;
    public float RelativeDistanceSolution;
    public bool CheckMirroredRelative;

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

    // DEBUG
    void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Name = {this.gameObject.name}\n"
                    +$"LocalRotation = {this.gameObject.transform.localRotation}\n"
                    +$"Rotation = {this.gameObject.transform.rotation}\n"
                    +$"Relative Position Vector3 = {RelativePuzzlePiece.transform.position - this.gameObject.transform.position}\n"
                    +$"Relative Distance = {Vector3.Magnitude(RelativePuzzlePiece.transform.position - this.gameObject.transform.position)}\n");
        }
    }

    void Awake()
    {
        if (_AxisHints == null)
            _AxisHints = GameObject.FindGameObjectWithTag("Axis_Hints").GetComponent<AxisHints>();
        Assert.IsNotNull(_AxisHints, "Axis hints GameObject not found in scene!");
        _AxisHints.Enable(false);

        if (_PuzzleContainer == null)
            _PuzzleContainer = this.GetComponentInParent<Puzzle>();
        Assert.IsNotNull(_PuzzleContainer, "Puzzle script not found in puzzle piece parent!");

        if (_MeshRenderer == null)
            _MeshRenderer = this.GetComponentInChildren<MeshRenderer>();
            // _MeshRenderer = this.GetComponent<MeshRenderer>();
        Assert.IsNotNull(_MeshRenderer, "Mesh renderer component not found in puzzle piece game object!");

        _MeshRenderer.materials[0].color = Color.white;
    }

    // Init at declaration
    // void Start()
    // {
    //     isPuzzlePieceValidated = false;
    //     _OrientationOK = false;
    //     _RelativePositionOK = false;
    // }

    void FixedUpdate()
    {
        _CurrentOrientation = this.gameObject.transform.localRotation;
        _CurrentPosition = this.gameObject.transform.localPosition;
        CheckSolutions();
        isPuzzlePieceValidated = (_OrientationOK && _RelativePositionOK);
    }

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
        if (OrientationSolutions.Length > 0)
        {
            for (int i = 0; i < OrientationSolutions.Length; i++)
            {
                Quaternion q = (Quaternion)OrientationSolutions[i];
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

    //  DEBUG -> Need to test if the mirror check is good!
    //  DEBUG -> Need to adjust the BIASES!

    private bool CheckRelativePosition()
    {
        if (!RelativePuzzlePiece)
        {
            return true;
        }
        else
        {
            Vector3 currentRelativeDirection = RelativePuzzlePiece.transform.position - this.gameObject.transform.position;
            float currentRelativeDistance = currentRelativeDirection.magnitude;

            if ((RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                && (RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                && (RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                && (RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
            {
                return true;
            }
            if (CheckMirroredRelative
                && (RelativeDistanceSolution >= currentRelativeDistance - _DistanceBias && RelativeDistanceSolution <= currentRelativeDistance + _DistanceBias)
                && (-RelativeDirectionSolution.x >= currentRelativeDirection.x - _DirectionBias && -RelativeDirectionSolution.x <= currentRelativeDirection.x + _DirectionBias)
                && (RelativeDirectionSolution.y >= currentRelativeDirection.y - _DirectionBias && RelativeDirectionSolution.y <= currentRelativeDirection.y + _DirectionBias)
                && (RelativeDirectionSolution.z >= currentRelativeDirection.z - _DirectionBias && RelativeDirectionSolution.z <= currentRelativeDirection.z + _DirectionBias))
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_PuzzleContainer.CurrentState == Puzzle.PuzzleStates.WinScreen)
            return;

        
        if (GameManager.GM.Settings.MouseControls)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                this.gameObject.transform.Rotate(ComputeRotation(0), Time.deltaTime * _RotationSpeed, Space.Self);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.gameObject.transform.Translate(ComputeTranslation(0).normalized * Time.deltaTime * _TranslationSpeed, Space.Self);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
            {
                this.gameObject.transform.Translate(ComputeTranslation(0).normalized * Time.deltaTime * _TranslationSpeed, Space.Self);
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
        _MeshRenderer.materials[0].color = Color.white;
        _AxisHints.Enable(false);
    }

    private Vector3 ComputeRotation(int iMod)
    {
        Vector3 newRotation = Vector3.zero;

        if (!_RotationConstraints.y && iMod != 2)
        {
            newRotation.y = Input.GetAxis("Mouse X") * -1f;
        }
        if (!_RotationConstraints.x && iMod != 1)
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
            newTranslation.x = Input.GetAxis("Mouse X") * -1f;
        }
        return newTranslation;
    }

    #endregion
}
