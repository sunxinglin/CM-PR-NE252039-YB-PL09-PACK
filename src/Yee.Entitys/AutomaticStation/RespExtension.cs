using Microsoft.FSharp.Core;

namespace Yee.Entitys.AutomaticStation
{
    public static class RespExtension
    {
        public static Resp<object> ToErr(this Resp<object> resp, int errorCode, string errorMessage)
        {
            resp.Success = false;
            resp.ErrorInfo = new ErrorInfo() { Code = errorCode.ToString(), Message = errorMessage };
            return resp;
        }

        public static Resp<object> ToSucc(this Resp<object> resp)
        {
            resp.Success = true;
            return resp;
        }

        public static Resp<T> ToErr<T>(this T t, int errorCode, string errorMessage)
        {
            var resp = new Resp<T>();
            resp.Success = false;
            resp.ErrorInfo = new ErrorInfo() { Code = errorCode.ToString(), Message = errorMessage };
            return resp;
        }

        public static Resp<T> ToSucc<T>(this T T1)
        {
            var resp = new Resp<T>();
            resp.Success = true;
            resp.Data = T1;
            return resp;
        }


        public static Resp MakeSuccess()
        {
            var resp = new Resp()
            {
                Success = true,
                Data = { },
            };
            return resp;
        }

        public static Resp<TData> MakeSuccess<TData>(TData data)
        {
            var resp = new Resp<TData>()
            {
                Success = true,
                Data = data,
            };
            return resp;
        }

        public static Resp<TData> MakeFail<TData>(string errCode, string errMsg)
        {
            var resp = new Resp<TData>()
            {
                Success = false,
                ErrorInfo = new ErrorInfo()
                {
                    Code = errCode,
                    Message = errMsg,
                    ErrorFields = new List<ErrorField>(),
                },
            };
            return resp;
        }

        public static Resp<TData> MakeFail<TData>(string errCode, string errMsg, IList<ErrorField> errFields)
        {
            var resp = new Resp<TData>()
            {
                Success = false,
                ErrorInfo = new ErrorInfo()
                {
                    Code = errCode,
                    Message = errMsg,
                    ErrorFields = errFields
                },
            };
            return resp;
        }

        public static Resp<TData> ToResp<TData>(this FSharpResult<TData, string> res, string code)
        {
            if (res.IsOk)
            {
                return MakeSuccess(res.ResultValue);
            }
            else
            {
                return MakeFail<TData>(code, res.ErrorValue);
            }
        }

        public static Resp<TData> ToResp<TData>(this FSharpResult<TData, ErrorInfo> res)
        {
            if (res.IsOk)
            {
                return MakeSuccess(res.ResultValue);
            }
            else
            {
                var err = res.ErrorValue;
                return MakeFail<TData>(err.Code, err.Message);
            }
        }

    }
}
