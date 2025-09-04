using UnityEngine;

public class PlayerInput
{
    private Vector2 inputVector = Vector2.zero;
    
    public bool IsPointerUp { get; set; }


    public Vector2 InputVector => inputVector;


    public void SetInputVector(Vector2 newInputVector)
    {
        inputVector = newInputVector;
    }
}
