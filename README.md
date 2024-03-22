# ThreadManager Documentation

The `ThreadManager` class is designed to facilitate thread creation, management, stopping, and cleanup in a .NET application. This comprehensive guide will help you utilize the `ThreadManager` effectively in your projects.

## Getting Started

### Start the Thread Manager

Optimally initialize the `ThreadManager` object in your main form or, better yet, in a module so you can refer to it globally. This ensures that thread management is accessible throughout your application.

```vb.net
Public SystemThreadManager As New ThreadManager()
```

### Create a Thread

To create a new thread, assign it a name and the address of the function that will run in the thread. This approach allows for easy identification and management of threads.

Example:
```vb.net
SystemThreadManager.AddThread("Thread1", new Thread(Sub() ThreadMethod()), New CancellationTokenSource())
```

Ensure to import the necessary namespaces at the beginning of your file:

```vb.net
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Net.Sockets
Imports System.Threading
```

## Class Overview

### Properties

- **threads**: A concurrent dictionary that stores thread information, indexed by thread name.
- **threadDescriptions**: A list that holds descriptions of all threads for display or logging purposes.

### Methods

- **GetThreadsList()**: Retrieves a list of thread descriptions, indicating whether each thread is alive.
- **AddThread(name As String, thread As Thread, cts As CancellationTokenSource) As Boolean**: Adds a new thread with a specified name, thread object, and cancellation token source.
- **AddThread(name As String, action As Action) As Boolean**: Overload for adding a thread by specifying an action to be executed in the thread.
- **StopThread(name As String)**: Attempts to stop a thread by its name, gracefully ending its execution if possible.
- **StopAllThreads()**: Stops all threads managed by the `ThreadManager`, ensuring a clean shutdown of all background activities.

## Usage Examples

### Stopping a Thread

To stop a thread, simply call the `StopThread` method with the name of the thread you wish to stop:

```vb.net
SystemThreadManager.StopThread("Thread1")
```

### Stopping All Threads

To ensure that all threads are properly stopped, especially upon application exit, use the `StopAllThreads` method:

```vb.net
SystemThreadManager.StopAllThreads()
```

## Conclusion

The `ThreadManager` class provides a structured approach to managing threads within .NET applications, making it easier to start, monitor, and terminate threads as required. By following the guidelines and examples provided, you can effectively manage thread lifecycles and ensure your application runs smoothly and efficiently.
