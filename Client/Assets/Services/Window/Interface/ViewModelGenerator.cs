using System;
using CommunityToolkit.Mvvm.DependencyInjection;
using UnityEngine;

[Serializable]
public class ViewModelGenerator
{
    private static ViewModelGenerator _default = new ViewModelGenerator();
    public static ViewModelGenerator Default => _default;
    
    [SerializeField]
    private SerializableDictionary<int, IViewModel> viewModels;

    private ViewModelGenerator()
    {
        viewModels = new SerializableDictionary<int, IViewModel>();
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
    public bool Delete(int uniqueId)
    {
        if (!viewModels.Remove(uniqueId))
        {
            LLogger.Error($"不存在 UniqueId: {uniqueId} 映射的 {nameof(IViewModel)}");
            return false;
        }
        return true;
    }
}
