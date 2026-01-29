using Godot;

public partial class Start : Control
{
	private Main main;

	public override void _Ready()
	{
		main = GetParent() as Main;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			StartGame();
		}
		else if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Space)
		{
			StartGame();
		}
	}

	private void StartGame()
	{
		Visible = false;
		if (main != null && !main.IsGameStarted())
		{
			main.StartGame();
        }
	}
}
