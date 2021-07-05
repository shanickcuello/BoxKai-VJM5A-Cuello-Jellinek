using UnityEngine;

public class GetRaycast : MonoBehaviour
{
    Camera camera;

    public MeshFilter PlaneTransform;
    public Transform Player;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        var plane = new Plane(PlaneTransform.transform.position, 
            PlaneTransform.transform.position + 
            PlaneTransform.transform.forward, 
            PlaneTransform.transform.position + 
            PlaneTransform.transform.right);

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!plane.Raycast(ray, out var enter)) return;

        var hitPosition = ray.GetPoint(enter);

        if (hitPosition.x < PlaneTransform.mesh.bounds.min.x * PlaneTransform.transform.lossyScale.x
            || hitPosition.x > PlaneTransform.mesh.bounds.max.x * PlaneTransform.transform.lossyScale.x
            || hitPosition.z < PlaneTransform.mesh.bounds.min.z * PlaneTransform.transform.lossyScale.z
            || hitPosition.z > PlaneTransform.mesh.bounds.max.z * PlaneTransform.transform.lossyScale.z)
        {
            return;
        }

        Player.position = hitPosition + Vector3.up * 0.1f;
    }
}
