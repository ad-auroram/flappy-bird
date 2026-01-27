using Godot;
using System;

public partial class Bird : Area2D
{
	public const float Max = 1000;
	public const float FlapSpeed = -350;
	
	public const float MinRotation = -1.1f;
	public const float MaxRotation = 1.1f;
	
	public bool flying = false;
	public bool falling = false;
	
	private bool _alive = true;
	public bool alive
	{
		get => _alive;
		set
		{
			if (_alive && !value)
			{
				// Bird just died - stop animation
				StopAnimation();
			}
			_alive = value;
		}
	}
	
	public Vector2 Velocity = Vector2.Zero;

	public Vector2 StartPos = new Vector2(150, 275);

	private AnimatedSprite2D sprite;
	private Main main;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		main = GetParent() as Main;
		reset();
	}

	public override void _Input(InputEvent @event)
	{
		// Only accept input if game has started and bird is alive
		if (main == null || !alive) return;

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			flap();
		}
		else if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Space)
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
		_alive = true;
		Position = StartPos;
		Rotation = 0;
		Velocity = Vector2.Zero;
		ResumeAnimation();
	}

	private void StopAnimation()
	{
		sprite.Stop();
	}

	private void ResumeAnimation()
	{
		sprite.Play("fly");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Apply gravity when flying, falling, or dead (so bird falls to ground)
		if (flying || falling || !alive)
		{
			Velocity = new Vector2(Velocity.X, Velocity.Y + (float)(Gravity * delta));
		}

		// Cap velocity
		if (Velocity.Y > Max)
		{
			Velocity = new Vector2(Velocity.X, Max);
		}

		UpdateRotation();
			
		// Move the bird manually
		Position += Velocity * (float)delta;

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

}
