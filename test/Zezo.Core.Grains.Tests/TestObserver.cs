using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Orleans;
using Xunit;
using Xunit.Abstractions;
using Zezo.Core.GrainInterfaces;
using Zezo.Core.GrainInterfaces.Observers;

namespace Zezo.Core.Grains.Tests
{
    internal class TestObserver : IStepGrainObserver, IDisposable
    {
        
        private readonly IGrainFactory _grainFactory;
        private readonly ITestOutputHelper _testOutputHelper;
        private IStepGrainObserver selfReference = null;
        public TimeSpan WaitingTimeout { get; set; } = TimeSpan.FromSeconds(30);

        private Task<IStepGrainObserver> SelfReference
        {
            get
            {
                if (selfReference != null)
                {
                    return Task.FromResult(selfReference);
                }
                else
                {
                    return _grainFactory.CreateObjectReference<IStepGrainObserver>(this)
                        .ContinueWith((x) =>
                        {
                            selfReference = x.Result;
                            return selfReference;
                        });
                }
            }
        }

        private int counter = -1;

        private Dictionary<string, IStepGrain> idToGrainMapping = new Dictionary<string, IStepGrain>();
        private Dictionary<Guid, string> guidToIdMapping = new Dictionary<Guid, string>();
        private Dictionary<string, List<(StepStatus, int)>> statusHistory 
            = new Dictionary<string, List<(StepStatus, int)>>();
        private ConcurrentDictionary<string, ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>>
            awaitingTasks 
                = new ConcurrentDictionary<string, ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>>();

        public TestObserver(ITestOutputHelper testOutputHelper, IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
            _testOutputHelper = testOutputHelper;
        }
        
        private IReadOnlyList<(StepStatus, int)> GetStatusHistory(string stepId)
        {
            if (statusHistory.ContainsKey(stepId))
            {
                return new List<(StepStatus, int)>(statusHistory[stepId]);
            }
            else
            {
                return new List<(StepStatus, int)>();
            }
        }

        public void OnStatusChanged(Guid caller, StepStatus newStatus)
        {
            var now = DateTime.Now;
            ++counter;
            var stepId = guidToIdMapping[caller];
            Assert.NotNull(stepId);
            if (!statusHistory.ContainsKey(stepId))
            {
                statusHistory[stepId] = new List<(StepStatus, int)>(); 
            }
            statusHistory[stepId].Add((newStatus, counter));
            _testOutputHelper
                .WriteLine($"({counter}) {now.Minute}:{now.Second}.{now.Millisecond} - Step {stepId} changed to [{newStatus}]");
            // check awaiting tasks
            foreach (var (predicate, tcs) in awaitingTasks[stepId])
            {
                if (predicate(newStatus))
                {
                    tcs.SetResult(null);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task WaitUntilStatus(string stepId, Func<StepStatus, bool> predicate)
        {
            var sh = GetStatusHistory(stepId);

            if (sh.Count > 0)
            {
                foreach (var (s, _) in sh)
                {
                    if (predicate(s))
                        return Task.CompletedTask;
                }
            }

            
            // if already waiting
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            awaitingTasks[stepId].Add((predicate, tcs));            
            
            Task.Run(async () =>
            {
                await Task.Delay(WaitingTimeout);
                tcs.TrySetException(new TimeoutException());
            });
            
            
            return tcs.Task;
        }
        
        public async Task ObserverStep(IStepGrain step, string stepId)
        {
            await step.Subscribe(await SelfReference);
            idToGrainMapping[stepId] = step;
            guidToIdMapping[step.GetPrimaryKey()] = stepId;
            awaitingTasks[stepId] = new ConcurrentBag<(Func<StepStatus, bool>, TaskCompletionSource<object>)>();
        }

        public void Dispose()
        {
            foreach (var stepGrain in idToGrainMapping.Values)
            {
                stepGrain.Unsubscribe(SelfReference.Result).GetAwaiter().GetResult();
            }
        }

        public ObservedSequence Observed()
        {
            return new ObservedSequence(this);
        }
        
        internal class ObservedSequence {
            private readonly TestObserver _testObserver;
            private IList<(string, Func<StepStatus, bool>)> predicates = new List<(string, Func<StepStatus, bool>)>();

            public ObservedSequence(TestObserver testObserver)
            {
                _testObserver = testObserver;
            }

            public ObservedSequence StartsWith(string stepId, Func<StepStatus, bool> predicate)
            {
                predicates.Add((stepId, predicate));
                return this;
            }

            public ObservedSequence LaterOn(string stepId, Func<StepStatus, bool> predicate)
            {
                predicates.Add((stepId, predicate));
                return this;
            }

            public void Validate()
            {
                int lastCount = -1;
                int i = 0;
                foreach (var (stepId, predicate) in predicates)
                {
                    ++i;
                    var found = false;
                    foreach (var (status, sequence) in _testObserver.statusHistory[stepId])
                    {
                        if (predicate(status))
                        {
                            if (sequence > lastCount)
                            {
                                lastCount = sequence;
                                found = true;
                                break;
                            }
                            else
                            {
                                throw new Exception($"Condition #{i} happened before last one.");
                            }
                        }
                    }
                    if (!found)
                    {
                        throw new Exception($"Condition #{i} did not happen.");  
                    }
                }
            }
            
        }
    }
}