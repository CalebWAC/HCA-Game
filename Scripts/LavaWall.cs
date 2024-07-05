using Godot;
using FSharpScripts;

public partial class LavaWall : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LavaWallFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LavaWallFS.process((float)delta);
	}
}
