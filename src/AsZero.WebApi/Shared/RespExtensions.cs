using Yee.Entitys.AutomaticStation;

namespace AsZero.WebApi.Shared
{
    public static class RespExtensions
    {
        public static Resp MakeSuccess()
        {
            return new Resp
            {
                Success = true
            };
        }

        public static Resp<TData> MakeSuccess<TData>(TData data)
        {
            return new Resp<TData>
            {
                Success = true,
                Data = data
            };
        }

        public static Resp<TData> ToSuccessfulResp<TData>(this TData data)
        {
            return MakeSuccess(data);
        }

        

        public static Resp<TData> MakeFail<TData>(string errCode, string errMsg)
        {
            return new Resp<TData>
            {
                Success = false,
                ErrorInfo = new ErrorInfo
                {
                    Code = errCode,
                    Message = errMsg,
                    ErrorFields = new List<ErrorField>()
                }
            };
        }

        public static Resp<TData> MakeFail<TData>(string errCode, string errMsg, IList<ErrorField> errFields)
        {
            return new Resp<TData>
            {
                Success = false,
                ErrorInfo = new ErrorInfo
                {
                    Code = errCode,
                    Message = errMsg,
                    ErrorFields = errFields
                }
            };
        }

        public static Resp<TData> AddFieldError<TData>(this Resp<TData> resp, string fieldName, params string[] fieldErrors)
        {
            if (resp.ErrorInfo == null)
            {
                resp.ErrorInfo = new ErrorInfo();
            }

            if (resp.ErrorInfo.ErrorFields == null)
            {
                resp.ErrorInfo.ErrorFields = new List<ErrorField>();
            }

            resp.ErrorInfo.ErrorFields.Add(new ErrorField
            {
                Name = fieldName,
                Errors = fieldErrors
            });
            return resp;
        }

        public static Resp<TData> MakeFieldFail<TData>(string fieldName, string fieldError)
        {
            return MakeFail<TData>("400", fieldName + ": " + fieldError).AddFieldError(fieldName, fieldError);
        }
    }

    public class Resp: Resp<object>
    {

    }
}
