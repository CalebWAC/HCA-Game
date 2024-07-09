using Godot;
using System;
using FSharpScripts;

public partial class GoalFragment : Node3D
{
	private GoalFragmentFS.Fragment frag;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		frag = new GoalFragmentFS.Fragment();
		frag.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		frag.physicsProcess((float) delta);
	}
}
