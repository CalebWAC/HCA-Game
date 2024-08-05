using Godot;
using FSharpScripts;

public partial class MiniBomb : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MiniBombFS.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//MiniBombFS.physicsProcess((float)delta);
	}
}
