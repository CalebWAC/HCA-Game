using Godot;
using System;
using FSharpScripts;

public partial class Glasses : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GlassesFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		GlassesFS.physicsProcess((float) delta);
	}
}
