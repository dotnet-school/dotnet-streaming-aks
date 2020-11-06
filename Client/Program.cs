using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Core;
using Service;

namespace Client
{
  class Program
  {
    static async Task Main()
    {
      AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
      SayHello();
      await SubscribeToStream();
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
    
    private static async Task SubscribeToStream()
    {
      using var channel = GrpcChannel.ForAddress("http://localhost:5000");
      
      var client = new Pricing.PricingClient(channel);
      var request = new PriceRequest{Uic = "211", AssetType = "Stock"};
      
      var streamReader = client.Subscribe(request).ResponseStream;
      
      while (await streamReader.MoveNext())
      {
        Console.WriteLine($"Received: {streamReader.Current}");
      }
      
      Console.WriteLine("Gracefully ended.");
    }

  }
}