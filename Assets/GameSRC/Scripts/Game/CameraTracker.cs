using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [SerializeField] private Vector2 yBoundaries;
    [SerializeField] private Camera priorCamera;
    private Transform target;
    private Vector3 destination = new Vector3(0, 0, -10f);
    public Camera PriorCamera => priorCamera;
    public void SetTarget(Transform target) 
    {
        this.target = target;
    }

    private void LateUpdate()
    {
        if (target != null) 
        {            
            destination.y = target.position.y;
            destination.y = destination.y < yBoundaries.x ? yBoundaries.x : destination.y;
            destination.y = destination.y > yBoundaries.y ? yBoundaries.y : destination.y;
            transform.position = destination;
        }
    }
}