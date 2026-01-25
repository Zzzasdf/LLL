using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;

public class ViewModelGenerator
{
    private static ViewModelGenerator _default = new ViewModelGenerator();
    public static ViewModelGenerator Default => _default;
    
    private Dictionary<int, IViewModel> viewModels;

    private ViewModelGenerator()
    {
        viewModels = new Dictionary<int, IViewModel>();
    }
    
    public TViewModel GetOrAdd<TViewModel>(int uniqueId) where TViewModel: class, IViewModel
    {
        if (!viewModels.TryGetValue(uniqueId, out IViewModel viewModel))
        {
            viewModel = Ioc.Default.GetRequiredService<TViewModel>();
            viewModels.Add(uniqueId, viewModel);
        }
        return viewModel as TViewModel;
    }
    public bool Remove(int uniqueId)
    {
        if (!viewModels.Remove(uniqueId))
        {
            LLogger.Error($"不存在 UniqueId: {uniqueId} 映射的 {nameof(IViewModel)}");
            return false;
        }
        return true;
    }
}
