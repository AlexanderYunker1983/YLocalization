using System;
using System.Collections.Generic;
using System.Linq;

namespace YLocalization
{
    public class LocalizedEnumWrapper<T>
    {
        public LocalizedEnumWrapper(ILocalizationManager localizationManager, T enumValue)
        {
            Value = enumValue;
            var key = EnumToResourceKey(enumValue);
            Text = localizationManager.GetString(key);
        }

        public string StringValue => Value?.ToString();
        public T Value { get; }

        public string Text { get; set; }

        public static List<LocalizedEnumWrapper<T>> GetValues(ILocalizationManager localizationManager, bool firstValueToLast = false)
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var result = new List<LocalizedEnumWrapper<T>>();
            if (firstValueToLast && enumValues.Count > 1)
            {
                result.AddRange(enumValues.Skip(1).Select(arg => new LocalizedEnumWrapper<T>(localizationManager, arg)));
                result.Add(new LocalizedEnumWrapper<T>(localizationManager, enumValues.First()));
            }
            else
            {
                result = enumValues.Select(arg => new LocalizedEnumWrapper<T>(localizationManager, arg)).ToList();
            }
            return result;
        }

        private bool Equals(LocalizedEnumWrapper<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((LocalizedEnumWrapper<T>)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public static bool operator ==(LocalizedEnumWrapper<T> left, LocalizedEnumWrapper<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LocalizedEnumWrapper<T> left, LocalizedEnumWrapper<T> right)
        {
            return !Equals(left, right);
        }

        public static string EnumToResourceKey(T enumValue)
        {
            var enumType = enumValue.GetType();
            var key = $"{enumType.Name}_{Enum.GetName(enumType, enumValue)}";
            return key;
        }
    }
}
