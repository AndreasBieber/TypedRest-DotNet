﻿using Caliburn.Micro;
using TypedRest.Samples.Library.Endpoints;
using TypedRest.Samples.Library.Models;
using TypedRest.Wpf.ViewModels;

namespace TypedRest.Samples.Wpf.ViewModels
{
    public class ResourceCollectionViewModel : CollectionViewModelBase<Resource, ResourceCollection, ResourceElement>
    {
        public ResourceCollectionViewModel(ResourceCollection endpoint) : base(endpoint)
        {
            DisplayName = "Resources";
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