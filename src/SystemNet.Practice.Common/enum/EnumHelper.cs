using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace SystemNet.Practice.Common
{
    /// <summary>
    /// util functions for enum
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Get Description for an property of enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        if (memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            throw new ArgumentNullException($" The enum {e} must be decorated with a Description ");
        }
    }
}
