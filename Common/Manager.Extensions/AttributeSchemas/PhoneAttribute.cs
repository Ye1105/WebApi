using System.Text.RegularExpressions;

namespace Manager.Extensions.AttributeSchemas
{
    public class PhoneAttribute : BaseAttribute
    {
        public override bool Validate(object value, out string errorMessages)
        {
            errorMessages = "";
            try
            {
                string _value = Convert.ToString(value);
                if (!Regex.IsMatch(_value, RegexHelper.PhonePattern))
                {
                    errorMessages = $"{this.GetType().Name}:请输入正确的手机号码";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessages = $"{this.GetType().Name}:{ex}";
                return false;
            }
        }
    }
}