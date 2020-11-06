using System.Threading.Tasks;
using Grpc.Core;
using Service.Grpc;

namespace Service
{
  public class HelloService : Grpc.MyService.MyServiceBase
  {
    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
      var response = new HelloResponse();
      response.Message = "Hello " + request.Name;
      return Task.FromResult(response);
    }
  }
}