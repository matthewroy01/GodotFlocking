using Godot;
using System;
using System.Collections.Generic;

public class FlockCamera : Camera
{
    private Vector3 defaultPosition;
    private Vector3 defaultRotation;

    [Export]
    public FlockManager refFlockManager;

    private bool followingBoid = false;
    private Boid toFollow;

    public override void _Ready()
    {
        // save default transform so we can return to it
        defaultPosition = Translation;
        defaultRotation = RotationDegrees;

        refFlockManager = (FlockManager)GetNode("/root/Spatial/FlockManager");
    }

    public override void _Process(float delta)
    {
        SwitchModes();
        FollowBoid();
    }

    private void SwitchModes()
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            followingBoid = !followingBoid;

            if (followingBoid)
            {
                List<Boid> boids = refFlockManager.GetBoids();

                GD.Randomize();
                int index = (int)GD.RandRange(0, boids.Count);

                toFollow = boids[index];
            }
            else
            {
                toFollow = null;

                Translation = defaultPosition;
                RotationDegrees = defaultRotation;
            }
        }
    }

    private void FollowBoid()
    {
        if (toFollow == null)
        {
            followingBoid = false;

            Translation = defaultPosition;
            RotationDegrees = defaultRotation;
        }

        if (followingBoid)
        {
            Translation = Translation.LinearInterpolate(toFollow.Translation - (toFollow.GlobalTransform.basis.z * 10.0f), 0.25f);
            LookAt(toFollow.Translation, Vector3.Up);
        }
    }
}
