using Godot;
using System;

public class Player : KinematicBody
{
    // following this tutorial for creating a first person controller: https://www.youtube.com/watch?v=mzsWfLsjG4w
    
    // stats
    public int healthCurrent;
    [Export]
    public int healtMax;
    public int score;

    // physics
    [Export]
    public float movementSpeed;
    [Export]
    public float jumpForce;
    [Export]
    public float gravity;

    // camera
    private const float LOOK_ANGLE_MIN = -90.0f;
    private const float LOOK_ANGLE_MAX = 90.0f;
    private const float LOOK_SENSITIVTY = 10.0f;

    // vectors
    private Vector3 velocity;
    private Vector2 mouseDelta;

    // components
    private Camera camera;
    private Spatial muzzle;

    // shooting
    private BulletPooler bulletPooler;
    [Export]
    public PackedScene bulletScene;
    [Export]
    public float bulletSpeed;

    public override void _Ready()
    {
        // hide and lock the mouse cursor
        Input.SetMouseMode(Input.MouseMode.Captured);

        bulletPooler = new BulletPooler();
        bulletPooler.Initialize(bulletScene, 20, GetParent());

        GetComponents();
    }

    private void GetComponents()
    {
        camera = (Camera)GetNode("Camera");
        muzzle = (Spatial)GetNode("Camera/TempGun/Muzzle");
    }

    public override void _PhysicsProcess(float delta)
    {
        Movement(delta);
        Jumping();
    }

    private void Movement(float delta)
    {
        // reset the X and Z velocities
        velocity.x = 0.0f;
        velocity.z = 0.0f;

        // movement inputs
        Vector2 input = Vector2.Zero;
        if (Input.IsActionPressed("move_forward"))
        {
            input.y -= 1;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            input.y += 1;
        }
        if (Input.IsActionPressed("move_left"))
        {
            input.x -= 1;
        }
        if (Input.IsActionPressed("move_right"))
        {
            input.x += 1;
        }
        input = input.Normalized();

        // get the forward and right directions
        var forward = GlobalTransform.basis.z;
        var right = GlobalTransform.basis.x;

        var relativeDirection = forward * input.y + right * input.x;

        // set the velocity
        velocity.x = relativeDirection.x * movementSpeed;
        velocity.z = relativeDirection.z * movementSpeed;

        // apply gravity
        velocity.y -= gravity * delta;

        // move the player
        velocity = MoveAndSlide(velocity, Vector3.Up);
    }

    private void Jumping()
    {
        // jumping
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.y = jumpForce;
        }
    }

    public override void _Process(float delta)
    {
        CameraMovement(delta);
        Fire();
    }

    private void CameraMovement(float delta)
    {
        // rotate the camera along the x axis and clamp it
        float xAngle = Mathf.Clamp(camera.RotationDegrees.x - (mouseDelta.y * LOOK_SENSITIVTY * delta), LOOK_ANGLE_MIN, LOOK_ANGLE_MAX);
        camera.RotationDegrees = new Vector3(xAngle, camera.RotationDegrees.y, camera.RotationDegrees.z);

        // rotate the player along their y axis
        float yAngle = RotationDegrees.y - (mouseDelta.x * LOOK_SENSITIVTY * delta);
        RotationDegrees = new Vector3(RotationDegrees.x, yAngle, RotationDegrees.z);;

        // reset mouse delta vector
        mouseDelta = Vector2.Zero;
    }

    private void Fire()
    {
        if (Input.IsActionJustPressed("shoot"))
        {
            Projectile tmp = bulletPooler.GetBullet();

            if (tmp != null)
            {
                tmp.GlobalTransform = muzzle.GlobalTransform;
                tmp.Fire(camera.GlobalTransform.basis.z * -1, bulletSpeed);
            }
        }
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion eventMouseMotion)
        {
            mouseDelta = eventMouseMotion.Relative;
        }
    }
}