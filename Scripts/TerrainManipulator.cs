using Godot;
using FSharpScripts;

public partial class TerrainManipulator : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		TerrainManipulatorFS.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TerrainManipulatorFS.process((float) delta);
	}

	public override void _Input(InputEvent @event)
	{
		TerrainManipulatorFS.input(@event);
	}
}
