using Godot;
using System;

public partial class Pipes : Node2D
{
    [Export] public float SpawnInterval = 2.0f;
    [Export] public float PipeSpeed = 300f;
    [Export] public float MinHeight = -250f;
    [Export] public float MaxHeight = 100f;
    [Export] public float GapSize = 150f;

    private float spawnTimer = 0f;
    private PackedScene pipeScene;
    private Bird bird;

    public override void _Ready()
    {
        pipeScene = GD.Load<PackedScene>("res://scenes/pipes.tscn");
        bird = GetParent().GetNode<Bird>("bird");
    }

    public override void _Process(double delta)
    {
        // Stop spawning pipes when bird dies
        if (!bird.alive) return;

        spawnTimer += (float)delta;

        if (spawnTimer >= SpawnInterval)
        {
            SpawnPipe();
            spawnTimer = 0f;
        }
    }

    private void SpawnPipe()
    {
        if (pipeScene == null) return;

        Node2D pipe = pipeScene.Instantiate<Node2D>();
        GetParent().AddChild(pipe);

        float randomHeight = GD.Randf() * (MaxHeight - MinHeight) + MinHeight;
        pipe.Position = new Vector2(GetViewportRect().Size.X + 100, randomHeight);

        // Register pipe with Main for movement handling
        if (GetParent() is Main main)
        {
            main.RegisterPipe(pipe);
        }
    }
}
