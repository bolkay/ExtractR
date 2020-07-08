using System;

namespace ExtractR.Exceptions
{
    public class ExtractRFilePathException : Exception
    {
        public ExtractRFilePathException(string message) : base(message)
        {

        }

        public override string Message => base.Message;
    }
}
