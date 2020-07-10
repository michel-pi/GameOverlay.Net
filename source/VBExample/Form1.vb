Imports System.IO
Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
       
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim tskThread As New Task(StartAttach, TaskCreationOptions.LongRunning)
        tskThread.Start()
    End Sub

    Private Shared StartAttach As New Action(
 Sub()
     Dim ProcessName As String = Path.GetFileNameWithoutExtension(Application.ExecutablePath)
     Dim GameOVB As New GameOverlayVB(ProcessName)

 End Sub)

End Class
