using Godot;
using System;

public partial class Bird : CharacterBody2D
{
	public const int Gravity = 800;
	public const float Max = 1000;
	public const float FlapSpeed = -350;
	
	public const float MinRotation = -1.1f;
	public const float MaxRotation = 1.1f;
	
	public bool flying = false;
	public bool falling = false;
	public bool alive = true;

	public Vector2 StartPos = new Vector2(50, 50);

	public override void _Ready()
	{
		reset();
	}

	public override void _Input(InputEvent @event)
	{
		if (alive && @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			flap();
		}
		else if (alive && @event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Space)
		{
			flap();
		}
	}

	public void flap()
	{
		flying = true;
		falling = false;
		Velocity = new Vector2(Velocity.X, FlapSpeed);
	}

	public void reset()
	{
		falling = false;
		flying = false;
		alive = true;
		Position = StartPos;
		Rotation = 0;
		Velocity = Vector2.Zero;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 vel = Velocity;

		if (alive)
		{
			// Apply gravity when flying or falling
			if (flying || falling)
			{
				vel.Y += (float)(Gravity * delta);
			}

			// Cap velocity
			if (vel.Y > Max)
			{
				vel.Y = Max;
			}

			Velocity = vel;

			UpdateRotation();
			MoveAndSlide();

			// Check if we hit anything (pipes, ground, etc.)
			if (GetSlideCollisionCount() > 0)
			{
				Die();
			}
		}
		else
		{
			// Dead: continue applying gravity to fall to floor
			vel.Y += (float)(Gravity * delta);
			if (vel.Y > Max)
			{
				vel.Y = Max;
			}
			Velocity = vel;
			MoveAndSlide();
		}
	}

	private void UpdateRotation()
	{
		// Map velocity to rotation angle	
		// Normalize velocity to -1 to 1 range
		float normalizedVelocity = Velocity.Y / Max;
		normalizedVelocity = Mathf.Clamp(normalizedVelocity, -1, 1);
		
		// When velocity is -Max (flying up), use MinRotation
		// When velocity is +Max (falling fast), use MaxRotation
		Rotation = Mathf.Lerp(MinRotation, MaxRotation, (normalizedVelocity + 1) / 2);
	}

	private void Die()
	{
		alive = false;
		flying = false;
		falling = true;
	}
}
