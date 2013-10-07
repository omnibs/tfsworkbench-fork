using System;
using System.Collections.Generic;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.NotePadUI.Services
{
    public interface IPadLayoutService
    {
        IList<PadItemBase> GetWorkspaceLayout(IProjectData projectData);
        IList<PadItemBase> GetWorkspaceLayout(IProjectData projectData, Predicate<IWorkbenchItem> filter);
        void Add(PadItemBase itemToAdd);
        void Save();
        void Remove(PadItemBase padItemToRemove);
    }
}