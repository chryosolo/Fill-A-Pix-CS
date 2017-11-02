#r "C:\\git\\MySandbox\\Fill-A-Pix\\packages\\Akka.1.2.3\\lib\\net45\\Akka.dll"
#r "C:\\git\\MySandbox\\Fill-A-Pix\\packages\\Akka.FSharp.1.2.3\\lib\\net45\\Akka.FSharp.dll"

open Akka.Actor
open Akka.FSharp

type CounterActor() =
    inherit UntypedActor()
        override this.OnReceive (msg:obj) =
            match msg with
            | :? int as msg' ->
                match msg' with
                | x when x < 0 -> printfn "Neg int!"
            printfn "Received message %A" msg               
                        
let system = System.create "system" <| Configuration.defaultConfig()
                        
// Use Props() to create actor from type definition
let echoActor = system.ActorOf(Props(typedefof<CounterActor>), "echo")

// tell a message
echoActor <! "Hello World!"
