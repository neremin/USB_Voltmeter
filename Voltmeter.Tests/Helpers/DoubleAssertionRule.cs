using System;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;

namespace Voltmeter.Tests.Helpers
{
    sealed class DoubleAssertionRule : IAssertionRule
    {
        readonly double TOLERANCE;

        public DoubleAssertionRule(double tolerance)
        {
            TOLERANCE = tolerance;
        }

        public bool AssertEquality(IEquivalencyValidationContext context)
        {
            if (context.RuntimeType != typeof (double))
            {
                return false;
            }
            var expectation = context.Expectation.As<double>();
            var actual      = context.Subject.As<double>();
            var delta       = actual - expectation;

            Execute.Assertion.ForCondition(Math.Abs(delta) <= TOLERANCE)
                .BecauseOf(string.Empty)
                .FailWith(string.Format(
                    "Expected property {0} to be {1} (+/- {2}), but found {3}. Differed by {4}.", 
                    context.PropertyInfo.Name, expectation, TOLERANCE, actual, delta
                ));

            return true;
        }
    }
}
