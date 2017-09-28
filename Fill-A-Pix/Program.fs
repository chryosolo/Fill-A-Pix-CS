module Fill_A_Pix.Program

open System
open System.Windows.Forms
open Akka.FSharp
open Fill_A_Pix_UI


[<EntryPoint>]
[<STAThread>]
let main argv =
    // set up WinForm environment
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault false

    // Form must get instantiated and drawn in order to start UI Thread
    let form = new FrmMain()

    // start ActorSystem and coordinator -- ui can now run from the UI Thread
    let system = System.create "system" (Configuration.defaultConfig())
    let uiDispatched = Dispatcher( "akka.actor.synchronized-dispatcher")
    let ui = spawnOpt system "ui" (UiActor.actor form) [uiDispatched]

    // tell form about its Actor
    form.SetActor( ui )

    do Application.Run form
    0
