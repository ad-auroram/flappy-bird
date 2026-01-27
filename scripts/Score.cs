using Godot;
using System;

public partial class Score : Control
{
	private Label scoreLabel;
	private int currentScore = 0;
	private Main main;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		main = GetParent() as Main;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentScore = main.GetScore();
		scoreLabel.Text = $"Score: {currentScore}";
	}
}
