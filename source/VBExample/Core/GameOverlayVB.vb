Imports System.Runtime.InteropServices
Imports GameOverlay.Windows
Imports GameOverlay.Drawing
Imports System.Text

Public Class GameOverlayVB

#Region " Properties "


    Public Property IsClientVisible As Boolean
        Get
            Return overlay.IsVisible
        End Get
        Set(value As Boolean)
            overlay.IsVisible = value
        End Set
    End Property

    Public Property IsClientTopmost As Boolean
        Get
            Return overlay.IsTopmost
        End Get
        Set(value As Boolean)
            overlay.IsTopmost = value
        End Set
    End Property

    Public Property IsClientRunning As Boolean
        Get
            Return overlay.IsRunning
        End Get
        Set(value As Boolean)
            overlay.IsRunning = value
        End Set
    End Property

    Public Property IsClientPaused As Boolean
        Get
            Return overlay.IsPaused
        End Get
        Set(value As Boolean)
            overlay.IsPaused = value
        End Set
    End Property

    Public ReadOnly Property IsClientInitialized As Boolean
        Get
            Return overlay.IsInitialized
        End Get
    End Property

    Public ReadOnly Property GetFPS As Integer
        Get
            Return overlay.FPS
        End Get
    End Property

    Public ReadOnly Property GetWindowsMenuName As String
        Get
            Return overlay.MenuName
        End Get
    End Property

    Public ReadOnly Property GetWindowsTitle As String
        Get
            Return overlay.Title
        End Get
    End Property

    Public ReadOnly Property GetWindowsCoordinates As System.Drawing.Point
        Get
            Return New System.Drawing.Point(overlay.X, overlay.Y)
        End Get
    End Property

    Public ReadOnly Property GetWindowsSize As System.Drawing.Size
        Get
            Return New System.Drawing.Size(overlay.Width, overlay.Height)
        End Get
    End Property

#End Region

    Private WindowsGHandle As IntPtr = Nothing
    Public overlay As StickyWindow
    Private _brushes As Dictionary(Of String, SolidBrush)
    Private _fonts As Dictionary(Of String, Font)
    Private _images As Dictionary(Of String, Image)
    Private _gridGeometry As Geometry
    Private _gridBounds As Rectangle
    Private _random As Random
    Private _lastRandomSet As Long
    Private _randomFigures As List(Of Action(Of Graphics, Single, Single))
    Public WinHelpersEx As New WinHelpersCore

    Public Enum OverlayMethods
        StickyWindow = 1
        GraphicsWindow = 2
    End Enum

    Sub New(ByVal ProcessName As String)
        WindowsGHandle = WinHelpersEx.GetProcessHandle(ProcessName).ToInt32()
        overlay = New StickyWindow(WindowsGHandle, Nothing)

        If Not WindowsGHandle = Nothing Then

            _brushes = New Dictionary(Of String, SolidBrush)()
            _fonts = New Dictionary(Of String, Font)()
            _images = New Dictionary(Of String, Image)()

            Dim gfx = New Graphics() With {
                .MeasureFPS = True,
                .PerPrimitiveAntiAliasing = True,
                .TextAntiAliasing = True
            }
          
            overlay = New StickyWindow(0, 0, 800, 600, WindowsGHandle, Nothing) With {
                               .BypassTopmost = True,
                               .IsTopmost = True,
                               .IsVisible = True
                     }
         

            AddHandler overlay.DestroyGraphics, AddressOf _overlay_DestroyGraphics
            AddHandler overlay.DrawGraphics, AddressOf _overlay_DrawGraphics
            AddHandler overlay.SetupGraphics, AddressOf _overlay_SetupGraphics

            overlay.Create()
            overlay.Join()
        End If
    End Sub



