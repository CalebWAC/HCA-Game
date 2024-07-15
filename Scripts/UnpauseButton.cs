using Godot;
using FSharpScripts;

public partial class UnpauseButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UnpauseButtonFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
