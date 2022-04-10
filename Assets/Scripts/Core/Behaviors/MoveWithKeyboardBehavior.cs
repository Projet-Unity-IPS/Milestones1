using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Input Keys
public enum InputKeyboard
{
    arrows = 0,
    wasd = 1
}

public class MoveWithKeyboardBehavior : AgentBehaviour
{
    public InputKeyboard inputKeyboard;
    public GameObject gameManager;

    private void Start()
    {
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        //implement your code here
        var direction = Vector3.zero;
        if (inputKeyboard == InputKeyboard.arrows)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                direction += Vector3.forward;
            if (Input.GetKey(KeyCode.DownArrow))
                direction += Vector3.back;
            if (Input.GetKey(KeyCode.LeftArrow))
                direction += Vector3.left;
            if (Input.GetKey(KeyCode.RightArrow))
                direction += Vector3.right;
        }
        else if (inputKeyboard == InputKeyboard.wasd)
        {
            if (Input.GetKey(KeyCode.W))
                direction += Vector3.forward;
            if (Input.GetKey(KeyCode.S))
                direction += Vector3.back;
            if (Input.GetKey(KeyCode.A))
                direction += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                direction += Vector3.right;
        }

        direction = direction.normalized;
        var manager = gameManager.GetComponent<GameManager>();
        if (!manager.hasStarted || manager.IsGameFinished())
            direction = Vector3.zero;

        steering.linear = new Vector3(direction.x, 0, direction.z) * agent.maxAccel;
        // steering.linear = new Vector3(1, 0, 0) * agent.maxAccel;
        steering.linear =
            transform.parent.TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
        return steering;
    }
}