using Godot;
using FSharpScripts;

public partial class Goal : Node3D
{
	private GoalFS.Goal goal;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		goal = new GoalFS.Goal();
		goal.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		goal.physicsProcess((float) delta);
	}
}