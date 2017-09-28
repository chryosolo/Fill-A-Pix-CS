module Fill_A_Pix.GameActor

open Akka.FSharp
open Akka.Actor
open System.Drawing
open GameTypes
open Messages
open System.Reflection

let actor (self:Actor<'a>) =
    let coord = self.Context.Parent
    let rec loop() = actor {
        let! msg = self.Receive()
        match msg with
        | _ -> self.Unhandled msg
        return! loop ()
    }
    loop()
