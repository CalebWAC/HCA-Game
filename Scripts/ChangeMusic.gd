extends Node

# Called when the node enters the scene tree for the first time.
func _ready():
	var name = get_tree().current_scene.scene_file_path
	if name.contains("Level") && !name.contains("Select"):
		IntroMusic.playing = false
	else:
		if !IntroMusic.playing:
			IntroMusic.playing = true
# try (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18..19]).ToString().ToInt() - 1
# with | _ -> (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18]).ToString().ToInt() - 1

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
