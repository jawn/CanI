﻿using CanI.Core.Configuration;
using CanI.Tests.TestUtilities;
using NUnit.Framework;

namespace CanI.Tests.Abilities
{
    [TestFixture]
    public class PostFixAbiltyTest
    {
        private class CustomerViewModel {}
        private class EditCustomer {}
        private class UpdateCustomer { }
        private class EditCustomerViewModel { }
        private class CustomerEditViewModel { }

        [SetUp]
        public void SetUp()
        {
            AbilityConfiguration.Reset();
        }

        [Test]
        public void abilities_ignore_postfixes_by_default()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("edit", "customer"));
            Then.IShouldBeAbleTo("edit", new CustomerViewModel());
        }

        [Test]
        public void abilities_ignore_prefixes_by_default()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("edit", "customer"));
            Then.IShouldBeAbleTo("edit", new EditCustomer());
        }

        [Test]
        public void abilities_ignore_prefixes_aliases_by_default()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("edit", "customer"));
            Then.IShouldBeAbleTo("edit", new UpdateCustomer());
        }

        [Test]
        public void abilities_ignore_prefixes_and_postfixes_by_default()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("edit", "customer"));
            Then.IShouldBeAbleTo("edit", new EditCustomerViewModel());
        }

        [Test]
        public void abilities_ignore_reversed_prefixes_and_postfixes_by_default()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("edit", "customer"));
            Then.IShouldBeAbleTo("edit", new CustomerEditViewModel());
        }

    }
}