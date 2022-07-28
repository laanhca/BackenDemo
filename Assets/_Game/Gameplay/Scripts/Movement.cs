
using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float rotateSpeed = 3;
    public static Action OnMovement;
    private void Update()
    {
        FloatingJoystick joystick = GameplayUI.Instance.Joystick;
        if (joystick.Direction.sqrMagnitude > 0.01f)
        {
            Vector3 input = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);
            OnMovement?.Invoke();
            LookAt(input);
            var pos = transform.position + input * moveSpeed * Time.smoothDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position,pos, moveSpeed* Time.smoothDeltaTime  );
        }
        
    }
    internal void LookAt(Vector3 position)
    {
        Quaternion direction = Quaternion.LookRotation(position);
        transform.rotation = Quaternion.Lerp(transform.rotation,direction, Time.smoothDeltaTime* rotateSpeed);
    }
}
