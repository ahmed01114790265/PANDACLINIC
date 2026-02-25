using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Shared.ResultModel
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public List<string> Errors { get; } = new();

        protected Result(bool isSuccess, string message, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            if (errors != null) Errors = errors;
        }

        public static Result Success(string message = "Operation successful")
            => new Result(true, message);

        public static Result Failure(string error)
            => new Result(false, "Operation failed", new List<string> { error });

        public static Result Failure(List<string> errors)
            => new Result(false, "Operation failed", errors);
    }

    // Generic version 
    public class Result<T> : Result
    {
        public T? Data { get; }

        protected Result(T? data, bool isSuccess, string message, List<string>? errors = null)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "Success")
            => new Result<T>(data, true, message);

        public new static Result<T> Failure(string error)
            => new Result<T>(default, false, "Failure", new List<string> { error });
    }
}
