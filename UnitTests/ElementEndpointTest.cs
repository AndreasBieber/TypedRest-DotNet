using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using RichardSzalay.MockHttp;
using Xunit;

namespace TypedRest
{
    public class ElementEndpointTest : EndpointTestBase
    {
        private readonly IElementEndpoint<MockEntity> _endpoint;

        public ElementEndpointTest() => _endpoint = new ElementEndpoint<MockEntity>(EntryEndpoint, "endpoint");

        [Fact]
        public async Task TestRead()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(JsonMime, "{\"Id\":5,\"Name\":\"test\"}");

            var result = await _endpoint.ReadAsync();
            result.Should().Be(new MockEntity(5, "test"));
        }

        [Fact]
        public async Task TestReadCache()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result1 = await _endpoint.ReadAsync();
            result1.Should().Be(new MockEntity(5, "test"));

            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .WithHeaders("If-None-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NotModified);
            var result2 = await _endpoint.ReadAsync();
            result2.Should().Be(new MockEntity(5, "test"));

            result2.Should().NotBeSameAs(result1,
                because: "Cache responses, not deserialized objects");
        }

        [Fact]
        public async Task TestExistsTrue()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(HttpStatusCode.OK);

            var result = await _endpoint.ExistsAsync();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task TestExistsFalse()
        {
            Mock.Expect(HttpMethod.Head, "http://localhost/endpoint")
                .Respond(HttpStatusCode.NotFound);

            var result = await _endpoint.ExistsAsync();
            result.Should().BeFalse();
        }

        [Fact]
        public async Task TestSetResult()
        {
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(JsonMime, "{\"Id\":5,\"Name\":\"testXXX\"}");

            var result = await _endpoint.SetAsync(new MockEntity(5, "test"));
            result.Should().Be(new MockEntity(5, "testXXX"));
        }

        [Fact]
        public async Task TestSetNoResult()
        {
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.SetAsync(new MockEntity(5, "test"));
        }

        [Fact]
        public async Task TestSetETag()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"123abc\"")}
                });
            var result = await _endpoint.ReadAsync();

            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .WithHeaders("If-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NoContent);
            await _endpoint.SetAsync(result);
        }

        [Fact]
        public async Task TestUpdateRetry()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test1\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"1\"")}
                });
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"testX\"}")
                .WithHeaders("If-Match", "\"1\"")
                .Respond(HttpStatusCode.PreconditionFailed);
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test2\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"2\"")}
                });
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"testX\"}")
                .WithHeaders("If-Match", "\"2\"")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.UpdateAsync(x => x.Name = "testX");
        }

        [Fact]
        public async Task TestUpdateFail()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test1\"}", Encoding.UTF8, JsonMime),
                    Headers = {ETag = new EntityTagHeaderValue("\"1\"")}
                });
            Mock.Expect(HttpMethod.Put, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"testX\"}")
                .WithHeaders("If-Match", "\"1\"")
                .Respond(HttpStatusCode.PreconditionFailed);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _endpoint.UpdateAsync(x => x.Name = "testX", maxRetries: 0));
        }

        [Fact]
        public async Task TestMergeResult()
        {
            Mock.Expect(HttpClientExtensions.Patch, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(JsonMime, "{\"Id\":5,\"Name\":\"testXXX\"}");

            var result = await _endpoint.MergeAsync(new MockEntity(5, "test"));
            result.Should().Be(new MockEntity(5, "testXXX"));
        }

        [Fact]
        public async Task TestMergeNoResult()
        {
            Mock.Expect(HttpClientExtensions.Patch, "http://localhost/endpoint")
                .WithContent("{\"Id\":5,\"Name\":\"test\"}")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.MergeAsync(new MockEntity(5, "test"));
        }

        [Fact]
        public async Task TestDelete()
        {
            Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.DeleteAsync();
        }

        [Fact]
        public async Task TestDeleteETag()
        {
            Mock.Expect(HttpMethod.Get, "http://localhost/endpoint")
                .Respond(new HttpResponseMessage
                {
                    Content = new StringContent("{\"Id\":5,\"Name\":\"test\"}", Encoding.UTF8, JsonMime),
                    Headers = { ETag = new EntityTagHeaderValue("\"123abc\"") }
                });
            await _endpoint.ReadAsync();

            Mock.Expect(HttpMethod.Delete, "http://localhost/endpoint")
                .WithHeaders("If-Match", "\"123abc\"")
                .Respond(HttpStatusCode.NoContent);

            await _endpoint.DeleteAsync();
        }
    }
}