using Tasker.Domain.Communication;
using Xunit;

namespace Tasker.Tests.Domain.Communication
{
    public class FailResponseTests
    {
        [Fact]
        public void Ctor_Arguments_ResponseObjectAndResponseMessage_AsExpected()
        {
            const string responseObject = "ThisIsResponseObject";
            const string responseMessage = "good";

            IResponse<string> response = new FailResponse<string>(responseObject, responseMessage);

            Assert.Equal(responseObject, response.ResponseObject);
            Assert.False(response.IsSuccess);
            Assert.Equal(responseMessage, response.Message);
        }

        [Fact]
        public void Ctor_Arguments_ResponseMessage_AsExpected()
        {
            const string responseMessage = "bad";

            IResponse<string> response = new FailResponse<string>(responseMessage);

            Assert.False(response.IsSuccess);
            Assert.Equal(responseMessage, response.Message);
        }
    }
}