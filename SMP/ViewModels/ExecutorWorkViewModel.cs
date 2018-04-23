using SMP.Models;
using SMP.Models.Repositoryes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.ViewModels
{
    public class ExecutorWorkViewModel
    {
        Person person;
        Project project;
        public Person Person { get => person; }
        public Project Project { get => project; }

        public ExecutorWorkViewModel (int idProject, int idPerson)
        {
            var model = new ModelContainer();
            person = model.Person.FirstOrDefault(x => x.IdPerson == idPerson);
            var projectRepository = new ProjectRepository(model);
            project = projectRepository.GetProjectById(idProject);
        }
    }
}