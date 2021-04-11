using Godot;
using System;
using Flocking;
using System.Collections.Generic;

public class FlockManager : Node
{
    [Export]
    public int numOfBoids;

    [Export]
    public PackedScene boidScene;

    [Export]
    public float distanceAlignment;
    [Export]
    public float distanceSeparation;
    [Export]
    public float distanceCohesion;

    private List<Boid> boids = new List<Boid>();

    public override void _Ready()
    {
        SpawnInitialBoids();
    }

    public override void _Process(float delta)
    {
        AddBoid();
        RemoveBoid();
    }

    private void SpawnInitialBoids()
    {
        // spawn the initial number of boids
        for (int i = 0; i < numOfBoids; ++i)
        {
            Spatial tmp = (Spatial)boidScene.Instance() as Spatial;
            tmp.Translation = new Vector3(i * 3, 0, 0);

            // initialize the boid itself
            ((Boid)tmp).Initialize(this);

            boids.Add((Boid)tmp);

            AddChild(tmp);
        }

        // for each boid, set up its raycast exceptions
        for (int i = 0; i < boids.Count; ++i)
        {
            boids[i].AddRayCastOverrides(boids);
        }
    }

    private void AddBoid()
    {
        // input here to add a new boid to the scene
        if (Input.IsActionJustPressed("ui_accept"))
        {
            Spatial tmp = (Spatial)boidScene.Instance() as Spatial;
            tmp.Translation = new Vector3(0, 0, 0);

            // initialize the boid itself
            ((Boid)tmp).Initialize(this);

            boids.Add((Boid)tmp);

            AddChild(tmp);
        }
    }

    private void RemoveBoid()
    {
        // input here to remove a boid from the scene
        if (Input.IsActionJustPressed("ui_back"))
        {
            if (boids.Count > 0)
            {
                // remove the latest boid from the list
                var tmp = boids[boids.Count - 1];
                boids.Remove(tmp);

                // destroy the latest boid
                tmp.QueueFree();
            }
        }
    }

    public List<Boid> GetBoids()
    {
        return boids;
    }
}