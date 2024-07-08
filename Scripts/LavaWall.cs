using Godot;
using FSharpScripts;

public partial class LavaWall : Node3D
{
	private LavaWallFS.LavaWall lavaWall;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lavaWall = new LavaWallFS.LavaWall();
		lavaWall.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		lavaWall.process((float)delta);
	}
}
