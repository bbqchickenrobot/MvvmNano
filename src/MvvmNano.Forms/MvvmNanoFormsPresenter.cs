﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MvvmNano;
using Xamarin.Forms;

namespace MvvmNano.Forms
{
    public class MvvmNanoFormsPresenter : IPresenter
    {
        private readonly Type[] _availableViewTypes;

        protected readonly Application Application;

        public MvvmNanoFormsPresenter(Application application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            Application = application;

            _availableViewTypes = Application
                .GetType()
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Select(t => t.AsType())
                .ToArray();
        }

        public Task ShowViewModelAsync<TViewModel, TNavigationParameter>(TNavigationParameter parameter)
        {
            Type viewModelType = typeof(TViewModel);

            var viewModel = CreateViewModel<TViewModel, TNavigationParameter>(viewModelType);
            viewModel.Initialize(parameter);

            IView view = CreateView(viewModelType);
            view.SetViewModel(viewModel);

            return OpenPageAsync(view as Page);
        }

        public Task ShowViewModelAsync<TViewModel>()
        {
            Type viewModelType = typeof(TViewModel);

            var viewModel = CreateViewModel<TViewModel>(viewModelType) as MvvmNanoViewModel;
            if (viewModel == null)
                throw new MvvmNanoFormsPresenterException(viewModelType + " is not a MvvmNanoViewModel.");
            
            viewModel.Initialize();

            IView view = CreateView(viewModelType);
            view.SetViewModel(viewModel);

            return OpenPageAsync(view as Page);
        }

        private static IViewModel CreateViewModel<TViewModel>(Type viewModelType)
        {
            var viewModel = MvvmNanoIoC.Resolve<TViewModel>() as IViewModel;
            if (viewModel == null)
                throw new MvvmNanoFormsPresenterException(viewModelType + " does not implement IViewModel.");

            return viewModel;
        }

        private static IViewModel<TNavigationParameter> CreateViewModel<TViewModel, TNavigationParameter>(Type viewModelType)
        {
            var viewModel = MvvmNanoIoC.Resolve<TViewModel>() as IViewModel<TNavigationParameter>;
            if (viewModel == null)
                throw new MvvmNanoFormsPresenterException(viewModelType + " does not implement IViewModel<" + typeof(TNavigationParameter).Name + ">");

            return viewModel;
        }

        private IView CreateView(Type viewModelType)
        {
            string viewName = viewModelType.Name.Replace("ViewModel", "Page");
            Type pageType = _availableViewTypes
                .FirstOrDefault(t => t.Name == viewName);

            var view = Activator.CreateInstance(pageType) as IView;

            if (view == null)
                throw new MvvmNanoFormsPresenterException(viewName + " could not be found. Does it implement IView?");

            if (!(view is Page))
                throw new MvvmNanoFormsPresenterException(viewName + " is not a Xamarin.Forms Page.");

            return view;
        }

        protected virtual Task OpenPageAsync(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            return Application.MainPage.Navigation.PushAsync(page, true);
        }
    }
}
