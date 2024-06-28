using Godot;
using FSharpScripts;

public partial class World : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		WorldFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		WorldFS.process(delta);
	}
}
