Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Net.Sockets
Imports System.Threading

Public Class ThreadManager

  Public ReadOnly threads As New ConcurrentDictionary(Of String, (Thread As Thread, CTS As CancellationTokenSource))()
  Public threadDescriptions As New List(Of String)()

  Public Function GetThreadsList() As List(Of String)
  	threadDescriptions.Clear() ' Clear existing descriptions
  
  	For Each kvp In threads
  		Debug.WriteLine("Found Thread.")
  		Dim name = kvp.Key
  		Dim thread = kvp.Value.Thread
  		Dim isAlive = thread.IsAlive ' Determine if the thread is alive
  		threadDescriptions.Add($"{name} - Alive: {isAlive}")
  	Next
  
  	' Setting the DataSource to Nothing first forces the control to refresh
  	Return threadDescriptions
  End Function


	' Starts a new thread with a given name and action
	Public Function AddThread(name As String, thread As Thread, cts As CancellationTokenSource) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Debug.WriteLine("Adding Existing Thread with CTS")
		Return threads.TryAdd(name, (thread, cts))
	End Function

	Public Function AddThread(name As String, action As Action) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Debug.WriteLine("Adding New Thread with private CTS")

		Dim cts As New CancellationTokenSource()
		Dim threadStart As ThreadStart = Sub() action.Invoke()
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}
		threads.TryAdd(name, (thread, cts))
		thread.Start()

		Return True
	End Function

	Public Function AddThread(name As String, action As Action, cts As CancellationTokenSource) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Dim threadStart As ThreadStart = Sub() action.Invoke()
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}
		threads.TryAdd(name, (thread, cts))
		thread.Start()
	End Function

	Public Function AddThread(name As String, action As Action(Of CancellationToken), cts As CancellationTokenSource) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Dim threadStart As ThreadStart = Sub() action.Invoke(cts.Token)
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}
		threads.TryAdd(name, (thread, cts))
		thread.Start()
	End Function

	Public Function AddThread(name As String, action As Action(Of CancellationToken)) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Debug.WriteLine("Adding New Thread with CT action")
		Dim cts As New CancellationTokenSource()
		Dim threadStart As ThreadStart = Sub() action.Invoke(cts.Token)
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}

		threads.TryAdd(name, (thread, cts))
		thread.Start()

		Return True
	End Function

	' Adds a new thread that takes a client parameter and an action to perform
	Public Function AddThread(name As String, client As TcpClient, action As Action(Of TcpClient, CancellationToken)) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Debug.WriteLine("Adding New Thread with client action")
		Dim cts As New CancellationTokenSource()
		Dim threadStart As ThreadStart = Sub() action.Invoke(client, cts.Token)
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}

		threads.TryAdd(name, (thread, cts))
		thread.Start()

		Return True
	End Function

	Public Function AddThread(name As String, client As Object, action As Action(Of Object)) As Boolean
		If threads.ContainsKey(name) Then
			Return False
		End If

		Debug.WriteLine("Adding New Thread with client action")
		Dim cts As New CancellationTokenSource()
		Dim threadStart As ThreadStart = Sub() action.Invoke(client)
		Dim thread As New Thread(threadStart) With {
			.Name = name
		}

		threads.TryAdd(name, (thread, cts))
		thread.Start()

		Return True
	End Function

	' Attempts to stop a thread by name
	Public Sub StopThread(name As String)
		If threads.ContainsKey(name) Then
			Dim threadTuple = threads(name)
			Dim targetThread = threadTuple.Thread
			Dim cts = threadTuple.CTS
			cts.Cancel() ' Signal cancellation to the thread

			Try
				' Optional: Wait a bit for the thread to finish
				If Not targetThread.Join(TimeSpan.FromSeconds(2)) Then
					targetThread.Interrupt() ' Force the thread to stop
				End If
				threads.TryRemove(name, Nothing)
			Catch ex As Exception
			
			End Try
		Else

		End If
	End Sub

	' Attempts to stop all threads
	Public Sub StopAllThreads()
		Debug.WriteLine("Stopping all threads.")
		For Each kvp In threads
			Debug.WriteLine($"Stopping thread: {kvp.Key}.")
			StopThread(kvp.Key)
		Next
	End Sub
                    
End Class
