﻿namespace CanI.Core
{
    public interface IAbility
    {
        bool Allows(string action, string subject);
    }

    public interface IAbilityConfiguration
    {
        void AllowTo(string action, string subject);
    }
}