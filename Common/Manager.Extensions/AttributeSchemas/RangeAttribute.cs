namespace Manager.Extensions.AttributeSchemas
{
    public class RangeAttribute : BaseAttribute
    {
        private readonly int[] ranges;

        public RangeAttribute(params int[] ranges)
        {
            this.ranges = ranges;
        }

        public override bool Validate(object value, out string errorMessages)
        {
            errorMessages = "";
            try
            {
                var length = Convert.ToString(value).Length;

                if (ranges == null || ranges.Length < 1 || ranges.Length > 2)
                {
                    errorMessages = $"{this.GetType().Name}:请输入正确的限制范围";
                    return false;
                }
                else if (ranges.Length == 1)
                {
                    if (ranges[0] != length)
                    {
                        errorMessages = $"{this.GetType().Name}:请输入正确的范围 {ranges[0]} 位";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (length < ranges[0] || length > ranges[1])
                    {
                        errorMessages = $"{this.GetType().Name}:请输入正确的范围 {ranges[0]}-{ranges[1]} 位";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessages = $"{this.GetType().Name}:{ex}";
                return false;
            }
        }
    }
}