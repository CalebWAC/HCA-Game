using Godot;
using FSharpScripts;

public partial class LavaPlume : Node3D
{
	private LavaPlumeFS.LavaPlume plume;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		plume = new LavaPlumeFS.LavaPlume();
		plume.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		plume.process((float) delta);
	}
}
