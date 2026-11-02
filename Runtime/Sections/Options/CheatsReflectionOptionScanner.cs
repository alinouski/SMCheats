using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SMCheats
{
    internal static class CheatsReflectionOptionScanner
    {
        private const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags SourceMembers =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly Dictionary<Type, MemberDescriptor[]> MembersByType = new();

        public static List<CheatOptionDefinition> Scan(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var options = new List<CheatOptionDefinition>();
            var targetType = target.GetType();
            var members = GetCachedMembers(targetType);

            foreach (var member in members)
            {
                switch (member.Member)
                {
                    case MethodInfo method:
                        AddMethodOption(options, target, targetType, method, member.Cheat, member.VisibleIf);
                        break;
                    case PropertyInfo property:
                        AddPropertyOption(
                            options,
                            target,
                            targetType,
                            property,
                            member.Cheat,
                            member.Dropdown,
                            member.DropdownItem,
                            member.Range,
                            member.Step,
                            member.VisibleIf);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"SMCheat supports only methods and properties: {targetType.Name}.{member.Member.Name}");
                }
            }

            return options;
        }

        private static MemberDescriptor[] GetCachedMembers(Type targetType)
        {
            if (MembersByType.TryGetValue(targetType, out var members))
            {
                return members;
            }

            members = targetType
                .GetMembers(InstanceMembers)
                .Select(member => new MemberDescriptor(
                    member,
                    member.GetCustomAttribute<CheatAttribute>(),
                    member.GetCustomAttribute<CheatDropdownAttribute>(),
                    member.GetCustomAttribute<CheatDropdownItemAttribute>(),
                    member.GetCustomAttribute<CheatRangeAttribute>(),
                    member.GetCustomAttribute<CheatStepAttribute>(),
                    member.GetCustomAttribute<CheatVisibleIfAttribute>()))
                .Where(member => member.Cheat != null)
                .OrderBy(member => GetDeclarationOrder(member.Cheat, member.Member))
                .ThenBy(member => member.Member.MetadataToken)
                .ToArray();
            MembersByType[targetType] = members;
            return members;
        }

        private static void AddMethodOption(
            ICollection<CheatOptionDefinition> options,
            object target,
            Type targetType,
            MethodInfo method,
            CheatAttribute attribute,
            CheatVisibleIfAttribute visibleIfAttribute)
        {
            if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0)
            {
                throw new InvalidOperationException(
                    $"SMCheat method must be parameterless void: {targetType.Name}.{method.Name}");
            }

            var displayName = GetDisplayName(attribute, method.Name);
            options.Add(CheatOptionDefinition
                .Action(displayName, () => method.Invoke(target, null), attribute.Category, attribute.SortOrder)
                .WithVisibility(CreateVisibilityGetter(target, targetType, method, visibleIfAttribute))
                .WithOwner(target));
        }

        private static void AddPropertyOption(
            ICollection<CheatOptionDefinition> options,
            object target,
            Type targetType,
            PropertyInfo property,
            CheatAttribute attribute,
            CheatDropdownAttribute dropdownAttribute,
            CheatDropdownItemAttribute dropdownItemAttribute,
            CheatRangeAttribute rangeAttribute,
            CheatStepAttribute stepAttribute,
            CheatVisibleIfAttribute visibleIfAttribute)
        {
            if (property.GetIndexParameters().Length != 0)
            {
                throw new InvalidOperationException(
                    $"SMCheat property must not be indexed: {targetType.Name}.{property.Name}");
            }

            if (property.GetMethod == null)
            {
                throw new InvalidOperationException($"SMCheat property must have getter: {targetType.Name}.{property.Name}");
            }

            var displayName = GetDisplayName(attribute, property.Name);
            var visibilityGetter = CreateVisibilityGetter(target, targetType, property, visibleIfAttribute);

            if (property.PropertyType == typeof(bool))
            {
                options.Add(CheatOptionDefinition
                    .Toggle(
                        displayName,
                        () => (bool)property.GetValue(target),
                        property.SetMethod == null ? null : value => property.SetValue(target, value),
                        attribute.Category,
                        attribute.SortOrder)
                    .WithVisibility(visibilityGetter)
                    .WithOwner(target));
                return;
            }

            ValidateNumericAttributes(targetType, property, rangeAttribute, stepAttribute, dropdownAttribute);

            options.Add(CheatOptionDefinition
                .Value(
                    displayName,
                    property.PropertyType,
                    () => property.GetValue(target),
                    property.SetMethod == null ? null : value => property.SetValue(target, value),
                    dropdownAttribute == null
                        ? null
                        : CreateDropdownChoicesGetter(target, targetType, property, dropdownAttribute, dropdownItemAttribute),
                    attribute.Category,
                    attribute.SortOrder)
                .WithVisibility(visibilityGetter)
                .WithNumericRange(
                    rangeAttribute?.MinimumValue,
                    rangeAttribute?.MaximumValue,
                    stepAttribute?.Value)
                .WithOwner(target));
        }

        private static void ValidateNumericAttributes(
            Type targetType,
            PropertyInfo property,
            CheatRangeAttribute rangeAttribute,
            CheatStepAttribute stepAttribute,
            CheatDropdownAttribute dropdownAttribute)
        {
            if (rangeAttribute == null && stepAttribute == null)
            {
                return;
            }

            if (dropdownAttribute != null)
            {
                throw new InvalidOperationException(
                    $"Numeric attributes cannot be combined with dropdown on {targetType.Name}.{property.Name}");
            }

            if (property.PropertyType != typeof(int) && property.PropertyType != typeof(float))
            {
                throw new InvalidOperationException(
                    $"Numeric attributes are supported only for int and float cheat properties: {targetType.Name}.{property.Name}");
            }
        }

        private static Func<IReadOnlyList<CheatDropdownChoice>> CreateDropdownChoicesGetter(
            object target,
            Type targetType,
            PropertyInfo targetProperty,
            CheatDropdownAttribute dropdownAttribute,
            CheatDropdownItemAttribute dropdownItemAttribute)
        {
            var sourceMember = targetType.GetMember(dropdownAttribute.SourceMemberName, SourceMembers).FirstOrDefault();
            if (sourceMember == null)
            {
                throw new InvalidOperationException(
                    $"Dropdown source member '{dropdownAttribute.SourceMemberName}' was not found for {targetType.Name}.{targetProperty.Name}");
            }

            return () =>
            {
                var rawValue = GetSourceMemberValue(target, sourceMember);
                return ConvertDropdownChoices(rawValue, targetType, targetProperty, sourceMember, dropdownItemAttribute);
            };
        }

        private static Func<bool> CreateVisibilityGetter(
            object target,
            Type targetType,
            MemberInfo cheatMember,
            CheatVisibleIfAttribute visibleIfAttribute)
        {
            if (visibleIfAttribute == null)
            {
                return null;
            }

            var sourceMember = targetType.GetMember(visibleIfAttribute.SourceMemberName, SourceMembers).FirstOrDefault();
            if (sourceMember == null)
            {
                throw new InvalidOperationException(
                    $"Visibility source member '{visibleIfAttribute.SourceMemberName}' was not found for {targetType.Name}.{cheatMember.Name}");
            }

            return () =>
            {
                var rawValue = GetSourceMemberValue(target, sourceMember);
                if (rawValue is not bool boolValue)
                {
                    throw new InvalidOperationException(
                        $"Visibility source member must return bool: {targetType.Name}.{cheatMember.Name} -> {sourceMember.Name}");
                }

                return boolValue == visibleIfAttribute.ExpectedValue;
            };
        }

        private static object GetSourceMemberValue(object target, MemberInfo sourceMember)
        {
            return sourceMember switch
            {
                MethodInfo method when method.GetParameters().Length == 0 => method.Invoke(
                    method.IsStatic ? null : target,
                    null),
                PropertyInfo property when property.GetMethod != null => property.GetValue(
                    property.GetMethod.IsStatic ? null : target),
                FieldInfo field => field.GetValue(field.IsStatic ? null : target),
                MethodInfo method => throw new InvalidOperationException(
                    $"Dropdown source method must be parameterless: {method.DeclaringType?.Name}.{method.Name}"),
                PropertyInfo property => throw new InvalidOperationException(
                    $"Dropdown source property must have getter: {property.DeclaringType?.Name}.{property.Name}"),
                _ => throw new InvalidOperationException(
                    $"Dropdown source member must be method, property or field: {sourceMember.DeclaringType?.Name}.{sourceMember.Name}")
            };
        }

        private static IReadOnlyList<CheatDropdownChoice> ConvertDropdownChoices(
            object rawValue,
            Type targetType,
            PropertyInfo targetProperty,
            MemberInfo sourceMember,
            CheatDropdownItemAttribute dropdownItemAttribute)
        {
            if (rawValue == null)
            {
                return Array.Empty<CheatDropdownChoice>();
            }

            if (rawValue is IReadOnlyList<CheatDropdownChoice> choices)
            {
                return choices;
            }

            if (rawValue is IEnumerable<CheatDropdownChoice> choiceEnumerable)
            {
                return choiceEnumerable.ToArray();
            }

            if (rawValue is string)
            {
                throw new InvalidOperationException(
                    $"Dropdown source must be an enumerable collection, not string: {targetType.Name}.{targetProperty.Name} -> {sourceMember.Name}");
            }

            if (rawValue is not IEnumerable enumerable)
            {
                throw new InvalidOperationException(
                    $"Dropdown source must return IEnumerable: {targetType.Name}.{targetProperty.Name} -> {sourceMember.Name}");
            }

            var result = new List<CheatDropdownChoice>();
            foreach (var item in enumerable)
            {
                if (item is CheatDropdownChoice choice)
                {
                    result.Add(choice);
                    continue;
                }

                if (dropdownItemAttribute != null)
                {
                    result.Add(CreateDropdownChoiceFromItem(item, targetType, targetProperty, sourceMember, dropdownItemAttribute));
                    continue;
                }

                result.Add(new CheatDropdownChoice(Convert.ToString(item) ?? "null", item));
            }

            return result;
        }

        private static CheatDropdownChoice CreateDropdownChoiceFromItem(
            object item,
            Type targetType,
            PropertyInfo targetProperty,
            MemberInfo sourceMember,
            CheatDropdownItemAttribute dropdownItemAttribute)
        {
            if (item == null)
            {
                return new CheatDropdownChoice("null", null);
            }

            var itemType = item.GetType();
            var labelMember = itemType.GetMember(dropdownItemAttribute.LabelMemberName, SourceMembers).FirstOrDefault();
            if (labelMember == null)
            {
                throw new InvalidOperationException(
                    $"Dropdown item label member '{dropdownItemAttribute.LabelMemberName}' was not found on {itemType.Name} for {targetType.Name}.{targetProperty.Name} -> {sourceMember.Name}");
            }

            var labelValue = GetSourceMemberValue(item, labelMember);
            var choiceValue = item;

            if (!string.IsNullOrWhiteSpace(dropdownItemAttribute.ValueMemberName))
            {
                var valueMember = itemType.GetMember(dropdownItemAttribute.ValueMemberName, SourceMembers).FirstOrDefault();
                if (valueMember == null)
                {
                    throw new InvalidOperationException(
                        $"Dropdown item value member '{dropdownItemAttribute.ValueMemberName}' was not found on {itemType.Name} for {targetType.Name}.{targetProperty.Name} -> {sourceMember.Name}");
                }

                choiceValue = GetSourceMemberValue(item, valueMember);
            }

            return new CheatDropdownChoice(Convert.ToString(labelValue) ?? "null", choiceValue);
        }

        private static int GetDeclarationOrder(CheatAttribute attribute, MemberInfo member)
        {
            if (attribute.SortOrder != 0)
            {
                return attribute.SortOrder;
            }

            if (attribute.LineNumber != 0)
            {
                return attribute.LineNumber;
            }

            return member.MetadataToken;
        }

        private static string GetDisplayName(CheatAttribute attribute, string fallbackName)
        {
            return string.IsNullOrWhiteSpace(attribute.DisplayName) ? fallbackName : attribute.DisplayName;
        }

        private readonly struct MemberDescriptor
        {
            public MemberDescriptor(
                MemberInfo member,
                CheatAttribute cheat,
                CheatDropdownAttribute dropdown,
                CheatDropdownItemAttribute dropdownItem,
                CheatRangeAttribute range,
                CheatStepAttribute step,
                CheatVisibleIfAttribute visibleIf)
            {
                Member = member;
                Cheat = cheat;
                Dropdown = dropdown;
                DropdownItem = dropdownItem;
                Range = range;
                Step = step;
                VisibleIf = visibleIf;
            }

            public MemberInfo Member { get; }
            public CheatAttribute Cheat { get; }
            public CheatDropdownAttribute Dropdown { get; }
            public CheatDropdownItemAttribute DropdownItem { get; }
            public CheatRangeAttribute Range { get; }
            public CheatStepAttribute Step { get; }
            public CheatVisibleIfAttribute VisibleIf { get; }
        }
    }
}
