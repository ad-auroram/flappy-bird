using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	[Export] public float PipeSpeed = 300f;

	private List<Node2D> activePipes = new List<Node2D>();
	private Bird bird;

	public override void _Ready()
	{
		bird = GetNode<Bird>("bird");
		
		// Connect bird collision signal
		bird.AreaEntered += OnBirdAreaEntered;
	}

	public override void _Process(double delta)
	{
		// Stop moving pipes when bird dies
		if (!bird.alive) return;

		// Move all active pipes left
		for (int i = activePipes.Count - 1; i >= 0; i--)
		{
			Node2D pipe = activePipes[i];

			if (pipe == null || !IsInstanceValid(pipe))
			{
				activePipes.RemoveAt(i);
				continue;
			}

			// Move pipe left
			pipe.Position = new Vector2(pipe.Position.X - PipeSpeed * (float)delta, pipe.Position.Y);

			// Remove when offscreen (add buffer for pipe width)
			if (pipe.Position.X < -150)
			{
				pipe.QueueFree();
				activePipes.RemoveAt(i);
				GD.Print("Pipe removed at X: ", pipe.Position.X);
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

	// Called by Pipes spawner to register new pipes
	public void RegisterPipe(Node2D pipe)
	{
		activePipes.Add(pipe);
	}
}
