﻿using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class PagedResourceCollectionViewModel : PagedCollectionViewModelBase<Resource, PagedResourceCollection, ResourceElement>
    {
        public PagedResourceCollectionViewModel(PagedResourceCollection endpoint) : base(endpoint)
        {
            DisplayName = "Resources (paged)";
        }

        protected override IScreen BuildElementScreen(ResourceElement elementEndpoint)
        {
            return new ResourceElementViewModel(elementEndpoint);
        }

        protected override IScreen BuildCreateElementScreen()
        {
            return new CreateResourceElementViewModel(Endpoint);
        }
    }
}