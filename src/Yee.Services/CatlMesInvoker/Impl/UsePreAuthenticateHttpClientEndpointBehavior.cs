using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Yee.Services.CatlMesInvoker
{
    public class UsePreAuthenticateHttpClientEndpointBehavior : IEndpointBehavior
    {
        public UsePreAuthenticateHttpClientEndpointBehavior(string User, string Password)
        {
            this.User = User;
            this.Pass = Password;
        }

        public string User { get; }
        public string Pass { get; }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            bindingParameters.Add(new Func<HttpClientHandler, HttpMessageHandler>(httpClientHandler => new InterceptingHttpMessageHandler(httpClientHandler, this)));
        }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
        public void Validate(ServiceEndpoint endpoint) { }

        public HttpResponseMessage? PreSending(HttpRequestMessage req, CancellationToken ct)
        {
            var cred = $"{User}:{Pass}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            var authorization = new AuthenticationHeaderValue("Basic", base64String);
            req.Headers.ExpectContinue = false;
            req.Headers.Authorization = authorization;
            return null;
        }

        class InterceptingHttpMessageHandler : DelegatingHandler
        {
            private readonly UsePreAuthenticateHttpClientEndpointBehavior _parent;

            public InterceptingHttpMessageHandler(HttpMessageHandler innerHandler, UsePreAuthenticateHttpClientEndpointBehavior parent)
            {
                InnerHandler = innerHandler;
                _parent = parent;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = _parent.PreSending(request, cancellationToken);
                if (response != null)
                    return response;

                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
        }
    }



}
