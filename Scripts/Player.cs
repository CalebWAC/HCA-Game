using Godot;
using FSharpScripts;

public partial class Player : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerFS.ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PlayerFS.process(delta);
	}
	
	public override void _Input(InputEvent @event) {
		PlayerFS.input(@event);
	}
}
