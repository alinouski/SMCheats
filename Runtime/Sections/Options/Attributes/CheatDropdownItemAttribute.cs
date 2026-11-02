using System;

namespace SMCheats
{
    /// <example>
    /// [CheatDropdown(nameof(Weapons))]
    /// [CheatDropdownItem(nameof(WeaponConfig.Title), nameof(WeaponConfig.Id))]
    /// [Cheat]
    /// public string CurrentWeaponId { get; set; }
    ///
    /// private IReadOnlyList<WeaponConfig> Weapons => _weaponConfigs;
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CheatDropdownItemAttribute : Attribute
    {
        public CheatDropdownItemAttribute(string labelMemberName, string valueMemberName = null)
        {
            if (string.IsNullOrWhiteSpace(labelMemberName))
            {
                throw new ArgumentException("Dropdown label member name must not be empty.", nameof(labelMemberName));
            }

            LabelMemberName = labelMemberName;
            ValueMemberName = valueMemberName;
        }

        public string LabelMemberName { get; }
        public string ValueMemberName { get; }
    }
}
