using System;
using System.Collections.Generic;
using System.Text;

namespace CreditAccountBLL
{
    public struct Result<T>
    {
        public String ErrorMessage { get; }
        public T Data { get; }
        public bool IsSuccess => ErrorMessage == null;

        public Result(T data, string errorMessage)
        {
            Data = data;
            ErrorMessage = errorMessage;
        }


        public static Result<T> CreateError(string errorMessage)
        {
            return new Result<T>(default, errorMessage);
        }

        public static Result<T> CreateSuccess(T data)
        {
            return new Result<T>(data, null);
        }
    }

    public struct Result
    {
        public String ErrorMessage { get; }
        public bool IsSuccess => ErrorMessage == null;

        public Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public static Result CreateSuccess()
        {
            return new Result();
        }

        public static Result CreateError(String errorMessage)
        {
            return new Result(errorMessage);
        }
    }

}
