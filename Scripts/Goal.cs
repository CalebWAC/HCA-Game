using Godot;
using FSharpScripts;

public partial class Goal : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GoalFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GoalFS.physicsProcess((float) delta);
	}
}