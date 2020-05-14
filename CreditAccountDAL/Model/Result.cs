using System;

namespace CreditAccountDAL
{
    public class Result
    {
        public String ErrorMessage { get; }
        public bool IsSuccess => ErrorMessage == null;

        public Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public static Result CreateSuccess()
        {
            return new Result(null);
        }

        public static Result CreateError(String errorMessage)
        {
            return new Result(errorMessage);
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        private Result(T data, string errorMessage) : base(errorMessage)
        {
            Data = data;
        }

        public static new Result<T> CreateError(string errorMessage)
        {
            return new Result<T>(default, errorMessage);
        }

        public static Result<T> CreateSuccess(T data)
        {
            return new Result<T>(data, null);
        }
    }
}
