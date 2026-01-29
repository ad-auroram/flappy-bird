using Godot;

public partial class Score : Control
{
	private Label scoreLabel;
	private int currentScore = 0;
	private Main main;
	
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		main = GetParent() as Main;
	}

	public override void _Process(double delta)
	{
		currentScore = main.GetScore();
		scoreLabel.Text = $"Score: {currentScore}";
	}
}
