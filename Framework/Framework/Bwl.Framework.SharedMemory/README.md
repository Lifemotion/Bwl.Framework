# Bwl.Framework.SharedMemory

**Bwl.Framework.SharedMemory** provides high-performance inter-process communication using shared memory. It includes two main communication models:

- **Binary Communication:** Uses `SharedMemoryBinaryClient` and `SharedMemoryBinaryServer` to exchange raw binary data.
- **RPC Communication:** Uses `SharedMemoryRPCClient<T>` and `SharedMemoryRPCServer<T>` to perform remote procedure calls over shared memory.

## Features

- **Efficient Data Transfer:** Leverages memory mapped files and low-level pointer operations for high throughput.
- **Configurable Synchronization:** Uses event wait handles to coordinate client/server communication.
- **RPC Support:** Simplifies remote method invocation using MessagePack-based serialization.

## Usage Examples

### Binary Communication

Below is an example of a binary server that processes incoming data by echoing it back, with a corresponding client that sends a request.

``` csharp
using System;
using System.Text;
using Bwl.Framework.SharedMemory;
class BinaryExample
{
    static void Main(string[] args)
    { 
        // Create a server that simply echoes the received data. 
        using (var server = new SharedMemoryBinaryServer(processFunc: request => request, // Echo the incoming data 
        id: "BinaryComm"
        ))
        {
            // Create a client to send data. 
            using (var client = new SharedMemoryBinaryClient("BinaryComm"))
            {
                byte[] requestData = Encoding.UTF8.GetBytes("Hello, Shared Memory!");
                byte[] responseData = client.ProcessData(requestData);
                Console.WriteLine("Received Response: " + Encoding.UTF8.GetString(responseData));
            }
        }
    }
}
```

### RPC Communication

This example demonstrates an RPC server and client setup. An interface is defined, the server hosts an implementation of that interface, and the client makes remote procedure calls.

``` csharp
using System;
using Bwl.Framework.SharedMemory;

// Define the RPC interface. 
public interface IMyService
{
    string Echo(string message);
}

// Implement the service. 
public class MyService : IMyService
{
    public string Echo(string message)
    {
        return $"Echo: {message}";
    }
}

class RPCExample
{
    static void Main(string[] args)
    {
        // Start the RPC server with a MyService implementation. 
        using (var rpcServer = new SharedMemoryRPCServer<IMyService>(implementation: new MyService(), id: "MyService"))
        {
            // Create a client proxy for IMyService. 
            var rpcClient = SharedMemoryRPCClient<IMyService>.Create("MyService");
            // Invoke the remote method.
            string response = rpcClient.Echo("Hello RPC!");
            Console.WriteLine("RPC Response: " + response);
            rpcClient.Dispose();
        }
    }
}
```

## Important Notes

- **Shared Identifier (ID):**  
  Clients and servers must use the same identifier (ID) to access the same shared memory segment and event handles.

- **Memory Capacity:**  
  The capacity of the memory mapped file is configurable. The server warms up memory pages during initialization to reduce latency during data transfer.

- **Timeouts:**  
  Adjust the `Timeout` property on client instances to configure how long they wait for a response.