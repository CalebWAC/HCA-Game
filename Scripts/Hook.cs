using Godot;
using FSharpScripts;

public partial class Hook : Node3D
{
	private HookFS.Hook hook;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hook = new HookFS.Hook();
		hook.ready(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnArea3DInputEvent()
	{
		GD.Print("It works!");
	}
}
