using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMP.Models;
using SMP.Models.Repositoryes;

namespace SMP.ViewModels
{
    public class WorkViewModel
    {
        Project project;
        List<Item> inlineWorks = new List<Item>();
        List<int> selectedValues = new List<int>();
        public Project Project { get => project; }
        public List<Item> InlineWorks { get => inlineWorks; }
        public List<int> SelectedValues { get => selectedValues; }

        public WorkViewModel(int idProject)
        {
            var model = new ModelContainer();
            var projectRepository = new ProjectRepository(model);
            project = projectRepository.GetProjectById(idProject);

            foreach (Project p in projectRepository.GetProjectsByParrentId(
                projectRepository.GetProjectById(idProject).parrentProject.IdProject))
            {
                if (p.IdProject != idProject)
                {
                    inlineWorks.Add(new Item(p.IdProject, p.projectName));
                    selectedValues.Add(p.IdProject);
                }
            }
        }
    }

    public class Item
    {
        int id;
        string text;
        public int Id { get => id; }
        public string Text { get => text; }

        public Item(int id, string text)
        {
            this.id = id;
            this.text = text;
        }
    }
}