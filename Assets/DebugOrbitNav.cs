using UnityEngine;

public class DebugOrbitNav : MonoBehaviour
{
    private Vector3 _startPos;
    private Vector3 _targetPos;
    public float _camAltitude = 2f;
    private float _timer = 0f;
    public float _speed = 1f;

    void Awake()
    {
        _targetPos = Camera.main.transform.position;
    }

    void Update()
    {
        if (Camera.main.transform.position != _targetPos)
        {
            Debug.Log("moving");
            Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, _targetPos, _timer);
            Camera.main.transform.LookAt(this.transform.position);
        }

        _timer = Time.deltaTime * _speed;
    }

    void OnMouseDown()
    {
        Vector3 clickPosition;
        clickPosition = Input.mousePosition;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Clicked on gameobject: " + hit.collider.name);
            Debug.Log(hit.point);

            _startPos = Camera.main.transform.position;
            _targetPos = (hit.point - this.gameObject.transform.position) * _camAltitude;
        }
    }
}