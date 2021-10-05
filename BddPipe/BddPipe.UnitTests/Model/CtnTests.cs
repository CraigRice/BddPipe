using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model
{
    [TestFixture]
    public class CtnTests
    {
        private const int DefaultValue = 32;
        private const string ScenarioTitle = "scenario-title";

        [Test]
        public void Ctor_NullReferenceType_DoesNotThrow()
        {
            var ctn = new Ctn<string>(null, None);

            ctn.Should().NotBeNull();
            ctn.Content.Should().Be(null);
            ctn.StepOutcomes.Should().NotBeNull();
            ctn.StepOutcomes.Count.Should().Be(0);
            ctn.ScenarioTitle.ShouldBeNone();
        }

        [Test]
        public void Ctor_NoTitle_CreatesCtnCorrectly()
        {
            var ctn = new Ctn<int>(DefaultValue, None);

            ctn.Should().NotBeNull();
            ctn.Content.Should().Be(DefaultValue);
            ctn.StepOutcomes.Should().NotBeNull();
            ctn.StepOutcomes.Count.Should().Be(0);
            ctn.ScenarioTitle.ShouldBeNone();
        }

        [Test]
        public void Ctor_Title_CreatesCtnCorrectly()
        {
            var ctn = new Ctn<int>(DefaultValue, ScenarioTitle);

            ctn.Should().NotBeNull();
            ctn.Content.Should().Be(DefaultValue);
            ctn.StepOutcomes.Should().NotBeNull();
            ctn.StepOutcomes.Count.Should().Be(0);
            ctn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
        }

        [Test]
        public void Ctor_NoTitleAndList_CreatesCtnCorrectly()
        {
            var ctn = new Ctn<int>(DefaultValue, new List<StepOutcome>(), None);

            ctn.Should().NotBeNull();
            ctn.Content.Should().Be(DefaultValue);
            ctn.StepOutcomes.Should().NotBeNull();
            ctn.StepOutcomes.Count.Should().Be(0);
            ctn.ScenarioTitle.ShouldBeNone();
        }

        [Test]
        public void Ctor_TitleAndList_CreatesCtnCorrectly()
        {
            var ctn = new Ctn<int>(DefaultValue, new List<StepOutcome>(), ScenarioTitle);

            ctn.Should().NotBeNull();
            ctn.Content.Should().Be(DefaultValue);
            ctn.StepOutcomes.Should().NotBeNull();
            ctn.StepOutcomes.Count.Should().Be(0);
            ctn.ScenarioTitle.ShouldBeSome(title => title.Should().Be(ScenarioTitle));
        }

        [Test]
        public void Ctor_StepOutcomesNull_ThrowsArgumentNullException()
        {
            Action create = () =>
            {
                var ctn = new Ctn<int>(DefaultValue, null, None);
            };

            create.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("stepOutcomes");
        }
    }
}
