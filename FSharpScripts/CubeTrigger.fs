namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module CubeTriggerFS =
    type CubeTrigger() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable falling = false // For Level 11's falling terrain manipulator
        
        let onActivated (other : Area3D) =
            if other.GetParent().IsInGroup(new StringName "cubes") then
                match level with
                | 9 -> WorldGeneratorFS.bridges.ForEach(fun b -> b.Visible <- true)
                | 10 ->
                    let scene = GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
                    let power = scene.Instantiate() :?> Node3D
                    power.Position <- Vector3(1f, 6f, 6f)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
                    falling <- true
                | _ -> ()
        
        let onDeactivated (other : Area3D) =
            match other.GetParent().Name.ToString() with
            | "CompanionCube" ->
                match level with
                | 9 -> if self.Position = Vector3(-2f, 1f, 4f) then WorldGeneratorFS.bridges.ForEach(fun b -> b.Visible <- false)
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