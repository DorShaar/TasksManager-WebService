using Tasker.Domain.Communication;
using Xunit;

namespace Tasker.Tests.Domain.Communication
{
    public class ResponseTests
    {
        [Fact]
        public void Ctor_Arguments_ResponseObject_ResponseMessage_AsExpected()
        {
            string responseObject = "ThisIsResponseObject";
            string responseMessage = "good";

            Response<string> response = new Response<string>(responseObject, true, responseMessage);

            Assert.Equal(responseObject, response.ResponseObject);
            Assert.True(response.IsSuccess);
            Assert.Equal(responseMessage, response.Message);
        }

        [Fact]
        public void Ctor_Arguments_ResponseMessage_AsExpected()
        {
            string responseMessage = "bad";

            Response<string> response = new Response<string>(false, responseMessage);

            Assert.False(response.IsSuccess);
            Assert.Equal(responseMessage, response.Message);
        }

        [Fact]
        public void Ctor_Arguments_ResponseObject_AsExpected()
        {
            string responseObject = "ThisIsResponseObject";

            Response<string> response = new Response<string>(responseObject, false);

            Assert.Equal(responseObject, response.ResponseObject);
            Assert.False(response.IsSuccess);
        }
    }
}