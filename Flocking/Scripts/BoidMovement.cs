using Godot;
using System;
using System.Collections.Generic;

namespace Flocking
{
	public class BoidMovement : Node
	{
		private const float WEIGHT_WANDER = 0.2f;
		private const float WEIGHT_SEPARATION = 0.40f;
		private const float WEIGHT_ALIGNMENT = 0.20f;
		private const float WEIGHT_COHESION = 0.20f;

		private const float WEIGHT_REFLECTION = 0.1f;

		private FlockManager refFlockManager;

		private Vector3 currentForward;
		private Vector3 currentPosition;

		private float togetherCount;

		public void Initialize(FlockManager fm)
		{
			refFlockManager = fm;
		}

		public Vector3 GetFlockDirection(Vector3 forward, Vector3 position, Vector3 reflection)
		{
			// reset our counter
			togetherCount = 0;

			// save the current forward direction for use in calculation
			currentForward = forward;
			currentPosition = position;

			List<Boid> boids = refFlockManager.GetBoids();

			// get all of our weighted flock directions
			Vector3 wander = GetWander();
            Vector3 cohesion = GetCohesion(boids);
            Vector3 alignment = GetAlignment(boids);
            Vector3 separation = GetSeparation(boids);

			wander *= WEIGHT_WANDER;
			cohesion *= WEIGHT_COHESION;
			alignment *= WEIGHT_ALIGNMENT;
			separation *= WEIGHT_SEPARATION;

			return wander + cohesion + alignment + separation + (reflection * WEIGHT_REFLECTION);
		}

		private Vector3 GetWander()
		{
			GD.Randomize();
			float x = (float)GD.RandRange(-1.0, 1.0);
            GD.Randomize();
            float y = (float)GD.RandRange(-1.0, 1.0);
            GD.Randomize();
            float z = (float)GD.RandRange(-1.0, 1.0);
			Vector3 result = currentForward + new Vector3(x, y, z);

			return result * WEIGHT_WANDER;
		}

		private Vector3 GetSeparation(List<Boid> boids)
		{
            Vector3 result = Vector3.Zero;

            // find all boids in range and accumulate an average direction vector
            for (int i = 0; i < boids.Count; ++i)
            {
                float distance = currentPosition.DistanceTo(boids[i].Translation);
                if (distance < refFlockManager.distanceSeparation)
                {
                    // add the direction away from the other boid, multiplied by a scalar that increases the closer the boid is
                    result += (currentPosition - boids[i].Translation) * ((refFlockManager.distanceSeparation * 0.8f) - distance);
                }
            }

            // calculate an average
            if (boids.Count != 0)
            {
                result /= boids.Count;
            }

            return result;
		}

		private Vector3 GetAlignment(List<Boid> boids)
		{
            Vector3 result = Vector3.Zero;

			// find all boids in range and accumulate an average direction vector
			for (int i = 0; i < boids.Count; ++i)
			{
				float distance = currentPosition.DistanceTo(boids[i].Translation);
				if (distance < refFlockManager.distanceAlignment)
				{
					// add direction the other boid is moving in, multiplied by a scalar that increases the closer the boid is
					result += boids[i].GlobalTransform.basis.z * (refFlockManager.distanceAlignment - distance);

                    IncrementTogetherCount(i);
                }
			}

			// calculate an average
			if (boids.Count != 0)
			{
                result /= boids.Count;
            }

            return result;
		}

		private Vector3 GetCohesion(List<Boid> boids)
		{
            Vector3 result = Vector3.Zero;

			// find all boids in range and accumulate an average direction vector
			for (int i = 0; i < boids.Count; ++i)
			{
				float distance = currentPosition.DistanceTo(boids[i].Translation);
				if (distance < refFlockManager.distanceCohesion)
				{
					// add the direction to the other boid
					result += boids[i].Translation - currentPosition;

                    IncrementTogetherCount(i);
				}
			}

			// calculate an average
			if (boids.Count != 0)
			{
				result /= boids.Count;
			}

            return result;
		}

		private void IncrementTogetherCount(int i)
		{
            if (togetherCount <= i)
            {
                togetherCount++;
            }
		}

		private bool GetTogetherCount()
		{
			if (togetherCount > 15)
			{
				return true;
			}
			return false;
		}
	}
}
