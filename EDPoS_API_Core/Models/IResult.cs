using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EDPoS_API_Core.Models
{
    /// <summary>
    /// The Interface of result
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Code
        /// </summary>
        ResultCode Code { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Is Success
        /// </summary>
        bool Success { get; }
    }

    /// <summary>
    /// Generic classes
    /// </summary>
    public interface IResult<TType> : IResult
    {
        /// <summary>
        /// Data
        /// </summary>
        TType Data { get; set; }
    }

    /// <summary>
    /// Code
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// OK
        ///</summary>
        [Display(Name = "OK")]
        Ok = 1,

        /// <summary>
        /// Fail
        ///</summary>
        [Display(Name = "Fail")]
        Fail = 11,

        /// <summary>
        /// Login fail
        ///</summary>
        [Display(Name = "Login fail")]
        LoginFail = 12,

        /// <summary>
        /// No Record
        ///</summary>
        [Display(Name = "No record")]
        NoRecord = 13,

        /// <summary>
        /// No users
        ///</summary>
        [Display(Name = "No users")]
        NoSuchUser = 14,

        /// <summary>
        /// Not logged
        ///</summary>
        [Display(Name = "Not logged")]
        Unauthorized = 20,

        /// <summary>
        /// Unauthorized
        /// </summary>
        [Display(Name = "Unauthorized")]
        Forbidden = 21,

        /// <summary>
        /// Invalid token
        /// </summary>
        [Display(Name = "Invalid token")]
        InvalidToken = 22,

        /// <summary>
        /// Parameter validation failed
        /// </summary>
        [Display(Name = "Parameter validation failed")]
        InvalidData = 23,

        /// <summary>
        /// Invalid user
        /// </summary>
        [Display(Name = "Invalid user")]
        InvalidUser = 24,

        /// <summary>
        /// The denominator can not be zero
        /// </summary>
        [Display(Name = "The denominator can not be zero")]
        Zero = 25
    }

    /// <summary>
    /// show message
    /// </summary>
    public static class EumHelper
    {
        /// <summary>
        /// Display message
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string DisplayName(this Enum val)
        {
            var type = val.GetType();
            var field = type.GetField(val.ToString());
            var obj = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
            return obj.Name ?? "";
        }
    }
}
