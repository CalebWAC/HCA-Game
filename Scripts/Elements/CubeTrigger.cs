using Godot;
using System;
using FSharpScripts;

public partial class CubeTrigger : Node3D
{
	private CubeTriggerFS.CubeTrigger trigger;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		trigger = new CubeTriggerFS.CubeTrigger();
		trigger.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		trigger.process((float) delta);
	}
}
