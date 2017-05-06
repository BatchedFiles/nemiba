Partial Public Class SQLiteBoardDatabaseProvider

	Public Overrides Function AddRoleFromAnswersThreadDeny(ByVal ThreadNumber As Long, ByVal RoleName As String) As ErrorInfo
		Return New ErrorInfo
	End Function

	Public Overrides Function GetRolesFromAnswersThreadDenyList(ByVal ThreadNumber As Long) As System.Collections.Generic.List(Of DenyRolesForThreadAnswersInfo)
		Return New List(Of DenyRolesForThreadAnswersInfo)
	End Function

	Public Overrides Function RemoveRoleFromAnswersThreadDeny(ByVal ThreadNumber As Long, ByVal RoleName As String) As ErrorInfo
		Return New ErrorInfo
	End Function

End Class
