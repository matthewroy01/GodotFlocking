using Godot;
using System;

public class Projectile : RigidBody
{
    public override void _Ready()
    {
        Connect("body_entered", this, "Collision");
    }

    public void Fire(Vector3 direction, float speed)
    {
        ApplyCentralImpulse(direction * speed);
    }

    private void Collision(Node body)
    {
        if (body is Boid)
        {
            GD.Print("found boid " + body.Name);
        }
    }
}
