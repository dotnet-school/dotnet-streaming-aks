using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Service;

namespace Client
{
  class Program
  {
    static void Main()
    {
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
      SayHello();
    }

    private static void SayHello()
    {
      using var channel = GrpcChannel.ForAddress("http://localhost:5000");
      
      var client = new Greeter.GreeterClient(channel);
      
      // HelloRequest is defined in hello.proto
      var request = new HelloRequest();
      request.Name = "Nishant";
      
      // SayHello method is defined in hello.proto
      var response = client.SayHello(request);
      
      Console.WriteLine(response.Message);
    }

  }
}