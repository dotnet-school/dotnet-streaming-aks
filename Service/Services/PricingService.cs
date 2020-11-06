using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Service
{
  // Pricing is the name or interface in prices.proto
  // <service-name>.<service-name>Base class is auto generated
  public class PricingService : Pricing.PricingBase
  {

    public override async Task Subscribe(
            PriceRequest request, 
            IServerStreamWriter<PriceResponse> responseStream, 
            ServerCallContext context
    ){
      var i = 0;
      while (true)
      {
        // At every second, keep sending a fake response
        await Task.Delay(TimeSpan.FromSeconds(1));
        var quote = $"Quote#{++i} for {request.Uic}-{request.AssetType}";
        Console.WriteLine($"Sent: {quote}");
        var response = new PriceResponse{Quote = quote};
        await responseStream.WriteAsync(response);
      }
    }
  }
}