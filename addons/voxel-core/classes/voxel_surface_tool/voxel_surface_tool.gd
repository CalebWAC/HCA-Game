@tool
@icon("res://addons/voxel-core/classes/voxel_surface_tool/voxel_surface_tool.svg")
class_name VoxelSurfaceTool
extends RefCounted
## Helper tool to create voxel geometry; part of Voxel-Core.



# Inner Classes
class Surface extends SurfaceTool:
	# Public Variables
	## Index of the last vertex added.
	var index : int
	
	
	
	# Built-In Virtual Methods
	func _init() -> void:
		index = 0
		begin(Mesh.PRIMITIVE_TRIANGLES)



# Enums
## Approaches for generating voxel meshes, each approach has its own advantage 
## and disadvantage; for more information visit:
## [url=http://web.archive.org/web/20200428085802/https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/]Voxel Meshing[/url],
## [url=http://web.archive.org/web/20201112011204/https://www.gedge.ca/dev/2014/08/17/greedy-voxel-meshing]Greedy Voxel Meshing[/url]
## [br]
## - Brute meshing, no culling of voxel faces; renders all voxel faces
## regardless of obstruction. (worst optimized approach)
## [br]f
## - Naive meshing, simple culling of voxel faces; only renders all non
## obstructed voxels. (best optimized approach for its execution cost)
## [br]
## - Greedy meshing, culls and merges similar voxel faces; renders all non
## obstructed voxels while reducing face count. (best used for static content,
## as its very costly to execute)
enum MeshType {
	BRUTE, ## No culling of voxel faces; renders all voxel faces regardless of obstruction.
	NAIVE, ## Simple culling of voxel faces; only renders all non obstructed voxels.
	GREEDY, ## Culls and merges similar voxel faces; renders all non obstructed voxels while reducing face count.
}



# Private Variables
# Flag signaling the start of meshing.
var _began : bool = false

# [VoxelSet] used to create voxel mesh.
var _voxel_set : VoxelSet

# Size of voxels being created.
var _voxel_size : float

# Flag signaling whether created voxel mesh is being uv mapped.
var _voxel_textured : bool

# Scale of uv mapping used for voxel mesh being created.
var _voxel_uv_scale : Vector2

var _voxel_offset : Vector3

# Collection of [Surface]s being created.
var _surfaces : Dictionary



# Public Methods
func set_offset(offset : Vector3) -> void:
	if _began:
		push_error("Can't call on `set_offset` having began VoxelSurfaceTool")
		return
	
	_voxel_offset = offset

## Initiates the voxel mesh creation process, must be called before passing in 
## any information.
func begin(voxel_set : VoxelSet, voxel_size : float = 0.25, voxel_textured : bool = false) -> void:
	clear()
	_voxel_set = voxel_set
	_voxel_size = voxel_size
	_voxel_textured = voxel_textured
	if _voxel_textured:
		if is_instance_valid(voxel_set.texture_atlas):
			if voxel_set.texture_atlas_dimensions == Vector2i.ZERO:
				_voxel_textured = false
				push_warning("VoxelSet passed to VoxelSurfaceTool has invalid `texture_dimensions`")
			else:
				_voxel_uv_scale = Vector2.ONE / (voxel_set.texture_atlas.get_size() / Vector2(voxel_set.texture_atlas_dimensions))
		else:
			_voxel_textured = false
			push_warning("VoxelSet passed to VoxelSurfaceTool is missing `texture`")
	_began = true


## Clear all information passed into the voxel surface tool so far.
func clear() -> void:
	_voxel_set = null
	_voxel_size = 0.25
	_voxel_textured = false
	_voxel_uv_scale = Vector2.ZERO
	_surfaces.clear()
	_began = false


## Returns a constructed [ArrayMesh] from current information passed in. If an 
## existing [ArrayMesh] is passed in as an argument, will add extra surface(s) 
## to the existing [ArrayMesh].
func commit(existing : ArrayMesh = null, flags : int = 0) -> ArrayMesh:
	if not _began:
		push_error("Can't call on `commit` without beginning VoxelSurfaceTool")
		return
	
	if not is_instance_valid(existing):
		existing = ArrayMesh.new()
	
	for surface_id in _surfaces:
		var surface : Surface = _surfaces[surface_id]
		surface.commit(existing, flags)
	
	return existing


