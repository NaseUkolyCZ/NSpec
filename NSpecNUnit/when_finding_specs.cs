using System;
using NSpec;
using NSpec.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSpecNUnit
{
    [TestFixture]
    public class when_finding_specs
    {
        private ISpecFinder finder;
        private IReflector reflector;
        private string someDLL;

        [SetUp]
        public void setup()
        {
            GivenDllContains();
        }

        private void GivenDllContains(params Type[] types)
        {
            reflector = MockRepository.GenerateMock<IReflector>();

            reflector.Stub(r => r.GetTypesFrom("")).IgnoreArguments().Return(types);

            someDLL = "an nspec project dll";

            finder = new SpecFinder(someDLL, reflector);
        }

        [Test]
        public void it_should_get_types_from_reflection()
        {
            reflector.AssertWasCalled(r=>r.GetTypesFrom(someDLL));
        }

        [Test]
        public void it_should_include_classes_that_implement_spec_and_have_public_methods()
        {
            GivenDllContains(typeof(SpecClass));

            finder.SpecClasses().should_contain(typeof(SpecClass));
        }

        [Test]
        public void it_should_exclude_classes_that_inherit_from_spec_but_have_no_public_methods()
        {
            GivenDllContains(typeof(SpecClassWithNoPublicMethods));

            finder.SpecClasses().should_be_empty();
        }

        [Test]
        public void it_should_exclude_classes_that_do_not_inherit_from_spec()
        {
            GivenDllContains(typeof(NonSpecClass));

            finder.SpecClasses().should_be_empty();
        }
    }
}