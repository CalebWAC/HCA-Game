using Godot;
using FSharpScripts;

public partial class CameraControl : Camera3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CameraControlFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CameraControlFS.process(delta);
	}

	public override void _Input(InputEvent @event)
	{
		CameraControlFS.input(@event);
	}
}
