using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    private Vector2 inputVector = Vector2.zero;

    public Vector2 InputVector => inputVector;


    public void SetInputVector(Vector2 newInputVector)
    {
        inputVector = newInputVector;
    }
}
