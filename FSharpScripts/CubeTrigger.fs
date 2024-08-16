namespace FSharpScripts

open Godot
open WorldFS
open WorldGeneratorFS
open GlobalFunctions

module CubeTriggerFS =
    type CubeTrigger() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable falling = false // For Level 11's falling terrain manipulator
        
        let onActivated (other : Area3D) =
            if other.GetParent().IsInGroup(new StringName "cubes") then
                match level with
                | 9 -> bridges.ForEach(fun b -> b.Visible <- true)
                | 10 ->
                    let scene = GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
                    let power = scene.Instantiate() :?> Node3D
                    power.Position <- Vector3(1f, 6f, 6f)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
                    falling <- true
                | 11 ->
                    match self.Position.X, self.Position.Y, self.Position.Z with
                    | -6f, 1f, -6f -> goalFragments.Find(fun g -> g.Position = Vector3(-6f, 2f, -6f)).Visible <- true
                    | 0f, 5f, 6f -> goalFragments.Find(fun g -> g.Position = Vector3(0f, 6f, 6f)).Visible <- true
                    | -3f, 1f, 0f -> goalFragments.Find(fun g -> g.Position = Vector3(-3f, 2f, 0f)).Visible <- true
                    | 5f, 1f, 4f -> goalFragments.Find(fun g -> g.Position = Vector3(5f, 2f, 4f)).Visible <- true
                    | -6f, 1f, 6f -> goalFragments.Find(fun g -> g.Position = Vector3(-6f, 2f, 6f)).Visible <- true
                    | _, _, _ -> ()
                | 15 ->
                    let plumes = [| LavaPlumeFS.lavaPlumes.Find(fun l -> (l.self.GetParent() :?> Node3D).Position = Vector3(3f, 0.5f, 2f))
                                    LavaPlumeFS.lavaPlumes.Find(fun l -> (l.self.GetParent() :?> Node3D).Position = Vector3(3f, 0.5f, 3f)) |]
                    for plume in plumes do
                        plume.active <- false
                        plume.self.Scale <- Vector3(1f, 20f, 1f)
                        (plume.self.GetParent() :?> Node3D).Visible <- true
                        let particles = plume.self.GetParent().GetNode<Node3D>("Particles")
                        particles.Position <- Vector3(particles.Position.X, 4f, particles.Position.Z)
                     
                    // Companion cube spawning
                    let scene = GD.Load<PackedScene>("res://Elements/CompanionCube.tscn")
                    let emt = scene.Instantiate() :?> Node3D
                    emt.Position <- Vector3(-5f, 1f, 5f)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild emt
                    companionCubes.Add emt
                    
                    // Extra terrain block addition
                    let blockScene = GD.Load<PackedScene>("res://Elements/VolcanicBlock.tscn")
                    let block = blockScene.Instantiate() :?> Node3D
                    block.Position <- Vector3(-4f, 1f, 0f)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
                    worlds[level][32] <- WorldFS.block -4f 1f 0f
                | 16 -> for frag in goalFragments do frag.Visible <- true
        
        let onDeactivated (other : Area3D) =
            match other.GetParent().Name.ToString() with
            | "CompanionCube" ->
                match level with
                | 9 -> if self.Position = Vector3(-2f, 1f, 4f) then bridges.ForEach(fun b -> b.Visible <- false)
                | 16 -> for frag in goalFragments do frag.Visible <- false
                | _ -> ()
            | _ -> ()
        
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered onActivated
            self.GetNode<Area3D>("Area3D").add_AreaExited onDeactivated
        
        member this.process (delta : float32) =
            if falling then
                let power = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("TerrainManipulator")
                if power.Position.Y >= 1f then
                    power.Translate(Vector3(0f, -delta * 4f, 0f))
                    
            // For invisble triggers
            if Array.contains Glasses PlayerFS.powerUps then
                if self.Position.DistanceTo PlayerFS.self.Position <= 3f then
                    self.Visible <- true
                else self.Visible <- false