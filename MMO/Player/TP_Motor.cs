using UnityEngine;
using System.Collections;

public class TP_Motor : MonoBehaviour {

    public static TP_Motor Instance;
    public float ForwardSpeed = 10f;
    public float BackwardSpeed = 2f;
    public float StrafingSpeed = 5f;
    public float SlideSpeed = 10f;
    public float JumpSpeed = 6f;
    public float Gravity = 21f;
    public float TerminalVelocity = 20f;
    public float SlideThreshold = 0.6f;
    public float MaxControllableSlideMagnitude = 0.4f;

    private Vector3 slideDirection;

    public Vector3 MoveVector { get; set; }
    public float VerticalVelocity { get; set; }
    	
	void Awake () 
    {
        Instance = this;	
	}

    public void UpdateMotor() 
    {
        SnapAlignCharacterWithCamera();
        ProcessMotion();	
	}

   void ProcessMotion() 
    { 
        // Transfrom MoveVector to World Space
        MoveVector = transform.TransformDirection(MoveVector);

        // Normalise MoveVector if Magnitude > 1
        if (MoveVector.magnitude > 1)
            MoveVector = Vector3.Normalize(MoveVector);

        // Apply sliding if applicable
        ApplySlide();

        // Multiply MoveVector by MoveSpeed
        MoveVector *= MoveSpeed();

        // Reapply VerticalVelocity to MoveVector.y
        MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);

        // Apply Gravity
        ApplyGravity();
    
        //Move the Character in World Space
        TP_Controller.CharacterController.Move(MoveVector * Time.deltaTime);
    }

    void ApplyGravity()
   {
       if (MoveVector.y > -TerminalVelocity)
           MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

       if (TP_Controller.CharacterController.isGrounded && MoveVector.y < -1)
           MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
   }

    void ApplySlide()
    {
        if (!TP_Controller.CharacterController.isGrounded)
            return;

        slideDirection = Vector3.zero;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, Vector3.down * 2, out hitInfo))
        {
            if (hitInfo.normal.y < SlideThreshold)
                slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
        }

        if (slideDirection.magnitude < MaxControllableSlideMagnitude)
            MoveVector += slideDirection;
        else
        {
            MoveVector = slideDirection;
        }
    }

    public void Jump()
    {
        if (TP_Controller.CharacterController.isGrounded)
            VerticalVelocity = JumpSpeed;
    }

    void SnapAlignCharacterWithCamera() 
    {
        if (MoveVector.x != 0 || MoveVector.z != 0) 
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                Camera.mainCamera.transform.eulerAngles.y,
                transform.eulerAngles.z);
        
        }
 
    
    }

    float MoveSpeed()
    {
        var moveSpeed = 0f;

        switch (TP_Animator.Instance.MoveDirection)
        {
            case TP_Animator.Direction.Stationary:
                moveSpeed = 0;
                break;
            case TP_Animator.Direction.Forward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_Animator.Direction.Backward:
                moveSpeed = BackwardSpeed;
                break;
            case TP_Animator.Direction.Left:
                moveSpeed = StrafingSpeed;
                break;
            case TP_Animator.Direction.Right:
                moveSpeed = StrafingSpeed;
                break;
            case TP_Animator.Direction.LeftForward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_Animator.Direction.RightForward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_Animator.Direction.LeftBackward:
                moveSpeed = BackwardSpeed;
                break;
            case TP_Animator.Direction.RightBackward:
                moveSpeed = BackwardSpeed;
                break;
        }

        if (slideDirection.magnitude > 0)
            moveSpeed = SlideSpeed;

        return moveSpeed;
    }

}
