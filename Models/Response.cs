using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Response<T>
    {
        /// <summary>
        /// The result of the response
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Indicates if the response contains a result
        /// </summary>
        public bool HasResult => this.Result != null;

        /// <summary>
        /// The result type
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        /// The message returned with the response
        /// </summary>
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The Error Code returned with the response
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The Error message returned with the response
        /// </summary>
        public string ErrorDescription { get; set; }

        public Response()
        {
            ResultType = ResultType.Success;
            StatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Creates a successful response with a given result object
        /// </summary>
        /// <param name="result">The result object to return with the response</param>
        /// <returns>The response object</returns>
        public void OnSuccess(T result)
        {
            this.ResultType = ResultType.Success;
            this.Result = result;
            this.StatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Creates a failed result. It takes no result object
        /// </summary>
        /// <param name="errorMessage">The error message returned with the response</param>
        /// <returns>The created response object</returns>
        public void OnFailure(string errorMessage, HttpStatusCode statusCode)
        {
            this.ResultType = ResultType.Error;
            this.StatusCode = statusCode;
            this.Message = errorMessage;
        }


        /// <summary>
        /// Creates a failed result. It takes no result object
        /// </summary>
        /// <param name="errorMessage">The error message returned with the response</param>
        /// <returns>The created response object</returns>
        ////public void OnBusinessFailure(string errorCode, string errorMessage)
        ////{

        ////}

        /// <summary>
        /// Creates a warning result. The warning result is successful, but might have issues that should be addressed or logged
        /// </summary>
        /// <param name="warningMessage">The warning returned with the response</param>
        /// <param name="result">The result object</param>
        /// <returns>The created response object</returns>
        public void Warning(string warningMessage, T result, HttpStatusCode statusCode)
        {
            this.ResultType = ResultType.Warning;
            this.Result = result;
            this.StatusCode = statusCode;
            this.Message = warningMessage;
        }
    }
    }
