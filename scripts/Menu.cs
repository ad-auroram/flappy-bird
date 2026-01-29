using Godot;

public partial class Menu : Control
{
	private Label scoreLabel;
	private Label highScoreLabel;
	private Button restartButton;
	private Bird bird;
	private Main main;
	private int currentScore = 0;
	private int highScore = 0;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("VBoxContainer/ScoreLabel");
		highScoreLabel = GetNode<Label>("VBoxContainer/HighScoreLabel");
		restartButton = GetNode<Button>("VBoxContainer/RestartButton");
		bird = GetParent().GetNode<Bird>("bird");
		main = GetParent() as Main;

		// Connect signals
		restartButton.Pressed += OnRestartPressed;

		Visible = false;
		highScore = LoadHighScore();
	}

	public override void _Process(double delta)
	{
		if (!bird.alive && !Visible)
		{
			ShowGameOverMenu();
		}
	}

	private void ShowGameOverMenu()
	{
		currentScore = main.GetScore();
		
		if (currentScore > highScore)
		{
			highScore = currentScore;
			SaveHighScore(highScore);
		}

		scoreLabel.Text = $"Score: {currentScore}";
		highScoreLabel.Text = $"High Score: {highScore}";
		Visible = true;
	}

	private void OnRestartPressed()
	{
		bird.reset();
		main.ResetGame();
		Visible = false;
	}

	private int LoadHighScore()
	{
		// Try to load from file
		if (FileAccess.FileExists("flappybird_highscore.save"))
		{
			var file = FileAccess.Open("flappybird_highscore.save", FileAccess.ModeFlags.Read);
			return int.Parse(file.GetAsText());
		}
		return 0;
	}

	private void SaveHighScore(int score)
	{
		var file = FileAccess.Open("flappybird_highscore.save", FileAccess.ModeFlags.Write);
		file.StoreString(score.ToString());
	}
}
