using Godot;
using FSharpScripts;

public partial class MovingBlock : Node3D
{
	private MovingBlockFS.MovingBlock block;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		block = new MovingBlockFS.MovingBlock();
		block.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		block.process((float) delta);
	}
}