## Adds a voxel's face mesh at the given voxel postion using the specified 
## [Voxel]s attributes.
func add_face(voxel_position : Vector3i, voxel_id : int, voxel_face : Vector3i) -> void:
	if not _began:
		push_error("Can't call on `add_face` without beginning VoxelSurfaceTool")
		return
	
	var voxel : Voxel = _voxel_set.get_voxel(voxel_id)
	
	# Surface ID(e.g. "1")
	var surface_id : String = str(voxel.material_index)
	
	# Should Surface be textured?
	var surface_textured : bool = _voxel_textured and voxel.has_face_texture(voxel_face)
	if surface_textured:
		# Mark Surface ID as textured(e.g. "1_textured")
		surface_id += "_textured"
	
	# Surface to which to add face to
	var surface : Surface
	if _surfaces.has(surface_id):
		surface = _surfaces[surface_id]
	else:
		surface = Surface.new()
		_voxel_set.format_material(
				_voxel_set.get_material_by_index(voxel.material_index))
		surface.set_material(
				_voxel_set.get_material_by_index(voxel.material_index))
		_surfaces[surface_id] = surface
	
	surface.set_normal(voxel_face)
	surface.set_color(voxel.color)
	
	match voxel_face:
		Voxel.FACE_RIGHT:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.UP) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.ONE) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.BACK) * _voxel_size)
		Voxel.FACE_LEFT:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.BACK) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP + Vector3i.BACK) * _voxel_size)
		Voxel.FACE_TOP:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP + Vector3i.BACK) *_voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.ONE) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.UP) * _voxel_size)
		Voxel.FACE_BOTTOM:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.BACK) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.BACK) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position) * _voxel_size)
		Voxel.FACE_FRONT:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.UP) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP) * _voxel_size)
		Voxel.FACE_BACK:
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.RIGHT) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.ONE) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.ONE) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.RIGHT + Vector3i.BACK) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face)) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.UP + Vector3i.BACK) * _voxel_size)
			if surface_textured:
				surface.set_uv(Vector2(voxel.get_face_texture(voxel_face) + Vector2i.DOWN) * _voxel_uv_scale)
			surface.add_vertex(_voxel_offset + (voxel_position + Vector3i.BACK) * _voxel_size)
	
	surface.index += 4
	surface.add_index(surface.index - 4)
	surface.add_index(surface.index - 3)
	surface.add_index(surface.index - 2)
	surface.add_index(surface.index - 3)
	surface.add_index(surface.index - 1)
	surface.add_index(surface.index - 2)


## Adds all the faces of a [Voxel] at the given voxel postion.
func add_faces(voxel_position : Vector3i, voxel_id : int) -> void:
	if not _began:
		push_error("Can't call on `add_face` without beginning VoxelSurfaceTool")
		return
	for voxel_face in Voxel.FACES:
		add_face(voxel_position, voxel_id, voxel_face)


## Passes in information from an existing
## [code]voxel_object[/code](e.g. [VoxelMeshInstance3D]) and
## returns a constructed [ArrayMesh]. Voxel mesh is generated using a
## [member MeshType] passed via [code]voxel_mesh_mode[/code]. Can
## delimitate voxels passed from [code]voxel_object[/code] by
## passing a array of targeted [code]voxel_positions[/code]
## (e.g. Array[ Vector3i ]).
## NOTE: Internally calls on [method clear] first.
func create_from(
		voxel_object,
		voxel_mesh_type : MeshType,
		voxel_positions : Array = []) -> ArrayMesh:
	if not voxel_object.has_voxel_set():
		push_error("Can't create VoxelSurface from voxel_object without VoxelSet")
	elif not voxel_object.has_voxels():
		push_error("Can't create VoxelSurface from voxel_object without Voxels")
	
	begin(
			voxel_object.voxel_set,
			voxel_object.voxel_size,
			voxel_object.voxel_textured and voxel_object.voxel_set.has_texture_atlas())
	
	if voxel_positions.is_empty():
		voxel_positions = voxel_object.get_voxel_positions()
	
	match voxel_mesh_type:
		MeshType.BRUTE:
			for voxel_position in voxel_positions:
				var voxel_id : int = voxel_object.get_voxel_id(voxel_position)
				add_faces(voxel_position, voxel_id)
		MeshType.NAIVE:
			for voxel_position in voxel_positions:
				var voxel_id : int = voxel_object.get_voxel_id(voxel_position)
				for voxel_face in Voxel.FACES:
					if not voxel_positions.has(voxel_position + voxel_face):
						add_face(voxel_position, voxel_id, voxel_face)
		MeshType.GREEDY:
			pass
	
	return null
