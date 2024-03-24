using UnityEngine;

class SplineFollower : MonoBehaviour
{
    public SplinePath Path;
    public float Speed = 0.1f;

    public float _progress;
    public bool _simulate;

    private void Update()
    {
        float vel = Path.SampleSpeed(_progress);
        if (vel == 0f) _progress = 0f;
        else if (_simulate)
            _progress += (Speed / vel) * Time.deltaTime;
        transform.position = Path.Sample(_progress);
        transform.rotation = Path.SampleDirection(_progress);
    }
}
