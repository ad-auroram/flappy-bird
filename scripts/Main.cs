using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public float PipeSpeed = 300f;

	private List<Node2D> activePipes = new List<Node2D>();
	private List<Node2D> activeGrounds = new List<Node2D>();
	private Bird bird;
	private int score = 0;
	private bool gameStarted = false;

	public override void _Ready()
	{
		bird = GetNode<Bird>("bird");
		
		// Connect bird collision signal
		bird.AreaEntered += OnBirdAreaEntered;
	}

	public override void _Process(double delta)
	{
		// Only run game logic after game has started
		if (!gameStarted) return;

		// Always check ground collision (even when dead, to stop bird at ground)
		CheckGroundCollision();

		// Check ceiling collision
		CheckCeilingCollision();

		// Stop moving objects when bird dies
		if (!bird.alive) return;

		// Move and remove pipes
		MoveAndRemoveObjects(activePipes, delta, -150);
		
		// Move and remove ground sections
		MoveAndRemoveObjects(activeGrounds, delta, -650);
	}

	private void CheckGroundCollision()
	{
		// Check if bird is hitting any ground section
		foreach (var ground in activeGrounds)
		{
			if (ground == null || !IsInstanceValid(ground)) continue;

			// Simple collision check: if bird Y is below ground Y minus some offset, it's hitting
			// Adjust the offset based on your bird and ground sizes
			if (bird.Position.Y >= ground.Position.Y - 30)
			{
				if (bird.alive)
				{
					bird.alive = false;
					GD.Print("Bird hit the ground! Game Over");
				}
				// Stop the bird on top of the ground
				bird.Velocity = new Vector2(bird.Velocity.X, 0);
				bird.Position = new Vector2(bird.Position.X, ground.Position.Y - 30);
				return;
			}
		}
	}

	private void CheckCeilingCollision()
	{
		// Try to get the ceiling node from the scene
		Node2D ceiling = GetNode<Node2D>("ceiling");
		if (ceiling == null || !IsInstanceValid(ceiling)) return;

		// Get the collision shape from the ceiling to check bounds
		// Assuming the ceiling has a CollisionShape2D child
		CollisionShape2D ceilingShape = ceiling.GetNode<CollisionShape2D>("CollisionShape2D");
		if (ceilingShape == null || !IsInstanceValid(ceilingShape)) return;

		// Get the ceiling's bottom position (top edge where bird would collide)
		float ceilingBottom = ceiling.GlobalPosition.Y + ceilingShape.GlobalPosition.Y + 30;

		// If bird goes above ceiling, push it down slightly
		if (bird.Position.Y <= ceilingBottom)
		{
			bird.Position = new Vector2(bird.Position.X, ceilingBottom);
			// Set velocity to zero or slightly downward to push bird down
			if (bird.Velocity.Y < 0)
			{
				bird.Velocity = new Vector2(bird.Velocity.X, 100);
			}
		}
	}

	private void MoveAndRemoveObjects(List<Node2D> objects, double delta, float removeThreshold)
	{
		for (int i = objects.Count - 1; i >= 0; i--)
		{
			Node2D obj = objects[i];

			if (obj == null || !IsInstanceValid(obj))
			{
				objects.RemoveAt(i);
				continue;
			}

			// Move object left
			obj.Position = new Vector2(obj.Position.X - PipeSpeed * (float)delta, obj.Position.Y);

			// Remove when offscreen
			if (obj.Position.X < removeThreshold)
			{
				obj.QueueFree();
				objects.RemoveAt(i);
			}
		}
	}

	private void OnBirdAreaEntered(Area2D area)
	{
		// Check if we hit a pipe obstacle (top or bottom)
		if (area.Name == "top pipe" || area.Name == "bottom pipe")
		{
			if (bird.alive)
			{
				bird.alive = false;
				GD.Print("Bird hit a pipe! Game Over");
			}
		}
		// Check if bird entered the score space
		else if (area.Name == "score space")
		{
			AddScore(1);
		}
		if (area.Name == "ground")
		{
			bird.alive = false;
			bird.flying = false;
			bird.falling = false;
			GD.Print("Bird hit the ground! Game Over");
		}
	}

	// Called by spawners to register new objects
	public void RegisterSpawnedObject(string objectType, Node2D obj)
	{
		if (objectType == "pipe")
		{
			activePipes.Add(obj);
		}
		else if (objectType == "ground")
		{
			activeGrounds.Add(obj);
		}
	}


	public void AddScore(int points)
	{
		score += points;
		GD.Print("Score: ", score);
	}

	public int GetScore()
	{
		return score;
	}

	public void ResetGame()
	{
		score = 0;
		
		// Clear all pipes and grounds
		foreach (var pipe in activePipes)
		{
			if (pipe != null && IsInstanceValid(pipe))
			{
				pipe.QueueFree();
			}
		}
		activePipes.Clear();

		foreach (var ground in activeGrounds)
		{
			if (ground != null && IsInstanceValid(ground))
			{
				ground.QueueFree();
			}
		}
		activeGrounds.Clear();

		// Respawn initial ground sections
		Spawner groundSpawner = GetNode<Spawner>("GroundSpawner");
		if (groundSpawner != null)
		{
			groundSpawner.RespawnInitialSections();
		}
	}

	public void StartGame()
	{
		gameStarted = true;
		GD.Print("Game Started!");
	}

	public bool IsGameStarted()
	{
		return gameStarted;
	}
}
