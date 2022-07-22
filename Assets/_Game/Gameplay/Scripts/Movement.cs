using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private int moveSpeed = 3;
    [SerializeField] private int rotateSpeed = 3;
    private void Update()
    {
        FloatingJoystick joystick = GameplayUI.Instance.joystick;
        if (joystick.Direction.sqrMagnitude > 0.01f)
        {
            Vector3 input = new Vector3(joystick.Direction.x, 0, joystick.Direction.y);
				
            LookAt(input);
            var pos = transform.position + input * moveSpeed * Time.smoothDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position,pos, moveSpeed* Time.smoothDeltaTime  );
        }
    }
    internal void LookAt(Vector3 position)
    {
        Quaternion direction = Quaternion.LookRotation(position);
        transform.rotation = Quaternion.Lerp(transform.rotation,direction, Time.deltaTime* rotateSpeed);
    }
}
