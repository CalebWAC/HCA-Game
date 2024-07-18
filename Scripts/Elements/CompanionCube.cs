using Godot;
using FSharpScripts;

public partial class CompanionCube : Node3D
{
	private CompanionCubeFS.CompanionCube cube;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cube = new CompanionCubeFS.CompanionCube();
		cube.ready(this);
		CompanionCubeFS.companionCubesCode.Add(cube);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		cube.physicsProcess((float) delta);
	}
	
	public override void _Input(InputEvent @event) {
		cube.input(@event);
	}
}
