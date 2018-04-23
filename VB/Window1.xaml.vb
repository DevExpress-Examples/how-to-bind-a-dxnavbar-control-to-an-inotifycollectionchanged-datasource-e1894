Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.Windows
Imports System.Windows.Threading
Imports DevExpress.Wpf.NavBar

Namespace NavBar_DataBinding
	Partial Public Class Window1
		Inherits Window

		Private groups As ObservableCollection(Of NavBarGroup)

		Public Sub New()
			InitializeComponent()

			groups = New ObservableCollection(Of NavBarGroup)()
			For i As Integer = 0 To 4
				groups.Add(New NavBarGroup() With {.Header = "Group" & count})
				count += 1
			Next i

			DataContext = groups

			Dim timer As New DispatcherTimer()
			timer.Interval = TimeSpan.FromMilliseconds(200)
			AddHandler timer.Tick, AddressOf timer_Tick
			timer.Start()
		End Sub

		Private count As Integer = 0
		Private random As New Random()

		Private Sub timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
			If (random.Next(2) = 1 OrElse groups.Count = 0) AndAlso groups.Count < 10 Then
				groups.Add(New NavBarGroup() With {.Header = "Group" & count})
				count += 1
			Else
				groups.RemoveAt(groups.Count - 1)
			End If
		End Sub
	End Class
End Namespace
