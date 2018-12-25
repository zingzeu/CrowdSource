using Xunit;
using Zezo.Core.GrainInterfaces;

namespace Zezo.Core.Grains.Tests
{
    public class StepGrainStatusTest
    {
        [Theory]
        [InlineData(StepStatus.Completed)]
        [InlineData(StepStatus.Error)]
        [InlineData(StepStatus.Skipped)]
        public void Stopped_bit_Positive_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0100_0000) != 0);
        }
        
        [Theory]
        [InlineData(StepStatus.Uninitialized)]
        [InlineData(StepStatus.Initializing)]
        [InlineData(StepStatus.Inactive)]
        [InlineData(StepStatus.Active)]
        [InlineData(StepStatus.Paused)]
        [InlineData(StepStatus.Working)]
        [InlineData(StepStatus.Stopping)]
        public void Stopped_bit_Negative_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0100_0000) == 0);
        }

        [Theory]
        [InlineData(StepStatus.Initializing)]
        [InlineData(StepStatus.Uninitialized)]
        public void Init_bit_Negative_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b1000_0000) == 0);
        }
        
        [Theory]
        [InlineData(StepStatus.Inactive)]
        [InlineData(StepStatus.Active)]
        [InlineData(StepStatus.Working)]
        [InlineData(StepStatus.Pausing)]
        [InlineData(StepStatus.Paused)]
        [InlineData(StepStatus.Stopping)]
        [InlineData(StepStatus.Resuming)]
        [InlineData(StepStatus.Error)]
        [InlineData(StepStatus.Completed)]
        [InlineData(StepStatus.Skipped)]
        public void Init_bit_Positive_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b1000_0000) != 0);
        }


        [Theory]
        [InlineData(StepStatus.Error)]
        public void Error_bit_Positive_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0000_1000) != 0);
        }
        
        [Theory]
        [InlineData(StepStatus.Skipped)]
        [InlineData(StepStatus.Completed)]
        public void Error_bit_Negative_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0000_1000) == 0);
        }

        [Theory]
        [InlineData(StepStatus.Pausing)]
        [InlineData(StepStatus.Stopping)]
        [InlineData(StepStatus.Resuming)]
        [InlineData(StepStatus.Working)]
        [InlineData(StepStatus.Initializing)]
        public void Busy_bit_Positive_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0001_0000) != 0);
        }
        
        [Theory]
        [InlineData(StepStatus.Uninitialized)]
        [InlineData(StepStatus.Inactive)]
        [InlineData(StepStatus.Active)]
        [InlineData(StepStatus.Paused)]
        [InlineData(StepStatus.Skipped)]
        [InlineData(StepStatus.Completed)]
        [InlineData(StepStatus.Error)]
        public void Busy_bit_Negative_Test(StepStatus status)
        {
            Assert.True((status & (StepStatus) 0b0001_0000) == 0);
        }
        
        [Fact]
        public void StepStatuses_Are_Unique()
        {
            EnsureAllDifferent(StepStatus.Uninitialized,
                StepStatus.Initializing,
                StepStatus.Inactive,
                StepStatus.Paused,
                StepStatus.Active,
                StepStatus.Working,
                StepStatus.Pausing,
                StepStatus.Stopping,
                StepStatus.Resuming,
                StepStatus.Completed,
                StepStatus.Skipped,
                StepStatus.Error);
        }

        private static void EnsureAllDifferent(params StepStatus[] statuses)
        {
            for (int i = 0; i < statuses.Length - 1; ++i)
            {
                for (int j = i+1; j < statuses.Length; ++j)
                    Assert.NotEqual(statuses[i], statuses[j]);
            }
        }
    }
}