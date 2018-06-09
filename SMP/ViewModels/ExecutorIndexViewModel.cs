using SMP.Models;
using SMP.Models.Repositoryes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP.Views.VievModels
{
    /// <summary>
    /// Модель для представления 
    /// </summary>
    public class ExecutorIndexViewModel
    {
        Person person;
        List<Project> works = null;

        public Person Person { get => person; }
        public List<Project> Projects { get => works; }
        public List<Project> InnerProjects { get => Projects.Select(x => (GetInnerProject(x))).ToList(); }

        public string PersonNameString { get => $"{person.firstName} {person.surName} {person.middleName}"; }

        public ExecutorIndexViewModel (int idPerson)
        {
            var model_container = new ModelContainer();
            var project_rep = new ProjectRepository(model_container);
            person = model_container.Person.FirstOrDefault(x => x.IdPerson == idPerson);
            if (person == null)
                throw new Exception("Персоны с таким id нет в базе");
            else
                works = project_rep.GetProjectsByPersonId(person.IdPerson)
                    .Where(x=> !x.isClose && !x.isDone && x.parrentProject != null)
                    .ToList();
        }

        
        /// <summary>
        /// Возвращает проект верхнего уровня для проекта нижнего уровня.
        /// </summary>
        /// <param name="child">Проект нижнего уровня</param>
        /// <returns></returns>
        private Project GetInnerProject (Project child)
        {
            if (child == null)
                return null;
            Project res = child;
            while (res.parrentProject != null)
            {
                res = res.parrentProject;
            }
            return res;
        }
    }
}