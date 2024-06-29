using Godot;
using FSharpScripts;

public partial class PlayerControl : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerControlFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		PlayerControlFS.physicsProcess((float) delta);
	}
}
