Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Windows
Imports DevExpress.Wpf.NavBar

Namespace NavBar_DataBinding

	Public Class NavBarDataBindingHelper
		Inherits DependencyObject
		Public Shared ReadOnly ItemsSourceProperty As DependencyProperty

		Shared Sub New()
			ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", GetType(IEnumerable), GetType(NavBarDataBindingHelper), New FrameworkPropertyMetadata(Nothing, AddressOf OnItemsSourceChanged))
		End Sub

		Public Shared Sub SetItemsSource(ByVal element As DependencyObject, ByVal value As IEnumerable(Of NavBarGroup))
			element.SetValue(NavBarDataBindingHelper.ItemsSourceProperty, value)
		End Sub

		Public Shared Function GetItemsSource(ByVal element As DependencyObject) As IEnumerable
			Return CType(element.GetValue(NavBarDataBindingHelper.ItemsSourceProperty), IEnumerable)
		End Function

		Private Shared navBars As New Dictionary(Of NavBarControl, Binder)()

		Private Shared Sub OnItemsSourceChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			Dim navBar As NavBarControl = TryCast(d, NavBarControl)
			If navBar Is Nothing Then
				Return
			End If
			If navBars.ContainsKey(navBar) Then
				navBars(navBar).UnsubscribeEventsIfNeeded()
				navBars.Remove(navBar)
			End If
			If e.NewValue IsNot Nothing Then
				navBars(navBar) = New Binder(navBar, NavBarDataBindingHelper.GetItemsSource(navBar))
			End If
		End Sub
	End Class

	Friend Class Binder
		Private navBar As NavBarControl
		Private collection As INotifyCollectionChanged

		Public Sub New(ByVal navBar As NavBarControl, ByVal source As IEnumerable)
			Me.navBar = navBar

			navBar.Groups.Clear()
			For Each group As NavBarGroup In source
				navBar.Groups.Add(group)
			Next group

			collection = TryCast(source, INotifyCollectionChanged)
			If collection IsNot Nothing Then
				AddHandler collection.CollectionChanged, AddressOf BindingHelper_CollectionChanged
			End If
		End Sub

		Public Sub UnsubscribeEventsIfNeeded()
			If collection IsNot Nothing Then
				RemoveHandler collection.CollectionChanged, AddressOf BindingHelper_CollectionChanged
			End If
		End Sub

		Private Sub BindingHelper_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
			If e.NewItems IsNot Nothing Then
				For Each group As NavBarGroup In e.NewItems
					navBar.Groups.Add(group)
				Next group
			End If
			If e.OldItems IsNot Nothing Then
				For Each group As NavBarGroup In e.OldItems
					navBar.Groups.Remove(group)
				Next group
			End If
		End Sub
	End Class
End Namespace