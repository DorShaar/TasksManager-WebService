using Tasker.Domain.Validators;
using Xunit;

namespace Tasker.Tests.Domain.Validators
{
    public class NameValidatorTests
    {
        [Fact]
        public void IsNameValid_NameIsValid_True()
        {
            NameValidator nameValidator = new NameValidator(10);
            Assert.True(nameValidator.IsNameValid("ValidName"));
        }

        [Fact]
        public void IsNameValid_NameIsNull_False()
        {
            NameValidator nameValidator = new NameValidator(5);
            Assert.False(nameValidator.IsNameValid(null));
        }

        [Fact]
        public void IsNameValid_NameIsEmpty_False()
        {
            NameValidator nameValidator = new NameValidator(5);
            Assert.False(nameValidator.IsNameValid(string.Empty));
        }

        [Fact]
        public void IsNameValid_NameIsLongerThanMaximalLength_False()
        {
            NameValidator nameValidator = new NameValidator(5);
            Assert.False(nameValidator.IsNameValid("LongName"));
        }

        [Fact]
        public void IsNameValid_NameHasInvalidCharacter_False()
        {
            NameValidator nameValidator = new NameValidator(5);
            Assert.False(nameValidator.IsNameValid("Invalid\'Name"));
        }
    }
}