using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR;
using Service;

namespace StreamWebService
{
  public class PricingHub : Hub
  {
    
    public async IAsyncEnumerable<string> Subscribe(
            string uic,
            string assetType,
            [EnumeratorCancellation]
            CancellationToken cancellationToken)
    {
      var url = Environment.GetEnvironmentVariable("PRICING_STREAM_ENDPOINT");
      using var channel = GrpcChannel.ForAddress(url);

      var client = new Pricing.PricingClient(channel);
      var request = new PriceRequest{Uic = uic, AssetType = assetType};
      
      var streamReader = client.Subscribe(request).ResponseStream;
      yield return $"Info: Opened channel to : {url}";

      yield return "Info: Waiting for data from gRPC server";

      while (await streamReader.MoveNext())
      {
        cancellationToken.ThrowIfCancellationRequested();
        yield return $"Data: {streamReader.Current}";
      }

      Console.WriteLine("Gracefully ended.");
    }
  }
}