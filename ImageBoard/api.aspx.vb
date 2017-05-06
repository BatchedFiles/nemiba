Imports System
Imports System.Reflection

Partial Class WebAPIHelpGenerator
	Inherits System.Web.UI.Page

	Public FullName As String
	Public Methods As String
	Public Types2 As String

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim t As Type = GetType(NemibaWebAPI)
		Dim a = t.GetCustomAttributesData()
		For i As Integer = 0 To a.Count - 1
			Types2 &= a(i).ToString & " * "
		Next
		For Each mm In t.GetMethods
			If mm.IsPublic AndAlso Not mm.IsSpecialName AndAlso Not mm.IsHideBySig Then
				Methods &= mm.Name & " " & mm.GetCustomAttributesData(0).ToString
			End If
		Next
	End Sub
End Class