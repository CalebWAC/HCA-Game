namespace FSharpScripts

open Godot
open GlobalFunctions

module UnpauseButtonFS =
    type UnpauseButton(name: string) =
        let mutable button = Unchecked.defaultof<Button>
       
        let unpause () =
            getRoot().GetNode<Control>(name).Visible <- false
            getRoot().GetTree().Paused <- false
        
        member this.ready () =
            button <- getRoot().GetNode<Control>(name).GetNode<Control>("Panel").GetNode<Button>("Button")
            button.add_Pressed unpause