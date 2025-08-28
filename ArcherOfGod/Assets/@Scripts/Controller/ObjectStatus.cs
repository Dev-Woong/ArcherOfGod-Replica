using UnityEngine;

public class ObjectStatus : MonoBehaviour
{
    [SerializeField] private LayerMask _objLayerMask;
    void Start()
    {
        
    }
    public LayerMask SetObjectLayerMask()
    {
        return _objLayerMask;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
