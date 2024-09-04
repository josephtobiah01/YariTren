namespace Core.Helpers
{
    public class RegistrationHelper
    {
        public static bool RegisterUser(string name, string email, string phone)
        {
            var requestBody = $"{{'Name':'{name}','Phone':'{phone}','Email':'{email}'}}";
            var responseBody = HttpUtility.GetResponseBody(Consts.RegistrationFunctionUrl, requestBody, null, "POST", null, null);
            if(responseBody.Contains("Message successfully sent"))
            {
                return true;
            }
            return false;
        }
    }
}
