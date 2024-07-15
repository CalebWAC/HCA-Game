namespace FSharpScripts

open Godot
open GlobalFunctions

module UnpauseButtonFS =
    let mutable button = Unchecked.defaultof<Button>
   
    let unpause () =
        getRoot().GetNode<Control>("TutorialIntro").Visible <- false
        getRoot().GetTree().Paused <- false
    
    let ready () =
        button <- getRoot().GetNode<Control>("TutorialIntro").GetNode<Control>("Panel").GetNode<Button>("Button")
        button.add_Pressed unpause