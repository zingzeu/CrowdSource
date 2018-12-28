using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zezo.Core.GrainInterfaces.Observers;

namespace Zezo.Core.Grains
{
    public partial class StepGrain
    {
        public Task Subscribe(IStepGrainObserver observer)
        {
            State.Observers.Add(observer);
            return WriteStateAsync();
        }

        public Task Unsubscribe(IStepGrainObserver observer)
        {
            State.Observers.Remove(observer);
            return WriteStateAsync();
        }
        
        /// <summary>
        /// Notifies all observers.
        /// </summary>
        /// <param name="notification">
        /// The notification delegate to call on each observer.
        /// </param>
        /// <param name="predicate">The predicate used to select observers to notify.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the work performed.
        /// </returns>
        private async Task Notify(Func<IStepGrainObserver, Task> notification, Func<IStepGrainObserver, bool> predicate = null)
        {
            var defunct = default(List<IStepGrainObserver>);
            foreach (var observer in State.Observers)
            {
                // Skip observers which don't match the provided predicate.
                if (predicate != null && !predicate(observer))
                {
                    continue;
                }

                try
                {
                    await notification(observer);
                }
                catch (Exception)
                {
                    // Failing observers are considered defunct and will be removed..
                    defunct = defunct ?? new List<IStepGrainObserver>();
                    defunct.Add(observer);
                }
            }

            // Remove defunct observers.
            if (defunct != default(List<IStepGrainObserver>))
            {
                foreach (var observer in defunct)
                {
                    State.Observers.Remove(observer);
                }

                await WriteStateAsync();
            }
        }
        
        /// <summary>
        /// Notifies all observers which match the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="notification">
        /// The notification delegate to call on each observer.
        /// </param>
        /// <param name="predicate">The predicate used to select observers to notify.</param>
        private void Notify(Action<IStepGrainObserver> notification, Func<IStepGrainObserver, bool> predicate = null)
        {
            var defunct = default(List<IStepGrainObserver>);
            foreach (var observer in State.Observers)
            {
                // Skip observers which don't match the provided predicate.
                if (predicate != null && !predicate(observer))
                {
                    continue;
                }

                try
                {
                    notification(observer);
                }
                catch (Exception)
                {
                    // Failing observers are considered defunct and will be removed..
                    defunct = defunct ?? new List<IStepGrainObserver>();
                    defunct.Add(observer);
                }
            }

            // Remove defunct observers.
            if (defunct != default(List<IStepGrainObserver>))
            {
                foreach (var observer in defunct)
                {
                    State.Observers.Remove(observer);
                }

                WriteStateAsync();
            }
        }
    }
}