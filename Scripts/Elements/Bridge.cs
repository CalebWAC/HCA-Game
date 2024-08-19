using Godot;
using FSharpScripts;

public partial class Bridge : Node3D
{
	private BridgeFS.Bridge bridge;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bridge = new BridgeFS.Bridge();
		bridge.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bridge.process((float) delta);
	}
}
