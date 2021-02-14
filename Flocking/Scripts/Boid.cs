using Godot;
using System;
using Flocking;
using System.Collections.Generic;

public class Boid : KinematicBody
{
	private BoidMovement refBoidMovement;
	private Vector3 previousDirection;

	private List<Boid> neighbors = new List<Boid>();
    private RayCast raycaster;
	private CollisionShape collision;

    [Export]
	public float speed;
	[Export]
	public float smoothing;
    [Export]
    public float wallDetectDistance;

	public void Initialize(FlockManager fm)
	{
		refBoidMovement = new BoidMovement();
		refBoidMovement.Initialize(fm);

		previousDirection = Vector3.Zero;

		// set a random rotation to start off with
		GD.Randomize();
		float x = (float)GD.RandRange(-1.0, 1.0);
		GD.Randomize();
		float y = (float)GD.RandRange(-1.0, 1.0);
		GD.Randomize();
		float z = (float)GD.RandRange(-1.0, 1.0);
		LookAt(new Vector3(x, y, z), GlobalTransform.basis.y);

		raycaster = (RayCast)GetNode("RayCast");
		raycaster.CastTo = new Vector3(0.0f, 0.0f, wallDetectDistance);

		collision = (CollisionShape)GetNode("Collision");

		GD.Print("Boid here!");
	}

	public void AddRayCastOverrides(List<Boid> boids)
	{
		// add the boids to the raycaster's list of exceptions so it doesn't detect itself or them
		for (int i = 0; i < boids.Count; ++i)
		{
			raycaster.AddException(boids[i].collision);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		// calculate vector of reflection off of surfaces
		Vector3 collisionReflection = GetReflectionVector();

		// calculate new flocking direction
		Vector3 newFlockDirection = refBoidMovement.GetFlockDirection(GlobalTransform.basis.z, Translation, collisionReflection);

		// smoothly interpolate between our newly calculate direction and the previous one
		Vector3 direction = previousDirection.LinearInterpolate(newFlockDirection, smoothing);

		// actually move the boid
		previousDirection = MoveAndSlide(direction * speed).Normalized();

		// look in the direction we're moving
		LookAt(Translation + (direction * -1), GlobalTransform.basis.y);
	}

	private Vector3 GetReflectionVector()
	{
		Vector3 result = Vector3.Zero;

		if (raycaster != null)
		{
			result = raycaster.GetCollisionNormal();
		}

		return result;
	}
}
