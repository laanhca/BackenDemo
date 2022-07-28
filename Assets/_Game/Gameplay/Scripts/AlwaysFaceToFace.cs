
using UnityEngine;

public class AlwaysFaceToFace : MonoBehaviour
{
    [SerializeField] private Camera camera;
        void Update()
        {
            transform.rotation = camera.transform.rotation;
        }
}