#Region " Events "

    Private Sub _overlay_SetupGraphics(ByVal sender As Object, ByVal e As SetupGraphicsEventArgs)
        Dim gfx = e.Graphics

        If e.RecreateResources Then

            For Each pair In _brushes
                pair.Value.Dispose()
            Next

            For Each pair In _images
                pair.Value.Dispose()
            Next
        End If

        _brushes("black") = gfx.CreateSolidBrush(0, 0, 0)
        _brushes("white") = gfx.CreateSolidBrush(255, 255, 255)
        _brushes("red") = gfx.CreateSolidBrush(255, 0, 0)
        _brushes("green") = gfx.CreateSolidBrush(0, 255, 0)
        _brushes("blue") = gfx.CreateSolidBrush(0, 0, 255)
        _brushes("background") = gfx.CreateSolidBrush(&H33, &H36, &H3F)
        _brushes("grid") = gfx.CreateSolidBrush(255, 255, 255, 0.2F)
        _brushes("random") = gfx.CreateSolidBrush(0, 0, 0)
        If e.RecreateResources Then Return
        _fonts("arial") = gfx.CreateFont("Arial", 12)
        _fonts("consolas") = gfx.CreateFont("Consolas", 14)
        _gridBounds = New Rectangle(20, 60, gfx.Width - 20, gfx.Height - 20)
        _gridGeometry = gfx.CreateGeometry()

        For x As Single = _gridBounds.Left To _gridBounds.Right Step 20
            Dim line = New Line(x, _gridBounds.Top, x, _gridBounds.Bottom)
            _gridGeometry.BeginFigure(line)
            _gridGeometry.EndFigure(False)
        Next

        For y As Single = _gridBounds.Top To _gridBounds.Bottom Step 20
            Dim line = New Line(_gridBounds.Left, y, _gridBounds.Right, y)
            _gridGeometry.BeginFigure(line)
            _gridGeometry.EndFigure(False)
        Next

        _gridGeometry.Close()
        _randomFigures = New List(Of Action(Of Graphics, Single, Single))() From {
          Sub(g, x, y) g.DrawRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110, 2.0F),
          Sub(g, x, y) g.DrawCircle(GetRandomColor(), x + 60, y + 60, 48, 2.0F),
          Sub(g, x, y) g.DrawRoundedRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110, 8.0F, 2.0F),
          Sub(g, x, y) g.DrawTriangle(GetRandomColor(), x + 10, y + 110, x + 110, y + 110, x + 60, y + 10, 2.0F),
          Sub(g, x, y) g.DashedRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110, 2.0F),
          Sub(g, x, y) g.DashedCircle(GetRandomColor(), x + 60, y + 60, 48, 2.0F),
          Sub(g, x, y) g.DashedRoundedRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110, 8.0F, 2.0F),
          Sub(g, x, y) g.DashedTriangle(GetRandomColor(), x + 10, y + 110, x + 110, y + 110, x + 60, y + 10, 2.0F),
          Sub(g, x, y) g.FillRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110),
          Sub(g, x, y) g.FillCircle(GetRandomColor(), x + 60, y + 60, 48),
          Sub(g, x, y) g.FillRoundedRectangle(GetRandomColor(), x + 10, y + 10, x + 110, y + 110, 8.0F),
          Sub(g, x, y) g.FillTriangle(GetRandomColor(), x + 10, y + 110, x + 110, y + 110, x + 60, y + 10)
      }

    End Sub

    Private Sub _overlay_DestroyGraphics(ByVal sender As Object, ByVal e As DestroyGraphicsEventArgs)
        For Each pair In _brushes
            pair.Value.Dispose()
        Next

        For Each pair In _fonts
            pair.Value.Dispose()
        Next

        For Each pair In _images
            pair.Value.Dispose()
        Next
    End Sub

    Private Sub _overlay_DrawGraphics(ByVal sender As Object, ByVal e As DrawGraphicsEventArgs)
        Dim gfx = e.Graphics
        Dim padding = 16

        Dim MauseGamePoint As System.Drawing.Point = WinHelpersEx.GetClientCursorPosition(WindowsGHandle)


        Dim infoText = New StringBuilder().Append("Mause Pos: ").Append(Convert.ToString(MauseGamePoint.X) & ";" & Convert.ToString(MauseGamePoint.Y)).Append("  FrameTime: ").Append(e.FrameTime.ToString().PadRight(padding)).Append("FrameCount: ").Append(e.FrameCount.ToString().PadRight(padding)).Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding)).ToString()
        gfx.ClearScene(Color.Transparent)
        gfx.DrawTextWithBackground(_fonts("consolas"), _brushes("green"), _brushes("black"), 20, 20, infoText)
        gfx.DrawGeometry(_gridGeometry, _brushes("grid"), 1.0F)
        gfx.DrawCrosshair(_brushes("green"), New Point(300, 300), 5, 5, CrosshairStyle.Cross)

        If _lastRandomSet = 0L OrElse e.FrameTime - _lastRandomSet > 2500 Then
            _lastRandomSet = e.FrameTime
        End If
        _random = New Random(CInt(Math.Truncate(_lastRandomSet)))

        If _lastRandomSet = 0L OrElse e.FrameTime - _lastRandomSet > 2500 Then
            _lastRandomSet = e.FrameTime
        End If
        _random = New Random(CInt(Math.Truncate(_lastRandomSet)))

        For row As Single = _gridBounds.Top + 12 To _gridBounds.Bottom - 120 - 1 Step 120

            For column As Single = _gridBounds.Left + 12 To _gridBounds.Right - 120 - 1 Step 120
                DrawRandomFigure(gfx, column, row)
            Next
        Next
    End Sub

    Public Function GetRandomColor() As SolidBrush
        Dim brush = _brushes("random")
        brush.Color = New Color(_random.[Next](0, 256), _random.[Next](0, 256), _random.[Next](0, 256))
        Return brush
    End Function

#End Region

#Region " Publics "

    Public Sub Move(ByVal PointC As System.Drawing.Point)
        overlay.Move(PointC.X, PointC.Y)
    End Sub

    Public Sub Resize(ByVal SizeC As System.Drawing.Size)
        overlay.Resize(SizeC.Width, SizeC.Height)
    End Sub

    Private Sub DrawFigure(ByVal gfx As Graphics, ByVal x As Single, ByVal y As Single)
        gfx.FillRectangle(_brushes("black"), x + 10, y + 10, x + 110, y + 110)
    End Sub

    Private Sub DrawRandomFigure(ByVal gfx As Graphics, ByVal x As Single, ByVal y As Single)
        Dim action = _randomFigures(_random.[Next](0, _randomFigures.Count))
        action(gfx, x, y)
    End Sub

#End Region

End Class
