using System.Text.RegularExpressions;

namespace Manager.Extensions.AttributeSchemas
{
    public class NumberAttribute : BaseAttribute
    {
        public override bool Validate(object value, out string errorMessages)
        {
            errorMessages = "";
            try
            {
                string _value = Convert.ToString(value);
                if (!Regex.IsMatch(_value, RegexHelper.NumberPattern))
                {
                    errorMessages = $"{this.GetType().Name}:请输入正确的数值类型";
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