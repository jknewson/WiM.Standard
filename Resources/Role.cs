using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WIM.Resources
{
    public class Role
    {
        public const string Admin = "Administrator";
        public const string Manager = "Manager";

        public static IList<String> ToList()
        {
            //https://stackoverflow.com/questions/4994469/loop-through-constant-members-of-a-class
            // Gets all public and static fields from Role and base class
            FieldInfo[] fieldInfos = typeof(Role).GetFields(BindingFlags.Public | BindingFlags.Static |
            BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(fi => fi.GetValue(null).ToString()).ToList();

        }
    }
}
