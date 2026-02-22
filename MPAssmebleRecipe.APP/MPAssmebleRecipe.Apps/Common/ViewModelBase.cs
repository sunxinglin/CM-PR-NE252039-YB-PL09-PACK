using Prism.Mvvm;
using MPAssmebleRecipe.Logger.Interfaces;

namespace MPAssmebleRecipe.Apps.Common
{
    public class ViewModelBase : BindableBase
    {
        protected readonly ILoggerHelper Logger;

        public ViewModelBase(ILoggerHelper logger)
        {
            Logger = logger;
        }
    }
}