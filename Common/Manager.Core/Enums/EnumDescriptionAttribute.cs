namespace Manager.Core.Enums
{
    //EnumDescription.GetEnumDescription((Enums.EnumHotSearchType)m.type);
    public class EnumDescriptionAttribute : Attribute
    {
        private string m_strDescription;

        public EnumDescriptionAttribute(string strPrinterName)
        {
            m_strDescription = strPrinterName;
        }

        public string Description
        {
            get { return m_strDescription; }
        }

        public static string GetEnumDescription(Enum enumObj)
        {
            System.Reflection.FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            try
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length == 0)
                {
                    return String.Empty;
                }
                else
                {
                    EnumDescriptionAttribute attrib = attribArray[0] as EnumDescriptionAttribute;

                    return attrib.Description;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}