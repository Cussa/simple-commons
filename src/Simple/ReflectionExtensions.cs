﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Reflection;
using System.Reflection;

namespace Simple
{
    public static class ReflectionExtensions
    {
        public static Type GetValueTypeIfNullable(this Type type)
        {
            return (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
               ? type.GetGenericArguments()[0] : type;
        }

        public static bool CanAssign<T, IntoThis>()
        {
            return CanAssign(typeof(T), typeof(IntoThis));
        }

        public static bool CanAssign<IntoThis>(this Type type)
        {
            return typeof(IntoThis).IsAssignableFrom(type);
        }


        public static bool CanAssign(this Type type, Type intoThis)
        {
            return intoThis.IsAssignableFrom(type);
        }

        public static ISettableMemberInfo ToSettable(this IEnumerable<ISettableMemberInfo> members)
        {
            return new CompositeSettableMember(members);
        }

        public static ISettableMemberInfo ToSettable(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).ToSettable();

            if (member is FieldInfo)
                return (member as FieldInfo).ToSettable();

            return null;
        }


        public static ISettableMemberInfo ToSettable(this FieldInfo member)
        {
            return new FieldInfoWrapper(member);
        }

        public static ISettableMemberInfo ToSettable(this PropertyInfo member)
        {
            return new PropertyInfoWrapper(member);
        }


        public static object GetBoxedDefaultInstance(this Type type)
        {
            return type.IsValueType && !typeof(void).IsAssignableFrom(type) ?
                Activator.CreateInstance(type) : null;
        }

        public static Type[] GetTypeArgumentsFor(this Type toTest, Type toImplement)
        {
            foreach (var iface in toTest.GetInterfaces())
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == toImplement)
                    return iface.GetGenericArguments();

            return new Type[0];
        }

        public static string GetRealClassName(this Type type)
        {
            return new TypeNameExtractor().GetName(type);
        }

        public static string GetFlatClassName(this Type type)
        {
            return new TypeNameExtractor().GetFlatName(type, "_");
        }

        internal static IEnumerable<string> SplitProperty(this string propertyPath)
        {
            return propertyPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static string JoinProperty(this IEnumerable<string> propertyPath)
        {
            return propertyPath.StringJoin(".");
        }

        public static bool IsNumericType(this Type type)
        {
            return type.IsNumericType(true);
        }

        public static bool IsNumericType(this Type type, bool allowEnums)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return !type.IsEnum;
                default:
                    return false;
            }
        }
    }
}
