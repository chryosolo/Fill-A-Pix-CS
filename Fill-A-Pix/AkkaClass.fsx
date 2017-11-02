#I __SOURCE_DIRECTORY__
#r "../packages/Akka.1.2.3/lib/net45/Akka.dll"
#r "../packages/Akka.FSharp.1.2.3/lib/net45/Akka.FSharp.dll"
#r "../packages/FsPickler.3.2.0/lib/net45/FsPickler.dll"

open Akka.Actor
open Akka.FSharp

type MultiActor() =
    inherit UntypedActor()
        override this.OnReceive (msg:obj) =
            match msg with
            | :? int as msg' ->
                match msg' with
                | x when x < 0 -> printfn "%d is neg int!" x
                | _ -> printfn "%d must be negative :(" msg'
            | :? bool as msg' ->
               printfn "Received bool %b" msg'
            | _ ->
               printfn "Received unknown message type %A" msg
               this.Unhandled msg
                        
let system = System.create "system" <| Configuration.defaultConfig()
let multi = system.ActorOf(Props(typedefof<MultiActor>), "multi")

multi.Tell -4
multi.Tell 6
multi.Tell false
multi.Tell "Hello World!"

let handleInt i =
   match i with
   | x when x < 0 -> printfn "%d is neg int!" x
   | _ -> printfn "%d must be negative :(" i

type AbsCounterActor () as this =
   inherit ReceiveActor () // Still untyped

   do
      this.Receive<int>( handleInt )
      this.Receive<bool>( fun b -> printfn "Received bool %b" b )

   override this.Unhandled(msg) =
      printfn "Received unknown message type %A" msg

let absCounter = system.ActorOf(Props(typedefof<AbsCounterActor>), "absCounter")

absCounter.Tell 4
absCounter.Tell -2
absCounter.Tell false


type countMsg =
   | Die
   | Incr of int
   | Fetch

let counter (self:Actor<countMsg>) =
   let rec count total = actor {
      let! msg = self.Receive()
      match msg with
      | Die -> self.Context.Self <! PoisonPill.Instance
      | Incr x -> return! count (total + x)
      | Fetch -> self.Context.Sender <! total
      return! count total
   }
   count 0

let aCounter = spawn system "counter" counter

let waitFor (task:Async<int>) =
   Async.RunSynchronously( task, 1000)

aCounter <? Fetch |> waitFor