using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

namespace NetDiscovery.Lib
{
    [DesignTimeVisible(false)]
    public abstract class DiscoveryNode<T> : DiscoveryItem, IDisposable where T : DiscoveryItem
    {
        public DateTime Created { get; } = DateTime.Now;
        private DateTime updated;
        public DateTime Updated
        {
            get => updated;
            internal set => this.RaiseAndSetIfChanged(ref updated, value);
        }

        internal SourceList<T> updatebleNodes = new SourceList<T>();
        protected ReadOnlyObservableCollection<T> nodes;
        IDisposable nodesDisposable;

        protected ReadOnlyObservableCollection<T> Nodes => nodes;

        protected static IComparer<T> DefaultComparer => SortExpressionComparer<T>.Ascending(x => x.Name);

        protected DiscoveryNode(IComparer<T> comparer)
        {
            updated = DateTime.Now;
            nodesDisposable = updatebleNodes.Connect().ObserveOn(RxApp.MainThreadScheduler).Sort(comparer).Bind(out nodes).Subscribe();
        }


        internal virtual void AddChild(T child)
        {
            updatebleNodes.Edit(l =>
            {
                if (!l.Contains(child))
                    l.Add(child);
            });
        }



        // To detect redundant calls
        private bool _disposed = false;

        /// <summary>
        /// The <see cref="IDisposable" /> implementation.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                nodesDisposable?.Dispose();
                nodesDisposable = null;
                updatebleNodes?.Dispose();
                updatebleNodes = null;

            }

            // Dispose of any unmanaged resources not wrapped in safe handles.

            _disposed = true;
        }

    }
}
