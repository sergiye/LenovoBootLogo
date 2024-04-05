using System;
using System.Linq;

namespace PCBootLogo {

  [AttributeUsage(AttributeTargets.Field)]
  public class EnumDisplayAttribute : Attribute {
  
    public EnumDisplayAttribute(string displayStr) {
      Display = displayStr;
    }

    public string Display { get; }
  }
  
  public static class EnumDisplayExtensions {
    
    public static string Display(this Enum t) {
      var type = t.GetType();
      var name = Enum.GetName(type, t);
      if (type.GetField(name).GetCustomAttributes(false)
            .FirstOrDefault(p => p.GetType() == typeof(EnumDisplayAttribute)) is EnumDisplayAttribute enumDisplayAttribute) return enumDisplayAttribute.Display;
      return name;
    }
  }

}