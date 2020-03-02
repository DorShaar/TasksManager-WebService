using System.IO;
using System.Linq;

namespace Tasker.Domain.Validators
{
    public class NameValidator
    {
        private readonly int mMaximalNameLength;

        public NameValidator(int maximalLength)
        {
            mMaximalNameLength = maximalLength;
        }

        public bool IsNameValid(string name)
        {
            if (name.Length > mMaximalNameLength)
                return false;

            if (GetInvalidFileAndPathCharacters().Any(ch => name.Contains(ch)))
                return false;

            return true;
        }

        private char[] GetInvalidFileAndPathCharacters()
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            char[] invalidPathChars = Path.GetInvalidPathChars();

            char[] invalidFileAndPathCharacters = new char[invalidFileNameChars.Length + invalidPathChars.Length];
            invalidFileNameChars.CopyTo(invalidFileAndPathCharacters, 0);
            invalidPathChars.CopyTo(invalidFileAndPathCharacters, invalidFileNameChars.Length);

            return invalidFileAndPathCharacters;
        }
    }
}