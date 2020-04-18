using System;
using Tasker.Domain.Communication;
using Xunit;

namespace Tasker.Tests.Domain.Communication
{
    public class SuccessResponseTests
    {
        [Fact]
        public void Ctor_Arguments_ResponseObjectAndResponseMessage_AsExpected()
        {
            string responseObject = "ThisIsResponseObject";
            string responseMessage = "good";

            IResponse<string> response = new SuccessResponse<string>(responseObject, responseMessage);

            Assert.Equal(responseObject, response.ResponseObject);
            Assert.True(response.IsSuccess);
            Assert.Equal(responseMessage, response.Message);
        }

        [Fact]
        public void Ctor_Arguments_ResponseObjectIsNull_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(CreateSuccessResponseWithNullArgument);
        }

        private void CreateSuccessResponseWithNullArgument()
        {
            new SuccessResponse<string>(null);
        }
    }
}