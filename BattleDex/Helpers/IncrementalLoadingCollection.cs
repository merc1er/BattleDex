using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;

using Microsoft.UI.Xaml.Data;

using Windows.Foundation;

namespace BattleDex.Helpers;

/// <summary>
/// An ObservableCollection that supports incremental loading for virtualized lists.
/// Items are loaded in batches as the user scrolls.
/// </summary>
public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
{
    private readonly IList<T> _source;
    private readonly int _batchSize;
    private int _currentIndex;

    public IncrementalLoadingCollection(IList<T> source, int batchSize = 50)
    {
        _source = source;
        _batchSize = batchSize;
        _currentIndex = 0;
    }

    public bool HasMoreItems => _currentIndex < _source.Count;

    public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
    {
        return AsyncInfo.Run(async _ =>
        {
            // Small yield to let UI breathe
            await Task.Delay(1);

            var itemsToLoad = Math.Min(_batchSize, _source.Count - _currentIndex);

            for (var i = 0; i < itemsToLoad; i++)
            {
                Add(_source[_currentIndex++]);
            }

            return new LoadMoreItemsResult { Count = (uint)itemsToLoad };
        });
    }

    /// <summary>
    /// Loads the initial batch of items synchronously.
    /// </summary>
    public void LoadInitialItems()
    {
        var itemsToLoad = Math.Min(_batchSize, _source.Count);

        for (var i = 0; i < itemsToLoad; i++)
        {
            Add(_source[_currentIndex++]);
        }
    }
}
