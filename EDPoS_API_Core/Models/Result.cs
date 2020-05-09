using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    /// <summary>
    /// Result
    /// </summary>
    public class Result : IResult
    {
        private string _message;

        /// <summary>
        /// Is success
        /// </summary>
        public bool Success => Code == ResultCode.Ok;

        /// <summary>
        /// Result code
        /// </summary>
        public ResultCode Code { get; set; }

        /// <summary>
        /// show message
        /// </summary>
        public string Message
        {
            get { return _message ?? Code.DisplayName(); }
            set { _message = value; }
        }

        /// <summary>
        /// Return result,default ok
        /// </summary>
        public Result()
        {
            Code = ResultCode.Ok;
        }

        /// <summary>
        /// Return the result
        /// </summary>
        /// <param name="code">state code</param>
        /// <param name="message">message</param>
        public Result(ResultCode code, string message = null)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Return the specific state code
        /// </summary>
        public static Result FromCode(ResultCode code, string message = null)
        {
            return new Result(code, message);
        }
        
         /// <summary>
        /// Return error message
        /// </summary>
        public static Result FromError(string message, ResultCode code = ResultCode.Fail)
        {
            return new Result(code, message);
        }
        
        /// <summary>
        /// Return successful
        /// </summary>
        public static Result Ok(string message = null)
        {
            return FromCode(ResultCode.Ok, message);
        }

        /// <summary>
        /// Return the specific state code
        /// </summary>
        public static Result<T> FromCode<T>(ResultCode code, string message = null)
        {
            return new Result<T>(code, message);
        }

        /// <summary>
        /// Return the specific state code, message and data
        /// </summary>
        public static Result<T> FromCode<T>(ResultCode code, T data, string message = null)
        {
            return new Result<T>(code, message, data);
        }

        /// <summary>
        /// Return error message
        /// </summary>
        public static Result<T> FromError<T>(string message, ResultCode code = ResultCode.Fail)
        {
            return new Result<T>(code, message);
        }

        /// <summary>
        /// Return data
        /// </summary>
        public static Result<T> FromData<T>(T data)
        {
            return new Result<T>(data);
        }

        /// <summary>
        /// Return data and message
        /// </summary>
        public static Result<T> FromData<T>(T data, string message)
        {
            return new Result<T>(ResultCode.Ok, message, data);
        }
        
        /// <summary>
        /// Return successfull
        /// </summary>
        public static Result<T> Ok<T>(T data)
        {
            return FromData(data);
        }
    }

    /// <summary>
    /// Return result
    /// </summary>
    public class Result<TType> : Result, IResult<TType>
    {
        public Result()
        {}
        /// <summary>
        /// Return result
        /// </summary>
        public Result(TType data) : base(ResultCode.Ok)
        {
            Data = data;
        }

        /// <summary>
        /// Return result and message
        /// </summary>
        /// <param name="code">state code</param>
        /// <param name="message">Note</param>
        public Result(ResultCode code, string message = null) : base(code, message)
        {
        }

        /// <summary>
        /// Return result
        /// </summary>
        public Result(ResultCode code, string message = null, TType data = default(TType)) : base(code, message)
        {
            Data = data;
        }

        /// <summary>
        /// Data
        /// </summary>
        public TType Data { get; set; }
    }
}
