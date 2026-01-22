using Godot;
using System;

public partial class Spawner : Node2D
{
	[Export] public string ObjectType = "pipe";  // "pipe" or "ground"
	[Export] public float SpawnInterval = 2.0f;
	[Export] public float MinHeight = -250f;
	[Export] public float MaxHeight = 100f;
	[Export] public float GapSize = 150f;
	[Export] public float GroundSpawnY = 600f;  // Y position for ground spawning
	[Export] public int InitialSpawns = 0;  // For ground: spawn this many sections at start
	[Export] public float SectionWidth = 336f;  // Width of one section (for ground spacing)

	private float spawnTimer = 0f;
	private PackedScene objectScene;
	private Bird bird;
	private Main main;

	public override void _Ready()
	{
		// Load the appropriate scene based on type
		objectScene = ObjectType switch
		{
			"pipe" => GD.Load<PackedScene>("res://scenes/pipes.tscn"),
			"ground" => GD.Load<PackedScene>("res://scenes/ground.tscn"),
			_ => null
		};

		if (objectScene == null)
		{
			GD.PrintErr($"Spawner: Could not load scene for type '{ObjectType}'");
			return;
		}

		bird = GetParent().GetNode<Bird>("bird");
		main = GetParent() as Main;

		// Spawn initial sections (useful for ground) - defer to avoid conflicts during setup
		if (InitialSpawns > 0)
		{
			CallDeferred(MethodName.SpawnInitialSections);
		}
	}

	private void SpawnInitialSections()
	{
		for (int i = 0; i < InitialSpawns; i++)
		{
			SpawnObject(i * SectionWidth);
		}
	}

	public void RespawnInitialSections()
	{
		CallDeferred(MethodName.SpawnInitialSections);
	}

	public override void _Process(double delta)
	{
		// Stop spawning when bird dies
		if (!bird.alive) return;

		spawnTimer += (float)delta;

		if (spawnTimer >= SpawnInterval)
		{
			SpawnObject();
			spawnTimer = 0f;
		}
	}

	private void SpawnObject(float customX = -1)
	{
		if (objectScene == null) return;

		Node2D obj = objectScene.Instantiate<Node2D>();
		GetParent().AddChild(obj);

		float spawnX = customX >= 0 ? customX : GetViewportRect().Size.X + 100;
		float spawnY = ObjectType == "pipe" ? GD.Randf() * (MaxHeight - MinHeight) + MinHeight : GroundSpawnY;

		obj.Position = new Vector2(spawnX, spawnY);

		// Register with Main for movement/removal management
		if (main != null)
		{
			main.RegisterSpawnedObject(ObjectType, obj);
		}
	}
}
