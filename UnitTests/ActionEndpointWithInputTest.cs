﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class ActionEndpointWithInputTest : EndpointTestBase
    {
        private readonly IActionEndpoint<MockEntity> _endpoint;

        public ActionEndpointWithInputTest() => _endpoint = new ActionEndpoint<MockEntity>(EntryEndpoint, "endpoint");

        [Fact]
        public async Task TestTrigger()
        {
            Mock.Expect(HttpMethod.Post, "http://localhost/endpoint")
                .WithContent("{\"Id\":1,\"Name\":\"input\"}")
                .Respond(HttpStatusCode.Accepted);

            await _endpoint.TriggerAsync(new MockEntity(1, "input"));
        }
    }
}