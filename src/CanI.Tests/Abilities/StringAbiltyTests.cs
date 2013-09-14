﻿using CanI.Core.Configuration;
using CanI.Tests.TestUtilities;
using NUnit.Framework;

namespace CanI.Tests.Abilities
{
    [TestFixture]
    public class StringAbiltyTests
    {
        [SetUp]
        public void SetUp()
        {
            AbilityConfiguration.Reset();
        }

        [Test]
        public void a_null_abiltiy_doesnt_allow_anything()
        {
            Then.IShouldNotBeAbleTo("view", "customer");
        }

        [Test]
        public void a_simple_ability_allows_its_action()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("view", "customer"));
            Then.IShouldBeAbleTo("view", "customer");
        }

        [Test]
        public void abilities_do_not_cross()
        {
            AbilityConfiguration.ConfigureWith(c =>
            {
                c.AllowTo("create", "customer");
                c.AllowTo("edit", "order");
            });
            Then.IShouldBeAbleTo("create", "customer");
            Then.IShouldBeAbleTo("edit", "order");

            Then.IShouldNotBeAbleTo("edit", "customer");
            Then.IShouldNotBeAbleTo("create", "order");
        }

        [Test]
        public void if_you_can_manage__you_can_do_anything_on_the_subject()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("manage", "customer"));
            Then.IShouldBeAbleTo("view", "customer");
            Then.IShouldBeAbleTo("create", "customer");
            Then.IShouldBeAbleTo("edit", "customer");
            Then.IShouldBeAbleTo("delete", "customer");
            Then.IShouldBeAbleTo("discombobulate", "customer");
            Then.IShouldNotBeAbleTo("view", "order");
        }

        [Test]
        public void allow_all__allows_anything_on_the_subject()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowAll().On("customer"));
            Then.IShouldBeAbleTo("view", "customer");
            Then.IShouldBeAbleTo("create", "customer");
            Then.IShouldBeAbleTo("edit", "customer");
            Then.IShouldBeAbleTo("delete", "customer");
            Then.IShouldBeAbleTo("discombobulate", "customer");
            Then.IShouldNotBeAbleTo("view", "order");
        }

        [Test]
        public void if_subject_is_all__you_can_do_the_action_on_everything()
        {
            AbilityConfiguration.ConfigureWith(c => c.AllowTo("view", "all"));
            Then.IShouldBeAbleTo("view", "customer");
            Then.IShouldBeAbleTo("view", "order");
            Then.IShouldNotBeAbleTo("edit", "customer");
        }

        [Test]
        public void allow_all_on_anything__allows_everything()
        {
            AbilityConfiguration.ConfigureWith(c => c.Allow("view").OnEverything());
            Then.IShouldBeAbleTo("view", "customer");
            Then.IShouldBeAbleTo("view", "order");
            Then.IShouldNotBeAbleTo("edit", "customer");
        }


    }
}
