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

	public override void _Ready()
	{
		bird = GetNode<Bird>("bird");
		
		// Connect bird collision signal
		bird.AreaEntered += OnBirdAreaEntered;
	}

	public override void _Process(double delta)
	{
		// Stop moving objects when bird dies
		if (!bird.alive) return;

		// Move and remove pipes
		MoveAndRemoveObjects(activePipes, delta, -150);
		
		// Move and remove ground sections
		MoveAndRemoveObjects(activeGrounds, delta, -650);
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
				GD.Print("Object removed at X: ", obj.Position.X);
			}
		}
	}

	private void OnBirdAreaEntered(Area2D area)
	{
		if (!bird.alive) return;

		// Check if we hit a pipe obstacle (top or bottom)
		if (area.Name == "top pipe" || area.Name == "bottom pipe")
		{
			bird.alive = false;
			GD.Print("Bird hit a pipe! Game Over");
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

	// Legacy method for compatibility with old Pipes.cs
	public void RegisterPipe(Node2D pipe)
	{
		activePipes.Add(pipe);
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
}
