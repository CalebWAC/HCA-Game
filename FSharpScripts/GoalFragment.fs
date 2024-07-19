namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFragmentFS =
    let mutable frags = 0
    let mutable change = 0.15f
    
    type Fragment() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onAreaEntered (other : Area3D) =
            if other.GetParent().Name.ToString() = "Player" then
                frags <- frags + 1
                if frags = 5 then
                    getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal").Visible <- true
                    GoalFS.ended <- true
                    
                    GoalFS.originalPos <- GoalFS.goal.GlobalPosition
                    GoalFS.endPos <-
                        let cameraPos = getRoot().GetNode<Node3D>("Camera").GetNode<Node3D>("EndPos")
                        cameraPos.GlobalPosition
                    GoalFS.midpoint <- (GoalFS.originalPos + GoalFS.endPos) / 2f
                    
                WorldGeneratorFS.goalFragments.Remove(self) |> ignore
                self.QueueFree()
        
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered onAreaEntered
            frags <- 0
            
        member this.physicsProcess delta =
            self.RotateX(degToRad 45f * delta)
            self.RotateZ(degToRad 45f * delta)