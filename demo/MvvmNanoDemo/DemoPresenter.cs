﻿using System.Threading.Tasks;
using MvvmNano.Forms;
using Xamarin.Forms;

namespace MvvmNanoDemo
{
    public class DemoPresenter : MvvmNanoFormsPresenter
    {
        public DemoPresenter(Application app) : base(app)
        {
        }

        protected override Task OpenPageAsync(Page page)
        {
            if (page is AboutPage)
            {
                return CurrentPage.Navigation.PushModalAsync(new MvvmNanoNavigationPage(page));
            }

            if (page is WelcomePage)
            {
                Application.MainPage = new MvvmNanoNavigationPage(page);
                return CurrentPage.Navigation.PopToRootAsync(false);
            }

            return base.OpenPageAsync(page);
        }
    }
}

