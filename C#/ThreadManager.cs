using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

public class ThreadManager
{
    public readonly ConcurrentDictionary<string, (Thread Thread, CancellationTokenSource CTS)> Threads = new ConcurrentDictionary<string, (Thread, CancellationTokenSource)>();
    public List<string> ThreadDescriptions = new List<string>();

    public List<string> GetThreadsList()
    {
        ThreadDescriptions.Clear(); // Clear existing descriptions

        foreach (var kvp in Threads)
        {
            Debug.WriteLine("Found Thread.");
            var name = kvp.Key;
            var thread = kvp.Value.Thread;
            var isAlive = thread.IsAlive; // Determine if the thread is alive
            ThreadDescriptions.Add($"{name} - Alive: {isAlive}");
        }

        return ThreadDescriptions;
    }

    // Starts a new thread with a given name and action
    public bool AddThread(string name, Thread thread, CancellationTokenSource cts)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        Debug.WriteLine("Adding Existing Thread with CTS");
        return Threads.TryAdd(name, (thread, cts));
    }

    public bool AddThread(string name, Action action)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        Debug.WriteLine("Adding New Thread with private CTS");

        var cts = new CancellationTokenSource();
        ThreadStart threadStart = () => action.Invoke();
        var newThread = new Thread(threadStart) { Name = name };
        Threads.TryAdd(name, (newThread, cts));
        newThread.Start();

        return true;
    }

    public bool AddThread(string name, Action action, CancellationTokenSource cts)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        ThreadStart threadStart = () => action.Invoke();
        var thread = new Thread(threadStart) { Name = name };
        Threads.TryAdd(name, (thread, cts));
        thread.Start();
        return true;
    }

    public bool AddThread(string name, Action<CancellationToken> action, CancellationTokenSource cts)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        ThreadStart threadStart = () => action.Invoke(cts.Token);
        var thread = new Thread(threadStart) { Name = name };
        Threads.TryAdd(name, (thread, cts));
        thread.Start();
        return true;
    }

    // Adds a new thread that takes a client parameter and an action to perform
    public bool AddThread(string name, TcpClient client, Action<TcpClient, CancellationToken> action)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        Debug.WriteLine("Adding New Thread with client action");
        var cts = new CancellationTokenSource();
        ThreadStart threadStart = () => action.Invoke(client, cts.Token);
        var thread = new Thread(threadStart) { Name = name };
        Threads.TryAdd(name, (thread, cts));
        thread.Start();

        return true;
    }

    public bool AddThread(string name, object client, Action<object> action)
    {
        if (Threads.ContainsKey(name))
        {
            return false;
        }

        Debug.WriteLine("Adding New Thread with client action");
        var cts = new CancellationTokenSource();
        ThreadStart threadStart = () => action.Invoke(client);
        var thread = new Thread(threadStart) { Name = name };
        Threads.TryAdd(name, (thread, cts));
        thread.Start();

        return true;
    }

    // Attempts to stop a thread by name
    public void StopThread(string name)
    {
        if (Threads.ContainsKey(name))
        {
            var threadTuple = Threads[name];
            var targetThread = threadTuple.Thread;
            var cts = threadTuple.CTS;
            cts.Cancel(); // Signal cancellation to the thread

            // Optional: Wait a bit for the thread to finish
            if (!targetThread.Join(TimeSpan.FromSeconds(2)))
            {
                targetThread.Interrupt(); // Force the thread to stop
            }
            Threads.TryRemove(name, out var _);
        }
    }

    // Attempts to stop all threads
    public void StopAllThreads()
    {
        Debug.WriteLine("Stopping all threads.");
        foreach (var kvp in Threads.Keys)
        {
            Debug.WriteLine($"Stopping thread: {kvp}.");
            StopThread(kvp);
        }
    }
}
