using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public float PipeSpeed = 300f;

	private List<Node2D> activePipes = new List<Node2D>();
	private List<Node2D> activeGrounds = new List<Node2D>();
	private Bird bird;
	private int score = 0;
	private bool gameStarted = false;
	private AudioStreamPlayer2D scoreSound;

	public override void _Ready()
	{
		bird = GetNode<Bird>("bird");
		bird.AreaEntered += OnBirdAreaEntered;
		scoreSound = GetNode<AudioStreamPlayer2D>("scoreSound");
		AudioStreamPlayer2D music = GetNode<AudioStreamPlayer2D>("music");
		music.Play();
	}

	public override void _Process(double delta)
	{
		if (!gameStarted) return;

		CheckGroundCollision();
		CheckCeilingCollision();
		if (!bird.alive) return;

		MoveAndRemoveObjects(activePipes, delta, -150);
		MoveAndRemoveObjects(activeGrounds, delta, -650);
	}

	private void CheckGroundCollision()
	{
		foreach (var ground in activeGrounds)
		{
			if (ground == null || !IsInstanceValid(ground)) continue;

			if (bird.Position.Y >= ground.Position.Y - 30)
			{
				if (bird.alive)
				{
					bird.alive = false;
					GD.Print("Bird hit the ground! Game Over");
				}
				// Stop the bird on top of the ground
				bird.Velocity = new Vector2(bird.Velocity.X, 0);
				bird.Position = new Vector2(bird.Position.X, ground.Position.Y - 20);
				return;
			}
		}
	}

	private void CheckCeilingCollision()
	{
		Node2D ceiling = GetNode<Node2D>("ceiling");
		if (ceiling == null || !IsInstanceValid(ceiling)) return;

		CollisionShape2D ceilingShape = ceiling.GetNode<CollisionShape2D>("CollisionShape2D");
		if (ceilingShape == null || !IsInstanceValid(ceilingShape)) return;

		float ceilingBottom = ceiling.GlobalPosition.Y + ceilingShape.GlobalPosition.Y + 30;
		if (bird.Position.Y <= ceilingBottom)
		{
			bird.Position = new Vector2(bird.Position.X, ceilingBottom);
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

			obj.Position = new Vector2(obj.Position.X - PipeSpeed * (float)delta, obj.Position.Y);

			if (obj.Position.X < removeThreshold)
			{
				obj.QueueFree();
				objects.RemoveAt(i);
			}
		}
	}

	private void OnBirdAreaEntered(Area2D area)
	{
		if (area.Name == "top pipe" || area.Name == "bottom pipe")
		{
			if (bird.alive)
			{
				bird.alive = false;
				GD.Print("Bird hit a pipe! Game Over");
			}
		}

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
		scoreSound.Play();
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
