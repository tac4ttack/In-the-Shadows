using System.Collections;
using UnityEngine;

public class DebugOrbitNav : MonoBehaviour
{
    public float _camAltitude = 3f;
    public float _speed = 1f;
    private IEnumerator _orbitCamCoroutine;
    private bool _orbiting = false;

    void OnMouseDown()
    {
        if (_orbiting)
            StopCoroutine(_orbitCamCoroutine);   
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            _orbitCamCoroutine = OrbitCamera(Camera.main.transform.position, (hit.point - this.gameObject.transform.position) * _camAltitude, 1f);
            StartCoroutine(_orbitCamCoroutine);
        }
    }

    IEnumerator OrbitCamera(Vector3 iStart, Vector3 iTarget, float iTime)
    {
        _orbiting = true;
        for (float t = 0f; t < iTime; t += Time.deltaTime * _speed)
        {
            Camera.main.transform.position = Vector3.Slerp(iStart, iTarget, t / iTime);
            Camera.main.transform.LookAt(this.transform.position);
            yield return 0;
        }
        _orbiting = false;
    }
}